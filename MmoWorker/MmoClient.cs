using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Google.Protobuf;
using Lidgren.Network;
using MessageProtocols;
using MessageProtocols.Core;
using MessageProtocols.Server;

namespace MmoWorker
{
    public class MmoClient
    {
        public long ClientId { get; private set; }

        private static NetClient s_client;

        public bool Connected => s_client.ConnectionStatus == NetConnectionStatus.Connected;

        public event Action<EntityInfo> OnEntityCreation;
        public event Action<EntityUpdate> OnEntityUpdate; 
        public event Action OnConnect;

        SynchronizationContext _sync;

        public MmoClient()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("dragon-bingus");
            config.AutoFlushSendQueue = false;
            s_client = new NetClient(config);

            _sync = new SynchronizationContext();
            s_client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage), _sync);

        }

        public void Connect(string host, short port)
        {
            s_client.Start();
            NetOutgoingMessage hail = s_client.CreateMessage("This is the hail message");
            s_client.Connect(host, port, hail);
        }

        public void Stop()
        {
            s_client.Disconnect("Requested by user");
        }

        public void GotMessage(object peer)
        {
            NetIncomingMessage im;
            while ((im = s_client.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        Console.WriteLine(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                        {
                            Console.WriteLine("Client connected");
                            //if self
                            OnConnect?.Invoke();
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {

                        }

                        string reason = im.ReadString();
                        Console.WriteLine(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        var simpleData = SimpleMessage.Parser.ParseFrom(im.Data);
                        Console.WriteLine("Client " + s_client.UniqueIdentifier + " Data: " + (ServerCodes)simpleData.MessageId);
                        switch ((ServerCodes)simpleData.MessageId)
                        {
                            case ServerCodes.ClientConnect:
                                //ClientId = ClientConnect.Parser.ParseFrom(simpleData.Info).ClientId;
                                //Console.WriteLine("My Client Id is " + ClientId);
                                //OnConnect?.Invoke();
                                break;
                            case ServerCodes.GameData:
                                var gameData = GameData.Parser.ParseFrom(simpleData.Info);
                                Console.WriteLine($"Client Game Data: {BitConverter.ToString(gameData.Info.ToByteArray())}");

                                break;
                            case ServerCodes.EntityInfo:
                                var entityInfo = EntityInfo.Parser.ParseFrom(simpleData.Info);
                                Console.WriteLine($"Client Entity Info: {entityInfo.EntityId}");
                                foreach (var pair in entityInfo.EntityData)
                                {
                                    Console.WriteLine($"{pair.Key} {BitConverter.ToString(pair.Value.ToByteArray())}");
                                }

                                OnEntityCreation?.Invoke(entityInfo);
                                break;
                            case ServerCodes.EntityUpdate:
                                var entityUpdate = EntityUpdate.Parser.ParseFrom(simpleData.Info);

                                OnEntityUpdate?.Invoke(entityUpdate);

                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                s_client.Recycle(im);
            }
        }

        internal void Send(IMessage message)
        {
            NetOutgoingMessage om = s_client.CreateMessage();
            om.Write(message.ToByteArray());
            s_client.SendMessage(om, NetDeliveryMethod.Unreliable);
            s_client.FlushSendQueue();
        }

    }
}
