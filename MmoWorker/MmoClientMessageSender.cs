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
    public class MmoClientMessageSender
    {
        private MmoClient _client;

        public MmoClientMessageSender(MmoClient mmoClient)
        {
            _client = mmoClient;

        }

        public void SendInterestChange(Position position)
        {
            var changeInterest = new ChangeInterestArea()
            {
                Position = position,
            };

            _client.Send(new SimpleMessage()
            {
                MessageId = (int) ServerCodes.ChangeInterestArea,
                Info = changeInterest.ToByteString(),
            });
        }

        public void SendEntityUpdate(int entityId, int componentId, IMessage message)
        {

            var changeInterest = new EntityUpdate()
            {
                EntityId = entityId,
                ComponentId = componentId,
                Info = message.ToByteString(),
            };

            _client.Send(new SimpleMessage()
            {
                MessageId = (int)ServerCodes.EntityUpdate,
                Info = changeInterest.ToByteString(),
            });
        }

    }
}
