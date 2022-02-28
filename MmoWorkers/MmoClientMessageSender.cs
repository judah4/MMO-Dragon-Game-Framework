using MessagePack;
using Mmogf;
using Mmogf.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MmoWorkers
{
    public class MmoClientMessageSender
    {
        private MmoWorker _client;

        public MmoClientMessageSender(MmoWorker mmoClient)
        {
            _client = mmoClient;

        }

        public void SendInterestChange(Position position)
        {
            var changeInterest = new ChangeInterestArea()
            {
                Position = position,
            };

            _client.Send(new MmoMessage()
            {
                MessageId = ServerCodes.ChangeInterestArea,
                Info = MessagePackSerializer.Serialize(changeInterest),
            });
        }

        public void SendEntityUpdate(int entityId, int componentId, IMessage message)
        {

            var changeInterest = new EntityUpdate()
            {
                EntityId = entityId,
                ComponentId = componentId,
                Info = MessagePackSerializer.Serialize(message),
            };

            _client.Send(new MmoMessage()
            {
                MessageId = ServerCodes.EntityUpdate,
                Info = MessagePackSerializer.Serialize(changeInterest),
            });
        }

    }
}
