using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Google.Protobuf;
using MessageProtocols;
using MessageProtocols.Core;
using MessageProtocols.Server;

namespace MmoWorker
{
    public class MmoClient
    {
        public int ClientId { get; private set; }

        private Telepathy.Client _client;

        public bool LoopOtherThread { get; set; }

        public bool Connected => _client.Connected;
        public bool Connecting => _client.Connecting;

        public event Action<EntityInfo> OnEntityCreation;
        public event Action<EntityUpdate> OnEntityUpdate; 
        public event Action OnConnect;

        public MmoClient()
        {
            _client = new Telepathy.Client();
            LoopOtherThread = true;

        }

        public void Connect(string ip, short port)
        {
            _client.Connect(ip, port);

            if (LoopOtherThread)
            {
                var thread2 = new Thread(() => Loop());
                thread2.Start();
            }
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void Update()
        {
            // grab all new messages. do this in your Update loop.
            Telepathy.Message msg;


            while (_client.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Telepathy.Logger.Log("Client " + msg.connectionId + " Connected");
                        break;
                    case Telepathy.EventType.Data:
                        var simpleData = SimpleMessage.Parser.ParseFrom(msg.data);
                        Telepathy.Logger.Log("Client " + ClientId + " Data: " + (ServerCodes)simpleData.MessageId);
                        switch ((ServerCodes)simpleData.MessageId)
                        {
                            case ServerCodes.ClientConnect:
                                ClientId = ClientConnect.Parser.ParseFrom(simpleData.Info).ClientId;
                                Telepathy.Logger.Log("My Client Id is " + ClientId);
                                OnConnect?.Invoke();
                                break;
                            case ServerCodes.GameData:
                                var gameData = GameData.Parser.ParseFrom(simpleData.Info);
                                Telepathy.Logger.Log($"Client Game Data: {BitConverter.ToString(gameData.Info.ToByteArray())}");

                                break;
                            case ServerCodes.EntityInfo:
                                var entityInfo = EntityInfo.Parser.ParseFrom(simpleData.Info);
                                Telepathy.Logger.Log($"Client Entity Info: {entityInfo.EntityId}");
                                foreach (var pair in entityInfo.EntityData)
                                {
                                    Telepathy.Logger.Log($"{pair.Key} {BitConverter.ToString(pair.Value.ToByteArray())}");
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
                    case Telepathy.EventType.Disconnected:
                        Telepathy.Logger.Log("Client " + msg.connectionId + " Disconnected");
                        break;
                }
            }
        }

        void Loop()
        {

            while (_client.Connected || _client.Connecting)
            {

                Update();
                Thread.Sleep(10);

            }

        }

        internal void Send(IMessage message)
        {
            _client.Send(message.ToByteArray());
        }

    }
}
