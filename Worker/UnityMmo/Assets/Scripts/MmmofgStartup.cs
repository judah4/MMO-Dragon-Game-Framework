using MessagePack;
using MessagePack.Resolvers;
using Mmogf.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public static class MmogfStartup
    {
        static bool serializerRegistered = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            if (!serializerRegistered)
            {
                StaticCompositeResolver.Instance.Register(
                    MessagePack.Resolvers.MmogfCoreResolver.Instance,
                    MessagePack.Resolvers.GeneratedResolver.Instance,
                    MessagePack.Resolvers.StandardResolver.Instance
                );

                var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

                MessagePackSerializer.DefaultOptions = option;
                serializerRegistered = true;

                PlayerCreatorHandler.CreatePlayer += CreatePlayer;

                LoadEntityComponentTypesList();
            }
        }

        static void LoadEntityComponentTypesList()
        {
            Dictionary<int, System.Type> types = new Dictionary<int, System.Type>();
            //map components to ids
            //yay reflection!
            var type = typeof(IEntityComponent);
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int cnt = 0; cnt < assemblies.Length; cnt++)
            {
                var assembly = assemblies[cnt];
                var typesList = assembly.GetTypes();
                for(int i = 0; i < typesList.Length; i++)
                {
                    var t = typesList[i];
                    if(type.IsAssignableFrom(t) && t.IsInterface == false)
                    {
                        var component = (IEntityComponent)System.Activator.CreateInstance(t);
                        //Debug.Log($"{component.GetComponentId()}, {t.Name}");
                        types.Add(component.GetComponentId(), t);
                    }
                }
            }

            ComponentMappings.Init(types);
        }

        static CreateEntityRequest CreatePlayer(PlayerCreator.ConnectPlayer connect, CommandRequest request)
        {
            var clientId = request.RequesterId;

            //pass in player id and load data at some point
            //connect.PlayerId;

            Debug.Log($"Creating Player {clientId}");

            var createEntity = new CreateEntityRequest("Player", new Position() { Y = 0, }, Rotation.Identity,
                new Dictionary<int, byte[]>()
                {
                    { Cannon.ComponentId, MessagePack.MessagePackSerializer.Serialize(new Cannon()) },
                    { Health.ComponentId, MessagePack.MessagePackSerializer.Serialize(new Health() { Current = 100, Max = 100, }) },
                    { ClientAuthCheck.ComponentId, MessagePack.MessagePackSerializer.Serialize(new ClientAuthCheck() {  WorkerId = clientId, }) },
                    { MovementState.ComponentId, MessagePack.MessagePackSerializer.Serialize(new MovementState() {  Forward = 0, Heading = 0, DesiredPosition = new Vector3d() { Y = 0, } }) },
                    { PlayerHeartbeatServer.ComponentId, MessagePack.MessagePackSerializer.Serialize(new PlayerHeartbeatServer() { MissedHeartbeats = 0 }) },
                    { PlayerHeartbeatClient.ComponentId, MessagePack.MessagePackSerializer.Serialize(new PlayerHeartbeatClient() { }) },
                },
                new List<Acl>()
                {
                    new Acl() { ComponentId = Position.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Rotation.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Acls.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Cannon.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = Health.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = ClientAuthCheck.ComponentId, WorkerType = $"Dragon-Client", WorkerId = clientId, },
                    new Acl() { ComponentId = MovementState.ComponentId, WorkerType = $"Dragon-Client", WorkerId = clientId, },
                    new Acl() { ComponentId = PlayerHeartbeatServer.ComponentId, WorkerType = "Dragon-Worker" },
                    new Acl() { ComponentId = PlayerHeartbeatClient.ComponentId, WorkerType = $"Dragon-Client", WorkerId = clientId, },

                } );

            return createEntity;
        }

    //#if UNITY_EDITOR


    //    [UnityEditor.InitializeOnLoadMethod]
    //    static void EditorInitialize()
    //    {
    //        Initialize();
    //    }

    //#endif
    }
}