using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElgatoWaveAPI.Models;
using Newtonsoft.Json;

namespace ElgatoWaveAPI
{
    public class ElgatoWaveClient
    {
        #region Constants

        private const int _bufferSize = 1024;
        private const int _maxBufferSize = 2000000; //2gig is a massive max size

#if  DEBUG
        private const int _responseTimeout = 10 * 60 * 1000;  
#else
        private const int _responseTimeout = 2 * 1000;
#endif

        private const int _startPort = 1824;
        private const int _portRange = 10;
        private const int _maxCycles = 2;

        #endregion

        #region Private Vars
        private ClientWebSocket _socket;
        private int _port { get; set; }
        private CancellationTokenSource _source;
        #endregion

        #region Public Vars

        public bool IsConnected => _socket?.State == WebSocketState.Open;

        #endregion

        #region Public Events

        public EventHandler<MicrophoneState> MicStateChanged;
        public EventHandler<MicrophoneSettings> MicSettingsChanged;
        public EventHandler<ChannelInfo> InputMixerChanged;
        public EventHandler<MonitoringState> OutputMixerChanged;
        public EventHandler<string> LocalMonitorOutputChanged;
        public EventHandler<OutputMix> MonitorSwitchOutputChanged;
        public EventHandler<List<ChannelInfo>> ChannelsChanged;

        #endregion

        public ElgatoWaveClient()
        {
            _port = _startPort;
            _source = new CancellationTokenSource();
        }

        #region Connection
        public async Task<bool> ConnectAsync()
        {
            int cycleCount = 0;
            while (_socket?.State != WebSocketState.Open && (cycleCount < _maxCycles))
            {
                _socket = new ClientWebSocket();
                try
                {
                    await _socket.ConnectAsync(new Uri($"ws://127.0.0.1:{_port}/"), _source.Token).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    _port++;
                    if (_port > (_startPort + _portRange))
                    {
                        _port = _startPort;
                        cycleCount++;
                    }
                }
            }

            if (_socket?.State != WebSocketState.Open)
            {
                throw new Exception($"Looped through possible ports {_maxCycles} times and couldn't connect [{_startPort}-{_startPort + _portRange}]");
            }

            StartReceiver();
            return true;
        }

        public void Disconnect()
        {
            _source.Cancel();
            _source = null;

            _socket.Dispose();
            _socket = null;
        }
        #endregion Connection

        #region Commands

        #region Get Commands

        public Task<ApplicationInfo> GetAppInfo()
        {
            return SendCommand<ApplicationInfo>("getApplicationInfo");
        }

        public Task<List<ChannelInfo>> GetAllChannelInfo()
        {
            return SendCommand<List<ChannelInfo>>("getAllChannelInfo");
        }

        public Task<MicrophoneState> GetMicrophoneState()
        {
            return SendCommand<MicrophoneState>("getMicrophoneState");
        }

        public Task<MicrophoneSettings> GetMicrophoneSettings()
        {
            return SendCommand<MicrophoneSettings>("getMicrophoneSettings");
        }

        public Task<MonitoringState> GetMonitoringState()
        {
            return SendCommand<MonitoringState>("getMonitoringState");
        }

        public Task<MonitorMixOutputList> GetMonitorMixOutputList()
        {
            return SendCommand<MonitorMixOutputList>("getMonitorMixOutputList");
        }

        public Task<SwitchState> GetSwitchState()
        {
            return SendCommand<SwitchState>("getSwitchState");
        }

        #endregion Get Commands

        #region Set Commands

        public Task<MonitorMixOutputList> SetMonitorMixOutput(string mixOutput)
        {
            return SendCommand<MonitorMixOutputList, MonitorMixOutputList>("setMonitorMixOutput", new MonitorMixOutputList()
            {
                MonitorMix = mixOutput
            });
        }

        //TODO Unable to switch to local for some reason, can switch to stream though
        public Task<SwitchState> SetMonitoringState(OutputMix mix)
        {
            return SendCommand<SwitchState, SwitchState>("switchMonitoring", new SwitchState()
            {
                switchState = mix.ToString()
            });
        }

        public Task<MicrophoneSettings> SetMicrophoneSettings(int micGain, int micOutputVol, int micBalcnce, bool isMicLowcutOn, bool isMicClipgaurdOn)
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

        public Task<MicrophoneSettings> SetMicrophoneSettings(MicrophoneSettings settings)
        {
            return SendCommand<MicrophoneSettings, MicrophoneSettings>("setMicrophoneSettings", settings);
        }

        public Task<MonitoringState> SetOutputMixer(MonitoringState state)
        {
            return SendCommand<MonitoringState, MonitoringState>("setOutputMixer", state);
        }

        public Task<MonitoringState> SetOutputMixer(int localVolumeOut, bool isLocalOutMuted, int streamVolumeOut, bool isStreamOutMuted)
        {
            return SetOutputMixer(new MonitoringState()
            {
                IsStreamOutMuted = isStreamOutMuted,
                LocalVolumeOut = localVolumeOut,
                StreamVolumeOut = streamVolumeOut,
                IsLocalOutMuted = isLocalOutMuted
            });
        }

        //TODO Need to look at the JS plugin for more details on this
        //public Task<ChannelInfo> SetInputMixer(string mixId, string name, string color, int inputType, string iconData, int localVol, bool isLocalMuted, int remoteVol, bool isStreamMuted, long deltaLink, bool isAvaible, bool isLinked)
        //{
        //    return SetInputMixer(new ChannelInfo()
        //    {
        //        MixId = mixId,
        //        MixerName = name,
        //        BgColor = color,
        //        InputType = inputType,
        //        IconData = iconData,
        //        LocalVolumeIn = localVol,
        //        IsLocalInMuted = isLocalMuted,
        //        StreamVolumeIn = remoteVol,
        //        IsStreamInMuted = isStreamMuted,
        //        DeltaLinked = deltaLink,
        //        IsAvailable = isAvaible,
        //        IsLinked = isLinked
        //    });
        //}

        //public Task<ChannelInfo> SetInputMixer(ChannelInfo info)
        //{
        //    return SendCommand<ChannelInfo, ChannelInfo>("setInputMixer", info);
        //}

        #endregion

        private Task<T> SendCommand<T>(string method)
        {
            return SendCommand<T, string>(method, null);
        }

        private async Task<T> SendCommand<T, Q>(string method, Q objectJson = default(Q))
        {
            if (_socket?.State == WebSocketState.Open)
            {
                SocketBaseObject<Q> baseObject = new SocketBaseObject<Q>()
                {
                    Method = method,
                    Id = NextTransactionId(),
                    Obj = objectJson
                };
                var s = baseObject.ToString();
                var array = Encoding.UTF8.GetBytes(s);
                await _socket.SendAsync(array, WebSocketMessageType.Text, true, _source.Token);

                SpinWait.SpinUntil(() => _responseCache.ContainsKey(baseObject.Id), TimeSpan.FromMilliseconds(_responseTimeout));

                if (_responseCache.ContainsKey(baseObject.Id))
                {
                    var reply = _responseCache[baseObject.Id];
                    _responseCache.Remove(reply.Id);

                    return JsonConvert.DeserializeObject<T>(reply.Result.ToString());
                }
            }

            return default(T);
        }

        #endregion Commands

        #region Reciever

        private Task _receiveTask;
        private void StartReceiver()
        {
            _receiveTask ??= Task.Run(ReceiverRun, _source.Token);
        }

        private async Task ReceiverRun()
        {
            var buffer = new byte[_bufferSize];
            var offset = 0;
            var free = buffer.Length;

            while (!_source.IsCancellationRequested)
            {
                if (_socket.State == WebSocketState.Open)
                {
                    try
                    {
                        buffer = new byte[_bufferSize];
                        offset = 0;
                        free = buffer.Length;

                        WebSocketReceiveResult result = null;
                        do
                        {
                            result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, free), _source.Token);
                            offset += result.Count;
                            free -= result.Count;
                            if (free == 0)
                            {
                                var newSize = buffer.Length + _bufferSize;
                                if (newSize > _maxBufferSize)
                                {
                                    throw new Exception ("Maximum size exceeded");
                                }
                                var newBuffer = new byte[newSize];
                                Array.Copy(buffer, 0, newBuffer, 0, offset);
                                buffer = newBuffer;
                                free = buffer.Length - offset;
                            }

                        } while (!result?.EndOfMessage ?? false);

                        string json = Encoding.UTF8.GetString(buffer).Replace("\0", "");

                        SocketBaseObject<dynamic> baseObject = JsonConvert.DeserializeObject<SocketBaseObject<dynamic>>(json);
                        if (baseObject?.Id == 0) //Not a command reply
                        {
                            switch (baseObject.Method)
                            {
                                case "microphoneStateChanged":
                                    MicStateChanged?.Invoke(this, JsonConvert.DeserializeObject<MicrophoneState>(baseObject.Obj.ToString()));
                                    break;
                                case "microphoneSettingsChanged":
                                    MicSettingsChanged?.Invoke(this, JsonConvert.DeserializeObject<MicrophoneSettings>(baseObject.Obj.ToString()));
                                    break;
                                case "localMonitorOutputChanged":
                                    LocalMonitorOutputChanged?.Invoke(this, baseObject.Obj.monitorMix.ToString());
                                    break;
                                case "monitorSwitchOutputChanged":
                                    MonitorSwitchOutputChanged?.Invoke(this, baseObject.Obj.switchState.ToString() == "LocalMix" ? OutputMix.LocaLMix : OutputMix.StreamMix);
                                    break;
                                case "channelsChanged":
                                    ChannelsChanged?.Invoke(this, JsonConvert.DeserializeObject<List<ChannelInfo>>(baseObject.Obj.ToString()));
                                    break;
                                case "outputMixerChanged":
                                    OutputMixerChanged?.Invoke(this, JsonConvert.DeserializeObject<MonitoringState>(baseObject.Obj.ToString()));
                                    break;
                                case "inputMixerChanged":
                                    InputMixerChanged?.Invoke(this, JsonConvert.DeserializeObject<ChannelInfo>(baseObject.Obj.ToString()));
                                    break;
                                default: //Ignore
                                    break;

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

                    }

                }
            }
        }
        #endregion

        #region TransationTracker

        private Dictionary<int, SocketBaseObject<dynamic>> _responseCache = new();
        private int _transactionId { get; set; } = 1;

        private int NextTransactionId()
        {
            return _transactionId++;
        }
        #endregion TransationTracker

    }
}
