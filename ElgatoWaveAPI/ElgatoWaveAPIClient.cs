using ElgatoWaveAPI.Models;
using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ElgatoWaveAPI
{
    public class ElgatoWaveAPIClient
    {
        #region public values

        public bool IsConnection => !_rpc.IsDisposed && _rpcWebSocket.State == WebSocketState.Open;

        #endregion public values

        #region Public Events

        public EventHandler<MicrophoneState> MicStateChanged;
        public EventHandler<MicrophoneSettings> MicSettingsChanged;
        public EventHandler<ChannelInfo> InputMixerChanged;
        public EventHandler<MonitoringState> OutputMixerChanged;
        public EventHandler<string> LocalMonitorOutputChanged;
        public EventHandler<OutputMix> MonitorSwitchOutputChanged;
        public EventHandler<List<ChannelInfo>> ChannelsChanged;

        #endregion Public Events

        #region private values

        private CancellationTokenSource _cancelToken { get; set; }
        private JsonRpc _rpc { get; set; }
        private ClientWebSocket _rpcWebSocket { get; set; }

        private string _url { get; set; }

        #endregion private values

        public ElgatoWaveAPIClient(string ip, int port)
        {
            _url = $"ws://{ip}:{port}/";

            _cancelToken = new CancellationTokenSource();

            _rpcWebSocket = new ClientWebSocket();
        }

        #region Connection

        public async Task<bool> ConnectAsync()
        {
            if (_rpcWebSocket.State != WebSocketState.Open)
            {
                await _rpcWebSocket.ConnectAsync(new Uri(_url), _cancelToken.Token);
            }

            _rpc = new JsonRpc(new WebSocketMessageHandler(_rpcWebSocket));
            _rpc.AddLocalRpcMethod("microphoneStateChanged", new Action<bool>(Wave_MicStateChanged));
            _rpc.AddLocalRpcMethod("microphoneSettingsChanged", new Action<int, int, int, bool, bool>(Wave_MicSettingsChanged));

            _rpc.AddLocalRpcMethod("localMonitorOutputChanged", new Action<string>(Wave_LocalMonitorOutputChanged));
            _rpc.AddLocalRpcMethod("monitorSwitchOutputChanged", new Action<string>(Wave_MonitorSwitchOutputChanged));

            _rpc.AddLocalRpcMethod("channelsChanged", new Action<List<ChannelInfo>>(Wave_ChannelsChanged));

            _rpc.AddLocalRpcMethod("outputMixerChanged", new Action<int, int, bool, bool>(Wave_OutputMixerChanged));
            _rpc.AddLocalRpcMethod("inputMixerChanged",
                new Action<string, string, string, bool, int, int, int, bool, bool, bool>
                ((mixerName, mixId, bgColor, isLinked, deltaLinked, localVolumeIn, streamVolumeIn, isLocalInMuted, isStreamInMuted, isAvailable) =>
                {
                    InputMixerChanged?.Invoke(this, new ChannelInfo()
                    {
                        BgColor = bgColor,
                        DeltaLinked = deltaLinked,
                        IsAvailable = isAvailable,
                        MixId = mixId,
                        StreamVolumeIn = streamVolumeIn,
                        MixerName = mixerName,
                        IsLinked = isLinked,
                        IsLocalInMuted = isLocalInMuted,
                        IsStreamInMuted = isStreamInMuted,
                        LocalVolumeIn = localVolumeIn,
                    });
                }));

            _rpc.StartListening();

            return true;
        }

        public bool Disconnect()
        {
            _cancelToken?.Cancel();

            _rpc?.Dispose();
            _rpc = null;

            _rpcWebSocket?.Dispose();
            _rpcWebSocket = null;

            _cancelToken?.Dispose();
            _cancelToken = null;

            return true;
        }

        #endregion Connection

        #region Events

        private void Wave_MicStateChanged(bool state)
        {
            MicStateChanged?.Invoke(this, new MicrophoneState()
            {
                IsMicrophoneConnected = state
            });
        }

        private void Wave_MicSettingsChanged(int microphoneGain, int microphoneOutputVolume, int microphoneBalance, bool isMicrophoneLowcutOn, bool isMicrophoneClipguardOn)
        {
            MicSettingsChanged?.Invoke(this, new MicrophoneSettings()
            {
                IsMicrophoneClipguardOn = isMicrophoneClipguardOn,
                IsMicrophoneLowcutOn = isMicrophoneLowcutOn,
                MicrophoneBalance = microphoneBalance,
                MicrophoneGain = microphoneGain,
                MicrophoneOutputVolume = microphoneOutputVolume,
            });
        }

        private void Wave_OutputMixerChanged(int localVolumeOut, int streamVolumeOut, bool isLocalOutMuted, bool isStreamOutMuted)
        {
            OutputMixerChanged?.Invoke(this, new MonitoringState()
            {
                IsLocalOutMuted = isLocalOutMuted,
                IsStreamOutMuted = isStreamOutMuted,
                LocalVolumeOut = localVolumeOut,
                StreamVolumeOut = streamVolumeOut,
            });
        }

        private void Wave_LocalMonitorOutputChanged(string monitorMix)
        {
            LocalMonitorOutputChanged?.Invoke(this, monitorMix);
        }

        private void Wave_MonitorSwitchOutputChanged(string switchState)
        {
            MonitorSwitchOutputChanged?.Invoke(this, (OutputMix) Enum.Parse(typeof(OutputMix), switchState));
        }

        private void Wave_ChannelsChanged(List<ChannelInfo> channels)
        {
            ChannelsChanged?.Invoke(this, channels);
        }

        #endregion Events

        #region Set Commands

        public async void SetMonitorMixOutput(int mixOutput)
        {
            var reply = await _rpc.InvokeAsync<dynamic>("setMonitorMixOutput", new { monitorMix = mixOutput });
        }

        public async void SwitchMonitoringState(string state)
        {
            var reply = await _rpc.InvokeAsync<dynamic>("switchMonitoring", new { switchState = state });
        }

        public async void SetMicSettings(int micGain, int micOutputVol, int micBalcnce, bool isMicLowcutOn, bool isMicClipgaurdOn)
        {
            var reply = await _rpc.InvokeAsync<dynamic>("setMicrophoneSettings", new
            {
                microphoneGain = micGain,
                microphoneOutputVolume = micOutputVol,
                microphoneBalance = micBalcnce,
                isMicrophoneLowcutOn = isMicLowcutOn,
                isMicrophoneClipguardOn = isMicClipgaurdOn,
            });
        }

        public async void SetInputMixer(string mixID, int slider, bool isLinked, int localVolIn, bool isLocalInMuted, int streamVolIn, bool isStreamInMuted)
        {
            var reply = await _rpc.InvokeAsync<dynamic>("setInputMixer", new
            {
                mixId = mixID,
                slider = slider,
                isLinked = isLinked,
                localVolumeIn = localVolIn,
                isLocalInMuted = isLocalInMuted,
                streamVolumeIn = streamVolIn,
                isStreamInMuted = isStreamInMuted,
            });
        }

        public async void SetOutputMixer(int localVolOut, bool isLocalOutMuted, int streamVolOut, bool isStreamOutMuted)
        {
            var reply = await _rpc.InvokeAsync<dynamic>("setOutputMixer", new
            {
                localVolumeOut = localVolOut,
                isLocalOutMuted = isLocalOutMuted,
                streamVolumeOut = streamVolOut,
                isStreamOutMuted = isStreamOutMuted,
            });
        }

        #endregion Set Commands

        #region Get Commands

        public async Task<ApplicationInfo> GetAppInfo()
        {
            var reply = await _rpc.InvokeAsync<ApplicationInfo>("getApplicationInfo");
            return reply;
        }

        public async Task<List<ChannelInfo>> GetAllChannelInfo()
        {
            var reply = await _rpc.InvokeAsync<List<ChannelInfo>>("getAllChannelInfo");
            return reply;
        }

        public async Task<MicrophoneState> GetMicrophoneState()
        {
            var reply = await _rpc.InvokeAsync<MicrophoneState>("getMicrophoneState");
            return reply;
        }

        public async Task<MicrophoneSettings> GetMicrophoneSettings()
        {
            var reply = await _rpc.InvokeAsync<MicrophoneSettings>("getMicrophoneSettings");
            return reply;
        }

        public async Task<MonitoringState> GetMonitoringState()
        {
            var reply = await _rpc.InvokeAsync<MonitoringState>("getMonitoringState");
            return reply;
        }

        public async Task<MonitorMixOutputList> GetMonitorMixOutputList()
        {
            var reply = await _rpc.InvokeAsync<MonitorMixOutputList>("getMonitorMixOutputList");
            return reply;
        }

        public async Task<SwitchState> GetSwitchState()
        {
            var reply = await _rpc.InvokeAsync<SwitchState>("getSwitchState");
            return reply;
        }

        #endregion Get Commands
    }
}