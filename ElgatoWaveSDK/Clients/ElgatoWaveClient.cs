using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ElgatoWaveSDK.Models;
using System.Text.Json;
using System.Threading.Tasks;
using ElgatoWaveSDK.Models._1._6;
using ElgatoWaveSDK.Models._1._6.Commands;
using ElgatoWaveSDK.Models._1._6.Events;
using ElgatoWaveSDK.HumbleObjects;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json.Nodes;
using System.Threading;
using System.Text;

namespace ElgatoWaveSDK.Clients
{
    public class ElgatoWaveClient
    {
        #region Private Vars

        internal IHumbleClientWebSocket? _socket;
        internal ClientConfig? Config
        {
            get; set;
        }
        internal int Port
        {
            get; set;
        }
        internal CancellationTokenSource? _source;
        internal ITransactionTracker? _transactionTracker;
        internal IReceiverUtils? _receiver;

        #endregion

        #region Public Vars

        public ClientConfig? ClientConfig => Config;
        public bool IsConnected => _socket?.State == WebSocketState.Open;

        #endregion

        #region Public Events

        public event EventHandler<ElgatoException>? ExceptionOccurred;

        public event EventHandler<MicrophoneConfigChanged>? MicrophoneConfigChanged;

        public event EventHandler<OutputSwitched>? OutputSwitched;
        public event EventHandler<OutputMuteChanged>? OutputMuteChanged;
        public event EventHandler<OutputVolumeChanged>? OutputVolumeChanged;
        public event EventHandler<SelectedOutputChanged>? SelectedOutputChanged;

        public event EventHandler<InputNameChanged>? InputNameChanged;
        public event EventHandler<InputMuteChanged>? InputMuteChanged;
        public event EventHandler<InputVolumeChanged>? InputVolumeChanged;
        public event EventHandler? InputsChanged;
        public event EventHandler<InputStateChange>? InputStateChanged;

        public event EventHandler<FilterBypassStateChanged>? FilterBypassStateChanged;
        public event EventHandler<FilterChanged>? FilterChanged;
        public event EventHandler<FilterAdded>? FilterAdded;
        public event EventHandler<FilterRemoved>? FilterRemoved;

        #endregion

        public ElgatoWaveClient(ClientConfig? config = null)
        {
            BaseExceptionOccurred += (sender, exception) =>
            {
                ExceptionOccurred?.Invoke(this, exception);
            };

            EventReceived += OnEventReceived;

            Config = config;

            Init();
        }

        internal void Init()
        {
            Config ??= new ClientConfig();
            _source ??= new CancellationTokenSource();
            _transactionTracker ??= new TransactionTracker();
            _receiver ??= new ReceiverUtils();

            Port = Config.PortStart;
        }

        #region Connection
        public async Task ConnectAsync(bool ignoreVersionCheck = false)
        {
            var cycleCount = 0;
            while (_socket?.State != WebSocketState.Open && cycleCount < (Config ?? new ClientConfig()).MaxAttempts)
            {
                _socket ??= new HumbleClientWebSocket();
                try
                {
                    await _socket
                        .ConnectAsync(new Uri($"ws://127.0.0.1:{Port}/"), _source?.Token ?? CancellationToken.None)
                        .ConfigureAwait(false);
                }
                catch (WebSocketException e) when (e.Message == "Unable to connect to the remote server")
                {
                    //ignore for now
                    if (_socket.State is WebSocketState.Aborted or WebSocketState.Closed or WebSocketState.CloseReceived)
                    {
                        _socket = new HumbleClientWebSocket();
                    }
                }
                catch (Exception ex)
                {
                    BaseExceptionOccurred?.Invoke(this, new ElgatoException("Unknown exception while trying to connect", ex, _socket.State));
                    throw;
                }
                finally
                {
                    Port++;
                    if (Port > (Config ?? new ClientConfig()).PortStart + (Config ?? new ClientConfig()).PortRange)
                    {
                        cycleCount++;
                        Port = (Config ?? new ClientConfig()).PortStart;
                    }
                }
            }

            if (_socket?.State != WebSocketState.Open)
            {
                var ex = new ElgatoException($"Looped through possible ports {(Config ?? new ClientConfig())?.MaxAttempts} times and couldn't connect [{(Config ?? new ClientConfig()).PortStart}-{(Config ?? new ClientConfig()).PortStart + (Config ?? new ClientConfig()).PortRange}]", _socket?.State);
                BaseExceptionOccurred?.Invoke(this, ex);
                throw ex;
            }

            StartReceiver();

            if (!ignoreVersionCheck)
            {
                var appInfo = await GetAppInfo();
                if (appInfo is null)
                {
                    throw new ElgatoException("Unable to get application info for version check", _socket.State);
                }

                if (appInfo.InterfaceRevision != ElgatoConstants.Values.ApplicationInfo.InterfaceRevision)
                {

                    try
                    {
                        throw new ElgatoException($"Unsupported version! Found: {appInfo?.InterfaceRevision} | Expected: {ElgatoConstants.Values.ApplicationInfo.InterfaceRevision}", _socket.State);
                    }
                    catch (ElgatoException ex)
                    {
                        if (ignoreVersionCheck)
                        {
                            ExceptionOccurred?.Invoke(this, ex);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        public void Disconnect()
        {
            _source?.Cancel();
            _source = null;

            _socket?.Dispose();
            _socket = null;
        }
        #endregion Connection

        #region Get Commands

        public Task<ApplicationInfo?> GetAppInfo()
        {
            return SendCommand<ApplicationInfo>(ElgatoConstants.Commands.GetApplicationInfo);
        }

        public Task<List<MicrophoneConfig>?> GetMicConfigs()
        {
            return SendCommand<List<MicrophoneConfig>>(ElgatoConstants.Commands.GetMicrophoneConfig);
        }

        public Task<SwitchState?> GetSwitchState()
        {
            return SendCommand<SwitchState>(ElgatoConstants.Commands.GetSwitchState);
        }

        public Task<OutputModel> GetOutputs()
        {
            return SendCommand<OutputModel>(ElgatoConstants.Commands.GetOutputs);
        }

        public Task<OutputConfig> GetOutputConfig()
        {
            return SendCommand<OutputConfig>(ElgatoConstants.Commands.GetOutputConfig);
        }

        #endregion

        #region Set Commands

        //public Task<dynamic?> SetMicConfig(string Id, string property, string value, bool isAdjustVolume)
        //{
        //    var values = new Dictionary<string, object>()
        //    {
        //        [ElgatoConstants.Properties.Common.Identifier] = Id,
        //        [ElgatoConstants.Properties.Common.Property] = property,
        //        [ElgatoConstants.Properties.Common.Value] = value,
        //        [ElgatoConstants.Properties.Common.IsAdjustVolume] = isAdjustVolume,
        //    };

        //    return SendCommand<MicrophoneConfig, dynamic>(ElgatoConstants.Commands.SetMicrophoneConfig, Utils.ToDynamic(values));
        //}

        public Task SwitchState()
        {
            return SendCommandNoReply(ElgatoConstants.Commands.SwitchOutput);
        }

        //public Task SetSelectedOutput(string outputId)
        //{
        //    var values = new Dictionary<string, object>()
        //        {
        //            [ElgatoConstants.Properties.Common.Identifier] = outputId,
        //        };

        //    return SendCommandNoReply(ElgatoConstants.Commands.SetSelectedOutput);
        //}

        //public Task SetOutput(string mixerID, string property, string value)
        //{
        //    var values = new Dictionary<string, object>()
        //        {
        //            [ElgatoConstants.Properties.Common.MixerID] = mixerID,
        //            [ElgatoConstants.Properties.Common.Property] = property,
        //            [ElgatoConstants.Properties.Common.Value] = value,
        //            [ElgatoConstants.Properties.Common.ForceLink] = force,
        //        };

        //        return SendCommand<MicrophoneConfig, dynamic>(ElgatoConstants.Commands.SetOutputConfig, Utils.ToDynamic(values));
        //}

        #endregion

        #region RecieverEvents

        private void OnEventReceived(object sender, EventReceivedArgs e)
        {
            object? obj = null;

            switch (e.EventName)
            {
                #region Mic
                case ElgatoConstants.Events.MicrophoneConfigChanged:
                    obj = JsonSerializer.Deserialize<MicrophoneConfigChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        MicrophoneConfigChanged?.Invoke(this, obj as MicrophoneConfigChanged ?? throw new InvalidOperationException());
                    }
                    break;
                #endregion

                #region Output
                case ElgatoConstants.Events.OutputSwitched:
                    obj = JsonSerializer.Deserialize<OutputSwitched>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        OutputSwitched?.Invoke(this, obj as OutputSwitched ?? throw new InvalidOperationException());
                    }
                    break;
                case ElgatoConstants.Events.SelectedOutputChanged:
                    obj = JsonSerializer.Deserialize<SelectedOutputChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        SelectedOutputChanged?.Invoke(this, obj as SelectedOutputChanged ?? throw new InvalidOperationException());
                    }
                    break;
                case ElgatoConstants.Events.OutputMuteChanged:
                    obj = JsonSerializer.Deserialize<OutputMuteChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        OutputMuteChanged?.Invoke(this, obj as OutputMuteChanged ?? throw new InvalidOperationException());
                    }
                    break;
                case ElgatoConstants.Events.OutputVolumeChanged:
                    obj = JsonSerializer.Deserialize<OutputVolumeChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        OutputVolumeChanged?.Invoke(this, obj as OutputVolumeChanged ?? throw new InvalidOperationException());
                    }
                    break;
                #endregion

                #region Input
                case ElgatoConstants.Events.InputsChanged:
                    InputsChanged?.Invoke(this, EventArgs.Empty);
                    break;
                case ElgatoConstants.Events.InputMuteChanged:
                    obj = JsonSerializer.Deserialize<InputMuteChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        InputMuteChanged?.Invoke(this, obj as InputMuteChanged ?? throw new InvalidOperationException());
                    }
                    break;
                case ElgatoConstants.Events.InputVolumeChanged:
                    obj = JsonSerializer.Deserialize<InputVolumeChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        InputVolumeChanged?.Invoke(this, obj as InputVolumeChanged ?? throw new InvalidOperationException());
                    }
                    break;
                case ElgatoConstants.Events.InputNameChanged:
                    obj = JsonSerializer.Deserialize<InputNameChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        InputNameChanged?.Invoke(this, obj as InputNameChanged ?? throw new InvalidOperationException());
                    }
                    break;
                case ElgatoConstants.Events.InputEnabled: //No idea how to trigger this to test it out
                case ElgatoConstants.Events.InputDisabled:
                    obj = JsonSerializer.Deserialize<InputStateChange>(e.Obj ?? "{}");
                    if (obj is InputStateChange stateChange)
                    {
                        stateChange.State = e.EventName == ElgatoConstants.Events.InputEnabled;
                        InputStateChanged?.Invoke(this, stateChange);
                    }
                    break;
                #endregion

                #region Filter
                case ElgatoConstants.Events.FilterBypassStateChanged:
                    obj = JsonSerializer.Deserialize<FilterBypassStateChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        FilterBypassStateChanged?.Invoke(this, obj as FilterBypassStateChanged ?? throw new InvalidOperationException());
                    }
                    break;
                case ElgatoConstants.Events.FilterAdded:
                    obj = JsonSerializer.Deserialize<FilterAdded>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        FilterAdded?.Invoke(this, obj as FilterAdded ?? throw new InvalidOperationException());
                    }
                    break;
                case ElgatoConstants.Events.FilterChanged:
                    obj = JsonSerializer.Deserialize<FilterChanged>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        FilterChanged?.Invoke(this, obj as FilterChanged ?? throw new InvalidOperationException());
                    }
                    break;
               
                case ElgatoConstants.Events.FilterRemoved:
                    obj = JsonSerializer.Deserialize<FilterRemoved>(e.Obj ?? "{}");
                    if (obj is not null)
                    {
                        FilterRemoved?.Invoke(this, obj as FilterRemoved ?? throw new InvalidOperationException());
                    }
                    break;
                #endregion
            }


            //TODO Log unknown event
        }
        #endregion

        #region ReceiverTask

        internal bool _receiverStarted = false;
        internal async Task WaitForReceiverToStart(int timeout)
        {
            var timeLeft = timeout;
            while (!_receiverStarted)
            {
                await Task.Delay(25);
                timeLeft -= 25;
                if (timeLeft < 0)
                {
                    throw new TimeoutException();
                }
            }
        }

        internal void StartReceiver()
        {
            Task.Run(ReceiverRun, _source?.Token ?? CancellationToken.None);
        }

        internal event EventHandler<ElgatoException>? BaseExceptionOccurred;
        internal event EventHandler<EventReceivedArgs>? EventReceived;

        private async Task ReceiverRun()
        {
            while (!_source?.IsCancellationRequested ?? false)
            {
                if (_socket?.State == WebSocketState.Open)
                {
                    _receiverStarted = true;

                    try
                    {
                        var baseObject = await _receiver.WaitForData(_socket, ClientConfig, _source?.Token ?? CancellationToken.None).ConfigureAwait(false);
                        if (baseObject == null)
                        {
                            continue;
                        }

                        baseObject.ReceivedAt = DateTime.Now;

                        foreach (var cache in _responseCache.Where(c => c.Value.ReceivedAt < DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(2))))
                        {
                            _responseCache.Remove(cache.Key);
                        }

                        if (baseObject is { Id: 0 }) //Id 0 = Event message
                        {
                            EventReceived?.Invoke(this, new EventReceivedArgs(baseObject.Method ?? "unknownEvent", baseObject.Obj));
                        }
                        else
                        {
                            if (!_responseCache.ContainsKey(baseObject.Id))
                            {
                                _responseCache.Add(baseObject.Id, baseObject);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        BaseExceptionOccurred?.Invoke(this, new ElgatoException("Unknown error in receiving task", ex, _socket.State));
                    }

                }
                else
                {
                    BaseExceptionOccurred?.Invoke(this, new ElgatoException("Socket connection failed, attempting a reconnect", _socket?.State));

                    Port = (Config ?? new ClientConfig()).PortStart;
                    try
                    {
                        await ConnectAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        BaseExceptionOccurred?.Invoke(this, new ElgatoException("Failed to reconnect, exiting thread...", ex, _socket?.State));
                        break;
                    }
                }
            }
        }

        #endregion ReceiverTask

        #region Base commands

        private readonly Dictionary<int, SocketBaseObject<JsonNode?, JsonDocument?>> _responseCache = new();

        internal Task SendCommandNoReply(string method)
        {
            if (_socket?.State == WebSocketState.Open)
            {
                var objId = _transactionTracker?.NextTransactionId() ?? (1000 + new Random().Next(10, 100000));

                SocketBaseObject<object?, object?> baseObject = new()
                {
                    Method = method,
                    Id = objId,
                };
                var s = baseObject.ToJson();
                var array = Encoding.UTF8.GetBytes(s);
                return _socket.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, _source?.Token ?? CancellationToken.None);
            }

            throw new ElgatoException("Connection not open", _socket.State);
        }

        internal Task<T?> SendCommand<T>(string method)
        {
            return SendCommand<T, T>(method, default);
        }

        internal async Task<OutT?> SendCommand<OutT, InT>(string method, InT? objectJson = default)
        {
            if (_socket?.State == WebSocketState.Open)
            {
                var objId = _transactionTracker?.NextTransactionId() ?? (1000 + new Random().Next(10, 100000));

                SocketBaseObject<InT?, OutT?> baseObject = new()
                {
                    Method = method,
                    Id = objId,
                    Obj = objectJson
                };
                var s = baseObject.ToJson();
                var array = Encoding.UTF8.GetBytes(s);
                await _socket.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, _source?.Token ?? CancellationToken.None).ConfigureAwait(false);

                SpinWait.SpinUntil(() => _responseCache.ContainsKey(baseObject.Id), TimeSpan.FromMilliseconds((Config ?? new ClientConfig()).ResponseTimeout));

                if (_responseCache.ContainsKey(baseObject.Id))
                {
                    var reply = _responseCache[baseObject.Id];
                    _responseCache.Remove(reply.Id);

                    return (reply?.Result ?? JsonDocument.Parse("{}")).Deserialize<OutT?>();
                }
            }

            return default;
        }

        #endregion
    }
}
