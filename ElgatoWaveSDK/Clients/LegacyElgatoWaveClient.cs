//using System;
//using System.Collections.Generic;
//using System.Net.WebSockets;
//using System.Runtime.CompilerServices;
//using System.Text.Json;
//using System.Threading.Tasks;
//using ElgatoWaveSDK.HumbleObjects;
//using ElgatoWaveSDK.Models;
//using ElgatoWaveSDK.Models.Old;

//namespace ElgatoWaveSDK.Clients
//{
//    public class LegacyElgatoWaveClient : BaseClient, IBaseClient
//    {
//        #region Public Events

//        public event EventHandler<MicrophoneState>? MicStateChanged;
//        public event EventHandler<MicrophoneSettings>? MicSettingsChanged;
//        public event EventHandler<ChannelInfo>? InputMixerChanged;
//        public event EventHandler<MonitoringState>? OutputMixerChanged;
//        public event EventHandler<string>? LocalMonitorOutputChanged;
//        public event EventHandler<MixType>? MonitorSwitchOutputChanged;
//        public event EventHandler<List<ChannelInfo>>? ChannelsChanged;

//        public event EventHandler<ElgatoException>? ExceptionOccurred;

//        #endregion

//        public LegacyElgatoWaveClient(ClientConfig? config = null)
//        {
//            BaseExceptionOccurred += (sender, exception) =>
//            {
//                ExceptionOccurred?.Invoke(this, exception);
//            };

//            EventReceived += OnEventReceived;

//            Config = config;

//            Init();
//        }

//        internal LegacyElgatoWaveClient(IHumbleClientWebSocket socket, IReceiverUtils receiver, ITransactionTracker transactionTracker) : this(new ClientConfig())
//        {
//            _socket = socket;
//            _receiver = receiver;
//            _transactionTracker = transactionTracker;
//        }

//        #region Commands

//        #region Get Commands

//        public Task<ApplicationInfo?> GetAppInfo()
//        {
//            return SendCommand<ApplicationInfo>("getApplicationInfo");
//        }

//        public Task<List<ChannelInfo>?> GetAllChannelInfo()
//        {
//            return SendCommand<List<ChannelInfo>>("getAllChannelInfo");
//        }

//        public Task<MicrophoneState?> GetMicrophoneState()
//        {
//            return SendCommand<MicrophoneState>("getMicrophoneState");
//        }

//        public Task<MicrophoneSettings?> GetMicrophoneSettings()
//        {
//            return SendCommand<MicrophoneSettings>("getMicrophoneSettings");
//        }

//        public Task<MonitoringState?> GetMonitoringState()
//        {
//            return SendCommand<MonitoringState>("getMonitoringState");
//        }

//        public Task<MonitorMixOutputList?> GetMonitorMixOutputList()
//        {
//            return SendCommand<MonitorMixOutputList>("getMonitorMixOutputList");
//        }

//        public async Task<MixType?> GetSwitchState()
//        {
//            var reply = await SendCommand<SwitchState>("getSwitchState").ConfigureAwait(false);
//            if (string.IsNullOrEmpty(reply?.CurrentState))
//            {
//                return null;
//            }
//            return reply?.CurrentState == MixType.LocalMix.ToString() ? MixType.LocalMix : MixType.StreamMix;
//        }

//        #endregion Get Commands

//        #region Set Commands

//        public Task<MonitorMixOutputList?> SetMonitorMixOutput(string mixOutput)
//        {
//            return SendCommand<MonitorMixOutputList, MonitorMixOutputList>("setMonitorMixOutput", new MonitorMixOutputList()
//            {
//                MonitorMix = mixOutput
//            });
//        }

//        public async Task<MixType?> SetMonitoringState(MixType mix)
//        {
//            var reply = await SendCommand<SwitchState, SwitchState>("switchMonitoring", new SwitchState()
//            {
//                CurrentState = mix.ToString()
//            });

//            if (string.IsNullOrEmpty(reply?.CurrentState))
//            {
//                return null;
//            }

//            var newValue = reply?.CurrentState == MixType.LocalMix.ToString() ? MixType.LocalMix : MixType.StreamMix;
//            MonitorSwitchOutputChanged?.Invoke(this, newValue);
//            return newValue;
//        }

//        public Task<MicrophoneSettings?> SetMicrophoneSettings(int micGain, int micOutputVol, int micBalcnce, bool isMicLowcutOn, bool isMicClipgaurdOn)
//        {
//            return SetMicrophoneSettings(new MicrophoneSettings()
//            {
//                MicrophoneGain = micGain,
//                MicrophoneOutputVolume = micOutputVol,
//                MicrophoneBalance = micBalcnce,
//                IsMicrophoneLowcutOn = isMicLowcutOn,
//                IsMicrophoneClipguardOn = isMicClipgaurdOn,
//            });
//        }

//        public Task<MicrophoneSettings?> SetMicrophoneSettings(MicrophoneSettings settings)
//        {
//            return SendCommand<MicrophoneSettings, MicrophoneSettings>("setMicrophoneSettings", settings);
//        }

//        public Task<MonitoringState?> SetOutputMixer(MonitoringState state)
//        {
//            return SendCommand<MonitoringState, MonitoringState>("setOutputMixer", state);
//        }

//        public Task<MonitoringState?> SetOutputMixer(int localVolumeOut, bool isLocalOutMuted, int streamVolumeOut, bool isStreamOutMuted)
//        {
//            return SetOutputMixer(new MonitoringState()
//            {
//                IsStreamOutMuted = isStreamOutMuted,
//                LocalVolumeOut = localVolumeOut,
//                StreamVolumeOut = streamVolumeOut,
//                IsLocalOutMuted = isLocalOutMuted
//            });
//        }

//        public Task<ChannelInfo?> SetInputMixer(string mixId, int localVol, bool isLocalMuted, int remoteVol, bool isStreamMuted, List<Filter> filters, bool localByPass, bool streamByPass, MixType mixType)
//        {
//            return SetInputMixer(new ChannelInfo()
//            {
//                MixId = mixId,
//                LocalVolumeIn = localVol,
//                IsLocalInMuted = isLocalMuted,
//                StreamVolumeIn = remoteVol,
//                IsStreamInMuted = isStreamMuted,
//                Filters = filters,
//                LocalMixFilterBypass = localByPass,
//                StreamMixFilterBypass = streamByPass
//            }, mixType);
//        }

//        public Task<ChannelInfo?> SetInputMixer(ChannelInfo info, MixType mixType)
//        {
//            info.BgColor = null;
//            info.IconData = null;
//            info.InputType = null;
//            info.IsAvailable = null;
//            info.MixerName = null;
//            info.Slider = mixType == MixType.LocalMix ? "local" : "stream";

//            return SendCommand<ChannelInfo, ChannelInfo>("setInputMixer", info);
//        }

//        #endregion

//        #endregion Commands

//        #region RecieverEvents

//        private void OnEventReceived(object sender, EventReceivedArgs e)
//        {
//            object? obj = null;
//            switch (e.EventName)
//            {
//                case "microphoneStateChanged":
//                    obj = JsonSerializer.Deserialize<MicrophoneState>(e.Obj?.ToString() ?? "{}");
//                    if (obj != null)
//                    {
//                        MicStateChanged?.Invoke(this, obj as MicrophoneState ?? throw new InvalidOperationException());
//                    }
//                    break;
//                case "microphoneSettingsChanged":
//                    obj = JsonSerializer.Deserialize<MicrophoneSettings>(e.Obj?.ToString() ?? "{}");
//                    if (obj != null)
//                    {
//                        MicSettingsChanged?.Invoke(this, obj as MicrophoneSettings ?? throw new InvalidOperationException());
//                    }
//                    break;
//                case "localMonitorOutputChanged":
//                    obj = e.Obj?["monitorMix"]?.ToString();
//                    if (obj != null)
//                    {
//                        LocalMonitorOutputChanged?.Invoke(this, obj as string ?? throw new InvalidOperationException());
//                    }
//                    break;
//                case "monitorSwitchOutputChanged":
//                    obj = e.Obj?["switchState"]?.ToString();
//                    if (obj != null)
//                    {
//                        MonitorSwitchOutputChanged?.Invoke(this, obj?.ToString() == "LocalMix" ? MixType.LocalMix : MixType.StreamMix);
//                    }
//                    break;
//                case "channelsChanged":
//                    obj = JsonSerializer.Deserialize<List<ChannelInfo>>(e.Obj?["channels"]?.ToString() ?? "{}");
//                    if (obj != null)
//                    {
//                        ChannelsChanged?.Invoke(this, obj as List<ChannelInfo> ?? throw new InvalidOperationException());
//                    }
//                    break;
//                case "outputMixerChanged":
//                    obj = JsonSerializer.Deserialize<MonitoringState>(e.Obj?.ToString() ?? "{}");
//                    if (obj != null)
//                    {
//                        OutputMixerChanged?.Invoke(this, obj as MonitoringState ?? throw new InvalidOperationException());
//                    }
//                    break;
//                case "inputMixerChanged":
//                    obj = JsonSerializer.Deserialize<ChannelInfo>(e.Obj?.ToString() ?? "{}");
//                    if (obj != null)
//                    {
//                        InputMixerChanged?.Invoke(this, obj as ChannelInfo ?? throw new InvalidOperationException());
//                    }
//                    break;
//                default:
//                    throw new ElgatoException($"Unsupported method received | {e.EventName}", _socket?.State ?? WebSocketState.None);

//            }
//        }
//        #endregion
//    }
//}
