using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using ElgatoWaveSDK.HumbleObjects;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK
{
    public class ElgatoWaveClient
    {
        #region Private Vars

        private IHumbleClientWebSocket? _socket;
        private ClientConfig Config { get; set; }
        private int Port { get; set; }
        private CancellationTokenSource? _source;
        private readonly ITransactionTracker _transactionTracker;

        #endregion

        #region Public Vars

        public ClientConfig ClientConfig => Config;
        public bool IsConnected => _socket?.State == WebSocketState.Open;

        #endregion

        #region Public Events

        public event EventHandler<MicrophoneState>? MicStateChanged;
        public event EventHandler<MicrophoneSettings>? MicSettingsChanged;
        public event EventHandler<ChannelInfo>? InputMixerChanged;
        public event EventHandler<MonitoringState>? OutputMixerChanged;
        public event EventHandler<string>? LocalMonitorOutputChanged;
        public event EventHandler<MixType>? MonitorSwitchOutputChanged;
        public event EventHandler<List<ChannelInfo>>? ChannelsChanged;

        public event EventHandler<ElgatoException>? ExceptionOccured;

        #endregion

        public ElgatoWaveClient()
        {
            Config ??= new ClientConfig();
            _source = new CancellationTokenSource();
            _transactionTracker ??= new TransactionTracker();

            Port = Config.PortStart;
        }

        public ElgatoWaveClient(ClientConfig config) : this()
        {
            Config = config;
        }

        internal ElgatoWaveClient(IHumbleClientWebSocket socket, ITransactionTracker transactionTracker) : this()
        {
            _socket = socket;
            _transactionTracker = transactionTracker;
        }

        #region Connection
        public async Task ConnectAsync()
        {
            var cycleCount = 0;
            while (_socket?.State != WebSocketState.Open && (cycleCount < Config.MaxAttempts))
            {
                _socket ??= new HumbleClientWebSocket();
                try
                {
                    await _socket
                        .ConnectAsync(new Uri($"ws://127.0.0.1:{Port}/"), _source?.Token ?? CancellationToken.None)
                        .ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //ignore for now
                }
                finally
                {
                    Port++;
                    if (Port > (Config.PortStart + Config.PortRange))
                    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                        cycleCount++;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                        Port = Config.PortStart;
                    }
                }
            }

            if (_socket?.State != WebSocketState.Open)
            {
                var ex = new ElgatoException($"Looped through possible ports {Config.MaxAttempts} times and couldn't connect [{Config.PortStart}-{Config.PortStart + Config.PortRange}]", _socket?.State);
                ExceptionOccured?.Invoke(this, ex);
                throw ex;
            }

            StartReceiver();
        }

        public void Disconnect()
        {
            _source?.Cancel();
            _source = null;

            _socket?.Dispose();
            _socket = null;
        }
        #endregion Connection

        #region Commands

        #region Get Commands

        public Task<ApplicationInfo?> GetAppInfo()
        {
            return SendCommand<ApplicationInfo>("getApplicationInfo");
        }

        public Task<List<ChannelInfo>?> GetAllChannelInfo()
        {
            return SendCommand<List<ChannelInfo>>("getAllChannelInfo");
        }

        public Task<MicrophoneState?> GetMicrophoneState()
        {
            return SendCommand<MicrophoneState>("getMicrophoneState");
        }

        public Task<MicrophoneSettings?> GetMicrophoneSettings()
        {
            return SendCommand<MicrophoneSettings>("getMicrophoneSettings");
        }

        public Task<MonitoringState?> GetMonitoringState()
        {
            return SendCommand<MonitoringState>("getMonitoringState");
        }

        public Task<MonitorMixOutputList?> GetMonitorMixOutputList()
        {
            return SendCommand<MonitorMixOutputList>("getMonitorMixOutputList");
        }

        public Task<SwitchState?> GetSwitchState()
        {
            return SendCommand<SwitchState>("getSwitchState");
        }

        #endregion Get Commands

        #region Set Commands

        public Task<MonitorMixOutputList?> SetMonitorMixOutput(string mixOutput)
        {
            return SendCommand<MonitorMixOutputList, MonitorMixOutputList>("setMonitorMixOutput", new MonitorMixOutputList()
            {
                MonitorMix = mixOutput
            });
        }

        public Task<SwitchState?> SetMonitoringState(MixType mix)
        {
            return SendCommand<SwitchState, SwitchState>("switchMonitoring", new SwitchState()
            {
                CurrentState = mix.ToString()
            });
        }

        public Task<MicrophoneSettings?> SetMicrophoneSettings(int micGain, int micOutputVol, int micBalcnce, bool isMicLowcutOn, bool isMicClipgaurdOn)
        {
            return SetMicrophoneSettings(new MicrophoneSettings()
            {
                MicrophoneGain = micGain,
                MicrophoneOutputVolume = micOutputVol,
                MicrophoneBalance = micBalcnce,
                IsMicrophoneLowcutOn = isMicLowcutOn,
                IsMicrophoneClipguardOn = isMicClipgaurdOn,
            });
        }

        public Task<MicrophoneSettings?> SetMicrophoneSettings(MicrophoneSettings settings)
        {
            return SendCommand<MicrophoneSettings, MicrophoneSettings>("setMicrophoneSettings", settings);
        }

        public Task<MonitoringState?> SetOutputMixer(MonitoringState state)
        {
            return SendCommand<MonitoringState, MonitoringState>("setOutputMixer", state);
        }

        public Task<MonitoringState?> SetOutputMixer(int localVolumeOut, bool isLocalOutMuted, int streamVolumeOut, bool isStreamOutMuted)
        {
            return SetOutputMixer(new MonitoringState()
            {
                IsStreamOutMuted = isStreamOutMuted,
                LocalVolumeOut = localVolumeOut,
                StreamVolumeOut = streamVolumeOut,
                IsLocalOutMuted = isLocalOutMuted
            });
        }

        public Task<ChannelInfo?> SetInputMixer(string mixId, int localVol, bool isLocalMuted, int remoteVol, bool isStreamMuted, List<Filter> filters, bool localByPass, bool streamByPass, MixType mixType)
        {
            return SetInputMixer(new ChannelInfo()
            {
                MixId = mixId,
                LocalVolumeIn = localVol,
                IsLocalInMuted = isLocalMuted,
                StreamVolumeIn = remoteVol,
                IsStreamInMuted = isStreamMuted,
                Filters = filters,
                LocalMixFilterBypass = localByPass,
                StreamMixFilterBypass = streamByPass
            }, mixType);
        }

        public Task<ChannelInfo?> SetInputMixer(ChannelInfo info, MixType mixType)
        {
            info.BgColor = null;
            info.IconData = null;
            info.InputType = null;
            info.IsAvailable = null;
            info.MixerName = null;
            info.Slider = mixType == MixType.LocalMix ? "local" : "stream";

            return SendCommand<ChannelInfo, ChannelInfo>("setInputMixer", info);
        }

        #endregion

        private Task<T?> SendCommand<T>(string method)
        {
            return SendCommand<T, T>(method, default);
        }

        private async Task<OutT?> SendCommand<OutT, InT>(string method, InT? objectJson = default)
        {
            if (_socket?.State == WebSocketState.Open)
            {
                SocketBaseObject<InT?, OutT?> baseObject = new()
                {
                    Method = method,
                    Id = _transactionTracker.NextTransactionId(),
                    Obj = objectJson
                };
                var s = baseObject.ToJson();
                var array = Encoding.UTF8.GetBytes(s);
                await _socket.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, _source?.Token ?? CancellationToken.None).ConfigureAwait(false);

                SpinWait.SpinUntil(() => _responseCache.ContainsKey(baseObject.Id), TimeSpan.FromMilliseconds(Config.ResponseTimeout));

                if (_responseCache.ContainsKey(baseObject.Id))
                {
                    var reply = _responseCache[baseObject.Id];
                    _responseCache.Remove(reply.Id);

                    return JsonSerializer.Deserialize<OutT?>(reply?.Result ?? JsonDocument.Parse("{}"));
                }
            }

            return default;
        }

        #endregion Commands

        #region Reciever

        private readonly Dictionary<int, SocketBaseObject<JsonNode?, JsonDocument?>> _responseCache = new();

        private void StartReceiver()
        {
            Task.Run(ReceiverRun, _source?.Token ?? CancellationToken.None);
        }

        private async Task ReceiverRun()
        {
            while (!_source?.IsCancellationRequested ?? false)
            {
                if (_socket?.State == WebSocketState.Open)
                {
                    try
                    {
                        var buffer = new byte[Config.BufferSize];
                        var offset = 0;
                        var free = buffer.Length;

                        WebSocketReceiveResult? result = null;
                        do
                        {
                            result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, free), _source?.Token ?? CancellationToken.None).ConfigureAwait(false);
                            offset += result.Count;
                            free -= result.Count;
                            if (free == 0)
                            {
                                var newSize = buffer.Length + Config.BufferSize;
                                if (newSize > Config.MaxBufferSize)
                                {
                                    throw new ElgatoException("Maximum receive buffer size exceeded", _socket.State);
                                }
                                var newBuffer = new byte[newSize];
                                Array.Copy(buffer, 0, newBuffer, 0, offset);
                                buffer = newBuffer;
                                free = buffer.Length - offset;
                            }

                        } while (!result?.EndOfMessage ?? false);

                        var json = Encoding.UTF8.GetString(buffer).Replace("\0", "");

                        var baseObject = JsonSerializer.Deserialize<SocketBaseObject<JsonNode?, JsonDocument?>?>(json);
                        if (baseObject == null)
                        {
                            continue;
                        }

                        baseObject.ReceivedAt = DateTime.Now;

                        foreach (var cache in _responseCache.Where(c => c.Value.ReceivedAt < DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(2))))
                        {
                            _responseCache.Remove(cache.Key);
                        }

                        if (baseObject.Id == 0) //Not a command reply
                        {
                            object? obj = null;
                            switch (baseObject.Method)
                            {
                                case "microphoneStateChanged":
                                    obj = JsonSerializer.Deserialize<MicrophoneState>(baseObject.Obj?.ToString() ?? "{}");
                                    if (obj != null)
                                    {
                                        MicStateChanged?.Invoke(this, obj as MicrophoneState ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "microphoneSettingsChanged":
                                    obj = JsonSerializer.Deserialize<MicrophoneSettings>(baseObject.Obj?.ToString() ?? "{}");
                                    if (obj != null)
                                    {
                                        MicSettingsChanged?.Invoke(this, obj as MicrophoneSettings ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "localMonitorOutputChanged":
                                    obj = baseObject.Obj?["monitorMix"]?.ToString();
                                    if (obj != null)
                                    {
                                        LocalMonitorOutputChanged?.Invoke(this, obj as string ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "monitorSwitchOutputChanged":
                                    obj = baseObject.Obj?["switchState"]?.ToString();
                                    if (obj != null)
                                    {
                                        MonitorSwitchOutputChanged?.Invoke(this, obj?.ToString() == "LocalMix" ? MixType.LocalMix : MixType.StreamMix);
                                    }
                                    break;
                                case "channelsChanged":
                                    obj = JsonSerializer.Deserialize<List<ChannelInfo>>(baseObject.Obj?["channels"]?.ToString() ?? "{}");
                                    if (obj != null)
                                    {
                                        ChannelsChanged?.Invoke(this, obj as List<ChannelInfo> ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "outputMixerChanged":
                                    obj = JsonSerializer.Deserialize<MonitoringState>(baseObject.Obj?.ToString() ?? "{}");
                                    if (obj != null)
                                    {
                                        OutputMixerChanged?.Invoke(this, obj as MonitoringState ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "inputMixerChanged":
                                    obj = JsonSerializer.Deserialize<ChannelInfo>(baseObject.Obj?.ToString() ?? "{}");
                                    if (obj != null)
                                    {
                                        InputMixerChanged?.Invoke(this, obj as ChannelInfo ?? throw new InvalidOperationException());
                                    }
                                    break;
                                default:
                                    throw new ElgatoException($"Unsupported method received | {baseObject.Method}", _socket.State);

                            }
                        }
                        else //Command reply
                        {
                            if (!_responseCache.ContainsKey(baseObject.Id))
                            {
                                _responseCache.Add(baseObject.Id, baseObject);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionOccured?.Invoke(this, new ElgatoException("Unknown error in receiving task", ex, _socket.State));
                    }

                }
            }
        }
        #endregion
    }
}
