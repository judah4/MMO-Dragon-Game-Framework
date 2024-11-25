using Mmogf.Core.Contracts;
using Mmogf.Core.Contracts.Commands;
using Mmogf.Servers.Serializers;
using Mmogf.Servers.Shared;

namespace Mmogf.Servers.Tests
{
    [TestClass]
    public class ProtobufSerializerTests
    {

        [TestMethod]
        public void Protobuf_Generics_Test()
        {
            var serializer = new ProtobufSerializer();

            var message = new MmoMessage<CommandRequest<CreateEntityRequest>>()
            {
                MessageId = ServerCodes.WorldCommandRequest,
                Info = new CommandRequest<CreateEntityRequest>(new CommandRequestHeader()
                {
                    CommandId = World.CreateEntity.CommandId,
                },
                new CreateEntityRequest("entityType", new Position(1, 2, 3).ToFixedVector3(), Rotation.Zero, new Dictionary<short, byte[]>(), new List<Acl>() { })),
            };

            var serializedData = serializer.Serialize(message);

            var deserializedMessage = serializer.Deserialize<MmoMessage<CommandRequest<CreateEntityRequest>>>(serializedData);

            Assert.IsNotNull(deserializedMessage);

            Assert.AreEqual(message.Info.Payload.Position.X, deserializedMessage.Info.Payload.Position.X);
        }

        [TestMethod]
        public void Protobuf_Generics_InitialNonGeneric_Test()
        {
            var serializer = new ProtobufSerializer();

            var expectedPayload = new CreateEntityRequest("entityType", new Position(1, 2, 3).ToFixedVector3(), Rotation.Zero, new Dictionary<short, byte[]>(), new List<Acl>() { });

            var message = new MmoMessage()
            {
                MessageId = ServerCodes.WorldCommandRequest,
                Info = serializer.Serialize(new CommandRequest(new CommandRequestHeader()
                {
                    CommandId = World.CreateEntity.CommandId,
                },
                serializer.Serialize(expectedPayload))),
            };

            var serializedData = serializer.Serialize(message);

            var deserializedMessage = serializer.Deserialize<MmoMessage<CommandRequest<CreateEntityRequest>>>(serializedData);

            Assert.IsNotNull(deserializedMessage);

            Assert.AreEqual(expectedPayload.Position.X, deserializedMessage.Info.Payload.Position.X);
        }


        [TestMethod]
        public void Protobuf_Generics_ToNonGenerics_Test()
        {
            var serializer = new ProtobufSerializer();

            var message = new MmoMessage<CommandRequest<CreateEntityRequest>>()
            {
                MessageId = ServerCodes.WorldCommandRequest,
                Info = new CommandRequest<CreateEntityRequest>(new CommandRequestHeader()
                {
                    CommandId = World.CreateEntity.CommandId,
                },
                new CreateEntityRequest("entityType", new Position(1, 2, 3).ToFixedVector3(), Rotation.Zero, new Dictionary<short, byte[]>(), new List<Acl>() { })),
            };

            var serializedData = serializer.Serialize(message);

            var deserializedMessage = serializer.Deserialize<MmoMessage>(serializedData);
            var deserializedCommand = serializer.Deserialize<CommandRequest>(deserializedMessage.Info);
            var deserializedPayload = serializer.Deserialize<CreateEntityRequest>(deserializedCommand.Payload);

            Assert.IsNotNull(deserializedMessage);

            Assert.AreEqual(message.Info.Payload.Position.X, deserializedPayload.Position.X);
        }
    }
}