using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{


    public class CommonHandler : MonoBehaviour
    {
        protected GameObjectRepresentation GameObjectRepresentation;
        protected MmoWorker Client;

        public List<CommandRequest> CommandRequests = new List<CommandRequest>();

        public string WorkerType = "Dragon-Worker";
        public string ipAddress = "localhost";
        public short port = 1337;

        public long ClientId
        {
            get { return Client?.ClientId ?? 0; }
        }

        public int Ping
        {
            get { return Client?.Ping ?? 0; }
        }
        public NetConnectionStatus Status
        {
            get { return Client?.Status ?? NetConnectionStatus.None; }
        }

        void OnEnable()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Application.isEditor == false)
            {
                var newIp = GetArg("hostIp");
                var newPort = GetArg("hostPort");

                if (newIp != null)
                {
                    ipAddress = newIp;
                }

                if (newPort != null)
                {
                    short portTemp;
                    if (short.TryParse(newPort, out portTemp))
                    {
                        port = portTemp;
                    }
                }
            }

            // update even if window isn't focused, otherwise we don't receive.
            Application.runInBackground = true;


            GameObjectRepresentation = new GameObjectRepresentation(this);

            var config = new NetPeerConfiguration(WorkerType);

            Client = new MmoWorker(config);
            Client.OnLog += Debug.Log;
            Client.OnEntityCreation += GameObjectRepresentation.OnEntityCreation;
            Client.OnEntityUpdate += GameObjectRepresentation.OnEntityUpdate;
            Client.OnEntityCommand += OnEntityCommand;

            Client.OnConnect += OnConnect;

            Client.Connect(WorkerType, ipAddress, port);


        }

        void OnConnect()
        {
            UpdateInterestArea(transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            CommandRequests.Clear(); //reset every tick

            Client.Update();

            OnUpdate();
        }

        void OnApplicationQuit()
        {
            Client.Stop();
        }

        protected virtual void OnUpdate()
        {

        }

        // Helper function for getting the command line arguments
        protected static string GetArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        protected void UpdateInterestArea(Vector3 position)
        {
            //adjust position this way to not lose precision
            var sendPos = PositionToServer(position);
            Client.SendInterestChange(sendPos);
        }

        public Position PositionToServer(Vector3 position)
        {
            var sendPos = new Position() { X = (double)position.x - transform.position.x, Y = (double)position.y - transform.position.y, Z = (double)position.z - transform.position.z, };
            return sendPos;
        }

        public Vector3 PositionToClient(Position position)
        {
            var adjustedPos = new Vector3((int)position.X, (int)position.Y, (int)position.Z) + transform.position;
            return adjustedPos;
        }

        public void UpdateEntity<T>(int entityId, int componentId, T component) where T : IEntityComponent
        {
            GameObjectRepresentation.UpdateEntity(entityId, componentId, component);
            Client.SendEntityUpdate(entityId, componentId, component);
        }

        private void OnEntityCommand(CommandRequest request)
        {
            //add to list to be handled
            CommandRequests.Add(request); 
        }


        public void SendCommand<T>(int entityId, int componentId, T fireCommand, System.Action<CommandResponse> callback = null) where T : ICommand
        {
            Client.SendCommand(entityId, componentId, fireCommand, callback);
        }
        public void SendCommandResponse<T>(CommandRequest request, T responsePayload) where T : ICommand
        {
            Client.SendCommandResponse(request, responsePayload);
        }

    }

}