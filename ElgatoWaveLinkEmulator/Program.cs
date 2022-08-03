using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Newtonsoft.Json;
using ElgatoWaveSDK;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveLinkEmulator
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            int port = 1820;
            if (args.Length == 1)
            {
                if (!Int32.TryParse(args.First(), out port))
                {
                    Console.WriteLine($"Unable to parse port number: {args.First()}");
                    Environment.Exit(1);
                    return;
                }
            }

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{port}/");
            listener.Prefixes.Add($"http://localhost:{port}/");

            listener.Start();
            Console.WriteLine("Started listening on:");
            foreach (var listenerPrefix in listener.Prefixes)
            {
                Console.WriteLine($"\t{listenerPrefix}");
            }

            CancellationTokenSource cancelSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!cancelSource.IsCancellationRequested)
                {
                    var context = await listener.GetContextAsync().ConfigureAwait(true);
                    if (context.Request.IsWebSocketRequest)
                    {
                        HandleContext(context, cancelSource.Token);
                    }
                    else
                    {
                        Console.WriteLine("Request was not a websocket, ignoring");
                    }
                }
            }, cancelSource.Token);

            Console.WriteLine("Press any key to cancel...");
            Console.ReadKey();
            cancelSource.Cancel();
        }

        private static async Task HandleContext(HttpListenerContext context, CancellationToken token)
        {
            var websocketContext = await context.AcceptWebSocketAsync(null).ConfigureAwait(true);
            var websocket = websocketContext.WebSocket;

            var _maxBufferSize = 1024 * 10;

            while (websocket.State == WebSocketState.Open)
            {
                var buffer = new byte[1024];
                var offset = 0;
                var free = buffer.Length;

                WebSocketReceiveResult? result = null;
                do
                {
                    result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, free), token).ConfigureAwait(false);
                    offset += result.Count;
                    free -= result.Count;
                    if (free == 0)
                    {
                        var newSize = buffer.Length + 1024;
                        if (newSize > _maxBufferSize)
                        {
                            throw new Exception("Maximum receive buffer size exceeded");
                        }
                        var newBuffer = new byte[newSize];
                        Array.Copy(buffer, 0, newBuffer, 0, offset);
                        buffer = newBuffer;
                        free = buffer.Length - offset;
                    }

                    string receviedJson = string.Empty;
                    try
                    {
                        receviedJson = Encoding.UTF8.GetString(buffer).Replace("\0", "");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to turn stream into a string: {ex.Message} {ex.StackTrace}");
                        continue;
                    }

                    SocketBaseObject<string, string>? receivedObject = null;
                    try
                    {
                        receivedObject = JsonConvert.DeserializeObject<SocketBaseObject<string, string>?>(receviedJson, new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to turn json to object: {ex.Message} {ex.StackTrace}");
                        continue;
                    }

                    switch (receivedObject.Method)
                    {
                        case "emulatorTest":
                            break;
                        case "getApplicationInfo":
                            receivedObject.Result = JsonConvert.SerializeObject(ObjectGenerator.GetApplicationInfo());
                            break;
                        case "getAllChannelInfo":
                            receivedObject.Result = JsonConvert.SerializeObject(ObjectGenerator.GetChannels());
                            break;
                    }

                    receivedObject.Obj = null;
                    var replyBytes = Encoding.UTF8.GetBytes(receivedObject.ToJson());

                    await websocket.SendAsync(replyBytes, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, token).ConfigureAwait(true);

                } while (!result?.EndOfMessage ?? false);
            }
        }
    }
}