﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElgatoWaveSDK.Models;
using Newtonsoft.Json;

namespace ElgatoWaveSDK
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
        private ClientWebSocket? _socket;
        private int Port { get; set; }
        private CancellationTokenSource? _source;
        #endregion

        #region Public Vars

        public bool IsConnected => _socket?.State == WebSocketState.Open;

        #endregion

        #region Public Events

        public EventHandler<MicrophoneState>? MicStateChanged { get; set; }
        public EventHandler<MicrophoneSettings>? MicSettingsChanged { get; set; }
        public EventHandler<ChannelInfo>? InputMixerChanged { get; set; }
        public EventHandler<MonitoringState>? OutputMixerChanged { get; set; }
        public EventHandler<string>? LocalMonitorOutputChanged { get; set; }
        public EventHandler<OutputMix>? MonitorSwitchOutputChanged { get; set; }
        public EventHandler<List<ChannelInfo>>? ChannelsChanged { get; set; }

        #endregion

        public ElgatoWaveClient()
        {
            Port = _startPort;
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
                    await _socket.ConnectAsync(new Uri($"ws://127.0.0.1:{Port}/"), _source?.Token ?? CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    Port++;
                    if (Port > (_startPort + _portRange))
                    {
                        Port = _startPort;
                        cycleCount++;
                    }
                }
            }

            if (_socket?.State != WebSocketState.Open)
            {
                throw new ElgatoException($"Looped through possible ports {_maxCycles} times and couldn't connect [{_startPort}-{_startPort + _portRange}]");
            }

            StartReceiver();
            return true;
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

        public Task<SwitchState?> SetMonitoringState(OutputMix mix)
        {
            return SendCommand<SwitchState, SwitchState>("switchMonitoring", new SwitchState()
            {
                switchState = mix.ToString()
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

        private Task<T?> SendCommand<T>(string method)
        {
            return SendCommand<T, string>(method, null);
        }

        private async Task<T?> SendCommand<T, Q>(string method, Q? objectJson = default)
        {
            if (_socket?.State == WebSocketState.Open)
            {
                SocketBaseObject<Q?> baseObject = new()
                {
                    Method = method,
                    Id = NextTransactionId(),
                    Obj = objectJson
                };
                var s = baseObject.ToJson();
                var array = Encoding.UTF8.GetBytes(s);
                await _socket.SendAsync(array, WebSocketMessageType.Text, true, _source?.Token ?? CancellationToken.None).ConfigureAwait(false);

                SpinWait.SpinUntil(() => _responseCache.ContainsKey(baseObject.Id), TimeSpan.FromMilliseconds(_responseTimeout));

                if (_responseCache.ContainsKey(baseObject.Id))
                {
                    var reply = _responseCache[baseObject.Id];
                    _responseCache.Remove(reply.Id);

                    if (reply.Result != null)
                    {
                        return JsonConvert.DeserializeObject<T>(reply.Result.ToString());
                    }
                }
            }

            return default;
        }

        #endregion Commands

        #region Reciever

        private Task? _receiveTask;
        private void StartReceiver()
        {
            _receiveTask ??= Task.Run(ReceiverRun, _source?.Token ?? CancellationToken.None);
        }

        private async Task ReceiverRun()
        {
            while (!_source?.IsCancellationRequested ?? false)
            {
                if (_socket?.State == WebSocketState.Open)
                {
                    try
                    {
                        var buffer = new byte[_bufferSize];
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
                                var newSize = buffer.Length + _bufferSize;
                                if (newSize > _maxBufferSize)
                                {
                                    throw new ElgatoException("Maximum size exceeded");
                                }
                                var newBuffer = new byte[newSize];
                                Array.Copy(buffer, 0, newBuffer, 0, offset);
                                buffer = newBuffer;
                                free = buffer.Length - offset;
                            }

                        } while (!result?.EndOfMessage ?? false);

                        string json = Encoding.UTF8.GetString(buffer).Replace("\0", "");

                        SocketBaseObject<dynamic?>? baseObject = JsonConvert.DeserializeObject<SocketBaseObject<dynamic?>?>(json);
                        if (baseObject == null)
                        {
                            continue;
                        }

                        if (baseObject.Id == 0) //Not a command reply
                        {
                            object? obj = null;
                            switch (baseObject.Method)
                            {
                                case "microphoneStateChanged":
                                    obj = JsonConvert.DeserializeObject<MicrophoneState>(baseObject.Obj?.ToString());
                                    if (obj != null)
                                    {
                                        MicStateChanged?.Invoke(this, obj as MicrophoneState ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "microphoneSettingsChanged":
                                    obj = JsonConvert.DeserializeObject<MicrophoneSettings>(baseObject.Obj?.ToString());
                                    if (obj != null)
                                    {
                                        MicSettingsChanged?.Invoke(this, obj as MicrophoneSettings ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "localMonitorOutputChanged":
                                    obj = baseObject.Obj?.monitorMix?.ToString();
                                    if (obj != null)
                                    {
                                        LocalMonitorOutputChanged?.Invoke(this, obj as string ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "monitorSwitchOutputChanged":
                                    obj = baseObject.Obj?.switchState?.ToString();
                                    if (obj != null)
                                    {
                                        MonitorSwitchOutputChanged?.Invoke(this, obj?.ToString() == "LocalMix" ? OutputMix.LocalMix : OutputMix.StreamMix);
                                    }
                                    break;
                                case "channelsChanged":
                                    obj = JsonConvert.DeserializeObject<List<ChannelInfo>>(baseObject.Obj?.ToString());
                                    if (obj != null)
                                    {
                                        ChannelsChanged?.Invoke(this, obj as List<ChannelInfo> ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "outputMixerChanged":
                                    obj = JsonConvert.DeserializeObject<MonitoringState>(baseObject.Obj?.ToString());
                                    if (obj != null)
                                    {
                                        OutputMixerChanged?.Invoke(this, obj as MonitoringState ?? throw new InvalidOperationException());
                                    }
                                    break;
                                case "inputMixerChanged":
                                    obj = JsonConvert.DeserializeObject<ChannelInfo>(baseObject.Obj?.ToString());
                                    if (obj != null)
                                    {
                                        InputMixerChanged?.Invoke(this, obj as ChannelInfo ?? throw new InvalidOperationException());
                                    }
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
                        throw new ElgatoException("Unknown error in receiving task", ex);
                    }

                }
            }
        }
        #endregion

        #region TransationTracker

        private readonly Dictionary<int, SocketBaseObject<dynamic?>> _responseCache = new();
        private int TransactionId { get; set; } = 1;

        private int NextTransactionId()
        {
            return TransactionId++;
        }
        #endregion TransationTracker

    }
}
