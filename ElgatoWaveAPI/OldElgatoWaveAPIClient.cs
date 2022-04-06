//using ElgatoWaveAPI.Models;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Net.WebSockets;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices.WindowsRuntime;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Websocket.Client;

//namespace ElgatoWaveAPI
//{
//    public class ElgatoWaveAPIClient
//    {

//        #region Public Events

//        public EventHandler<MicrophoneState> MicStateChanged;
//        public EventHandler<MicrophoneSettings> MicSettingsChanged;
//        public EventHandler<ChannelInfo> InputMixerChanged;
//        public EventHandler<MonitoringState> OutputMixerChanged;
//        public EventHandler<string> LocalMonitorOutputChanged;
//        public EventHandler<OutputMix> MonitorSwitchOutputChanged;
//        public EventHandler<List<ChannelInfo>> ChannelsChanged;

//        #endregion Public Events

//        #region private values

//        private CancellationTokenSource _cancelToken { get; set; }
//        private WebsocketClient _socket { get; set; }

//        private string _ip { get; set; }
//        private int _port { get; set; }

//        private const int _startPort = 1824;
//        private const int _endPort = 1834;

//        #endregion private values

//        public ElgatoWaveAPIClient() : this("localhost")
//        {

//        }

//        public ElgatoWaveAPIClient(string ip)
//        {
//            _ip = ip;
//            _port = _startPort;

//            _cancelToken = new CancellationTokenSource();
//        }

//        #region Connection

//        public async Task<bool> ConnectAsync()
//        {

//            _socket = new WebsocketClient(new Uri($"ws://{_ip}:{_port}/"))
//            {
//                ErrorReconnectTimeout = TimeSpan.FromSeconds(30),
//                ReconnectTimeout = TimeSpan.FromSeconds(30),
//                IsReconnectionEnabled = true,
//                MessageEncoding = Encoding.UTF8,
//                IsTextMessageConversionEnabled = true,
//                Name = "WaveLinkDotNetClient",
//            };

//            _socket.MessageReceived.Subscribe(OnNext);

//            try
//            {
//                await _socket.StartOrFail();
//            }
//            catch (Exception ex)
//            {
//                _port++;
//                if (_port > _endPort)
//                {
//                    _port = _startPort;
//                }
//            }

//            _rpc.UnknownReply += (sender, o) =>
//             {

//                if(!string.IsNullOrEmpty(o?.bgColor?.ToString()))
//                {
//                    ChannelInfo obj = JsonConvert.DeserializeObject(o.ToString(), typeof(ChannelInfo)) as ChannelInfo;
//                    InputMixerChanged?.Invoke(this, obj);
//                }
//                else if(!string.IsNullOrEmpty(o?.monitorMix?.ToString()))
//                {
//                    LocalMonitorOutputChanged?.Invoke(this, o?.monitorMix?.ToString());
//                }
//                else if(!string.IsNullOrEmpty(o?.isLocalOutMuted?.ToString()))
//                {
//                    Wave_OutputMixerChanged((int)o.localVolumeOut, (int)o.streamVolumeOut, (bool)o.isLocalOutMuted, (bool)o.isStreamOutMuted);
//                }
//                else if(!string.IsNullOrEmpty(o?.switchState?.ToString()))
//                {
//                    Wave_MonitorSwitchOutputChanged(o?.switchState?.ToString());
//                }
//                else if (!string.IsNullOrEmpty(o?.isMicrophoneClipguardOn.ToString()))
//                {
//                    Wave_MicSettingsChanged((int)o?.microphoneGain, (int)o?.microphoneOutputVolume, (int)o?.microphoneBalance, (bool)o?.isMicrophoneLowcutOn, (bool)o?.isMicrophoneClipguardOn);
//                }
//             };

//            _rpc.AddLocalRpcMethod("microphoneStateChanged", new Action<bool>(Wave_MicStateChanged));
//            _rpc.AddLocalRpcMethod("microphoneSettingsChanged", new Action<int, int, int, bool, bool>(Wave_MicSettingsChanged));

//            _rpc.AddLocalRpcMethod("localMonitorOutputChanged", new Action<string>(Wave_LocalMonitorOutputChanged));
//            _rpc.AddLocalRpcMethod("monitorSwitchOutputChanged", new Action<string>(Wave_MonitorSwitchOutputChanged));

//            _rpc.AddLocalRpcMethod("channelsChanged", new Action<List<ChannelInfo>>(Wave_ChannelsChanged));

//            _rpc.AddLocalRpcMethod("outputMixerChanged", new Action<int, int, bool, bool>(Wave_OutputMixerChanged));
//            _rpc.AddLocalRpcMethod("inputMixerChanged", new Action<string, int, dynamic[], string, string, bool, bool, bool, bool, bool, int, string, string, bool, int>((bgColor, deltaLinked, filters, iconData, inputType, isAvailable, isLinked, isLocalInMuted, isStreamInMuted, localMixFilterBypass, localVolumeIn, mixId, mixerName, streamMixFilterBypass, streamVolumeIn) =>
//            {
//                //filter object: active - bool,  filterID - Guid, name - string, pluginID - string
//                InputMixerChanged?.Invoke(this, new ChannelInfo()
//                {
//                    BgColor = bgColor,
//                    DeltaLinked = deltaLinked,
//                    IsAvailable = isAvailable,
//                    MixId = mixId,
//                    StreamVolumeIn = streamVolumeIn,
//                    MixerName = mixerName,
//                    IsLinked = isLinked,
//                    IsLocalInMuted = isLocalInMuted,
//                    IsStreamInMuted = isStreamInMuted,
//                    LocalVolumeIn = localVolumeIn,
//                });
//            }));


//            var appInfo = await this.GetAppInfo();
//            if(appInfo.Version < new Version(1,4,0,0) )
//            {
//                throw new Exception($"Unsupported WaveLink version | Expected v1.4.0 or higher but found: {appInfo.Version}");
//            }

//            return true;
//        }

//        private void OnNext(ResponseMessage e)
//        {
//            if (JsonConvert.DeserializeObject(e.Text, typeof(SocketBaseObject<string>)) is SocketBaseObject<string> baseObject)
//            {
//                switch (baseObject.Method)
//                {
//                    case "outputMixer":
//                        if (JsonConvert.DeserializeObject(baseObject.Obj, typeof(MonitoringState)) is MonitoringState obj)
//                        {
//                            this.Wave_OutputMixerChanged(obj);
//                        }
//                        break;
//                }
//            }
//        }

//        public bool Disconnect()
//        {
//            _cancelToken?.Cancel();

//            _rpc?.Dispose();
//            _rpc = null;

//            _rpcWebSocket?.Dispose();
//            _rpcWebSocket = null;

//            _cancelToken?.Dispose();
//            _cancelToken = null;

//            return true;
//        }

//        #endregion Connection

//        #region Events

//        private void Wave_MicStateChanged(bool state)
//        {
//            MicStateChanged?.Invoke(this, new MicrophoneState()
//            {
//                IsMicrophoneConnected = state
//            });
//        }

//        private void Wave_MicSettingsChanged(int microphoneGain, int microphoneOutputVolume, int microphoneBalance, bool isMicrophoneLowcutOn, bool isMicrophoneClipguardOn)
//        {
//            MicSettingsChanged?.Invoke(this, new MicrophoneSettings()
//            {
//                IsMicrophoneClipguardOn = isMicrophoneClipguardOn,
//                IsMicrophoneLowcutOn = isMicrophoneLowcutOn,
//                MicrophoneBalance = microphoneBalance,
//                MicrophoneGain = microphoneGain,
//                MicrophoneOutputVolume = microphoneOutputVolume,
//            });
//        }

//        private void Wave_OutputMixerChanged(int localVolumeOut, int streamVolumeOut, bool isLocalOutMuted, bool isStreamOutMuted)
//        {
//            OutputMixerChanged?.Invoke(this, new MonitoringState()
//            {
//                IsLocalOutMuted = isLocalOutMuted,
//                IsStreamOutMuted = isStreamOutMuted,
//                LocalVolumeOut = localVolumeOut,
//                StreamVolumeOut = streamVolumeOut,
//            });
//        }

//        private void Wave_OutputMixerChanged(MonitoringState obj)
//        {
//            OutputMixerChanged?.Invoke(this, obj);
//        }

//        private void Wave_LocalMonitorOutputChanged(string monitorMix)
//        {
//            LocalMonitorOutputChanged?.Invoke(this, monitorMix);
//        }

//        private void Wave_MonitorSwitchOutputChanged(string switchState)
//        {
//            MonitorSwitchOutputChanged?.Invoke(this, (OutputMix) Enum.Parse(typeof(OutputMix), switchState));
//        }

//        private void Wave_ChannelsChanged(List<ChannelInfo> channels)
//        {
//            ChannelsChanged?.Invoke(this, channels);
//        }

//        #endregion Events

//        #region Set Commands
//        //TODO The command don't seem to work atm, not sure why
//        public async Task<string> SetMonitorMixOutput(string mixOutput)
//        {
//            JObject reply = await _rpc.InvokeAsync<JObject>("setMonitorMixOutput", new { monitorMix = mixOutput });

//            return reply?.First?.First?.ToString();
//        }

//        public async Task<OutputMix?> SwitchMonitoringState(OutputMix state)
//        {
//            JObject response = await _rpc.InvokeAsync<JObject>("switchMonitoring", new { switchState = "LocalMix" });

//            var responseValue = response?.First?.First?.ToString()?.ToLower();
//            if (string.IsNullOrEmpty(responseValue))
//            {
//                return null;
//            }

//            return responseValue.Equals(OutputMix.StreamMix.ToString(), StringComparison.InvariantCultureIgnoreCase) ? OutputMix.StreamMix : OutputMix.LocaLMix;
//        }

//        public async Task<MicrophoneSettings> SetMicSettings(int micGain, int micOutputVol, int micBalcnce, bool isMicLowcutOn, bool isMicClipgaurdOn)
//        {
//            var reply = await _rpc.InvokeAsync<MicrophoneSettings>("setMicrophoneSettings", new
//            {
//                microphoneGain = micGain,
//                microphoneOutputVolume = micOutputVol,
//                microphoneBalance = micBalcnce,
//                isMicrophoneLowcutOn = isMicLowcutOn,
//                isMicrophoneClipguardOn = isMicClipgaurdOn,
//            });

//            return reply;
//        }

//        public async Task<MicrophoneSettings> SetMicSettings(MicrophoneSettings newSettings)
//        {
//            return await SetMicSettings(newSettings.MicrophoneGain, newSettings.MicrophoneOutputVolume, newSettings.MicrophoneBalance, newSettings.IsMicrophoneLowcutOn, newSettings.IsMicrophoneClipguardOn);
//        }

//        public async Task<JObject> SetInputMixer(string mixId, int slider, bool isLinked, int localVolumeIn, bool isLocalInMuted, int streamVolumeIn, bool isStreamInMuted)
//        {
//            JObject reply = await _rpc.InvokeAsync<JObject>("setInputMixer", new
//            {
//                mixId,
//                slider,
//                isLinked,
//                localVolumeIn,
//                isLocalInMuted,
//                streamVolumeIn,
//                isStreamInMuted,
//            });

//            return reply;
//        }

//        public async Task<string> SetOutputMixer(int localVolumeOut, bool isLocalOutMuted, int streamVolumeOut, bool isStreamOutMuted)
//        {
//            var reply = await _rpc.InvokeAsync<dynamic>("setOutputMixer", new
//            {
//                localVolumeOut,
//                isLocalOutMuted = isLocalOutMuted.ToString().ToLower(),
//                streamVolumeOut,
//                isStreamOutMuted = isStreamOutMuted.ToString().ToLower()
//            });
//            return "";
//        }

//        #endregion Set Commands

//        #region Get Commands

//        public async Task<ApplicationInfo> GetAppInfo()
//        {
//            var reply = await _rpc.InvokeAsync<ApplicationInfo>("getApplicationInfo");
//            return reply;
//        }

//        public async Task<List<ChannelInfo>> GetAllChannelInfo()
//        {
//            var reply = await _rpc.InvokeAsync<List<ChannelInfo>>("getAllChannelInfo");
//            return reply;
//        }

//        public async Task<MicrophoneState> GetMicrophoneState()
//        {
//            var reply = await _rpc.InvokeAsync<MicrophoneState>("getMicrophoneState");
//            return reply;
//        }

//        public async Task<MicrophoneSettings> GetMicrophoneSettings()
//        {
//            var reply = await _rpc.InvokeAsync<MicrophoneSettings>("getMicrophoneSettings");
//            return reply;
//        }

//        public async Task<MonitoringState> GetMonitoringState()
//        {
//            var reply = await _rpc.InvokeAsync<MonitoringState>("getMonitoringState");
//            return reply;
//        }

//        public async Task<MonitorMixOutputList> GetMonitorMixOutputList()
//        {
//            var reply = await _rpc.InvokeAsync<MonitorMixOutputList>("getMonitorMixOutputList");
//            return reply;
//        }

//        public async Task<SwitchState> GetSwitchState()
//        {
//            var reply = await _rpc.InvokeAsync<SwitchState>("getSwitchState");
//            return reply;
//        }

//        #endregion Get Commands
//    }
//}