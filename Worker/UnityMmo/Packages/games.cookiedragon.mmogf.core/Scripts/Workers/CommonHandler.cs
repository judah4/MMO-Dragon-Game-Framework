using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{


    public class CommonHandler : MonoBehaviour
    {
        public GameObjectRepresentation GameObjectRepresentation { get; protected set; }

        [SerializeField]
        private GameObjectRepresentation _gameObjectRepresentation;
        protected MmoWorker Client;
        protected float ConnectDelay = 0;

        public List<CommandRequest> CommandRequests = new List<CommandRequest>();
        public List<EventRequest> EventRequests = new List<EventRequest>();


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

        protected virtual void Init()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
            StartCoroutine(ConnectGame());
        }

        IEnumerator ConnectGame()
        {      
            yield return new WaitForSeconds(ConnectDelay);

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
            Client.OnLog += (logLevel, message) => {
                switch(logLevel)
                {
                    case LogLevel.Debug:
                    default:
                        Debug.Log(message);
                        break;
                    case LogLevel.Warning:
                        Debug.LogWarning(message);
                        break;
                    case LogLevel.Error:
                        Debug.LogError(message);
                        break;
                }
            };
            Client.OnEntityCreation += GameObjectRepresentation.OnEntityCreation;
            Client.OnEntityUpdate += GameObjectRepresentation.OnEntityUpdate;
            Client.OnEntityEvent += OnEntityEvent;
            Client.OnEntityCheckout += OnEntityCheckout;
            Client.OnEntityCommand += OnEntityCommand;
            Client.OnEntityDelete += GameObjectRepresentation.OnEntityDelete;

            Client.OnConnect += OnConnectHandle;

            Client.Connect(WorkerType, ipAddress, port);


        }

        private void OnEntityCheckout(EntityCheckout checkout)
        {
            //save checkout list where?

            if(checkout.Remove)
            {
                for(int cnt = 0; cnt < checkout.Checkouts.Count; cnt++)
                {
                    _gameObjectRepresentation.DeleteEntity(checkout.Checkouts[cnt]);
                }
            }
        }

        void OnConnectHandle()
        {
            UpdateInterestArea(transform.position);

            OnConnect();
        }

        protected virtual void OnConnect()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Client == null)
                return;

            CommandRequests.Clear(); //reset every tick
            EventRequests.Clear();

            Client.Update();

            OnUpdate();
        }

        void OnApplicationQuit()
        {
            if (Client == null)
                return;
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
            var pos = transform.position;
            position.X += pos.x;
            position.Y += pos.y;
            position.Z += pos.z;
            var adjustedPos = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
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

        private void OnEntityEvent(EventRequest eventRequest)
        {
            //add to list to be handled
            EventRequests.Add(eventRequest);
        }

        public void SendCommand<T, TRequest, TResponse>(int entityId, int componentId, TRequest request, System.Action<CommandResult<T, TRequest, TResponse>> callback = null) where T : ICommandBase<TRequest, TResponse>, new () where TRequest : struct where TResponse : struct
        {
            var command = new T()
            {
                Request = request,
            };
            Client.SendCommand<T, TRequest, TResponse>(entityId, componentId, command, callback);
        }
        public void SendCommandResponse<T, TRequest, TResponse>(CommandRequest request, T command, TResponse responsePayload) where T : ICommandBase<TRequest,TResponse> where TRequest : struct where TResponse : struct
        {
            command.Response = responsePayload;

            Client.SendCommandResponse<T,TRequest, TResponse>(request, command);
        }
        public void SendCommandResponseFailure(CommandRequest request, string message)
        {

            Client.SendCommandResponseFailure(request, CommandStatus.Failure, message);
        }
        public void SendEvent<T>(int entityId, int componentId, T eventPayload) where T : IEvent
        {
            Client.SendEvent(entityId, componentId, eventPayload);
        }

    }

}