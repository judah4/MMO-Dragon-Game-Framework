using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf.Core
{


    public class CommonHandler : MonoBehaviour
    {
        public GameObjectRepresentation GameObjectRepresentation { get; protected set; }
        public Vector3 InterestCenter => _interestCenter;
        public bool UpdateInterestFromPosition => _updateInterestFromPosition;

        protected MmoWorker Client;
        protected float ConnectDelay = 0;

        public List<CommandRequest> CommandRequests = new List<CommandRequest>();
        public List<EventRequest> EventRequests = new List<EventRequest>();


        public string WorkerType = "Dragon-Worker";
        public string ipAddress = "localhost";
        public short port = 1337;

        //local vars
        private List<int> _entitiyClears = new List<int>();

        [SerializeField]
        private Vector3 _interestCenter = Vector3.zero;
        [SerializeField]
        private bool  _updateInterestFromPosition;



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
                var newIp = GetArg("--hostIp");
                var newPort = GetArg("--hostPort");

                if (newIp != null)
                {
                    Debug.Log($"Setting IP {newIp}");
                    ipAddress = newIp;
                }

                if (newPort != null)
                {
                    short portTemp;
                    if (short.TryParse(newPort, out portTemp))
                    {
                        Debug.Log($"Setting Port {portTemp}");
                        port = portTemp;
                    }
                }
            }

            // update even if window isn't focused, otherwise we don't receive.
            Application.runInBackground = true;

            Debug.Log($"Connecting to {ipAddress}:{port}");

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
                    GameObjectRepresentation.DeleteEntity(checkout.Checkouts[cnt]);
                }
            }
            //else
            //{
            //    if(GameObjectRepresentation.Entities != null)
            //    {
            //        _entitiyClears.Clear();
            //        foreach (var entity in GameObjectRepresentation.Entities)
            //        {
            //            if (checkout.Checkouts.Contains(entity.Key))
            //                continue;

            //            _entitiyClears.Add(entity.Key);
            //        }

            //        for (int cnt = 0; cnt < _entitiyClears.Count; cnt++)
            //        {
            //            #if UNITY_EDITOR
            //            Debug.Log($"Deleting {_entitiyClears[cnt]} from out of range.");
            //            #endif
            //            GameObjectRepresentation.DeleteEntity(_entitiyClears[cnt]);

            //        }
            //    }
                
            //}
        }

        void OnConnectHandle()
        {
            var pos = Vector3.zero;
            if(_updateInterestFromPosition)
            {
                pos = transform.position;
            }
            UpdateInterestArea(pos, true);

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

            GameObjectRepresentation.UpdateInterests();

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
            Debug.Log(string.Join(",", args));
            for (int i = 0; i < args.Length; i++)
            {
                if(args[i] == null)
                    continue;
                if (args[i].StartsWith(name, System.StringComparison.InvariantCultureIgnoreCase) && args.Length > i)
                {
                    var splits = args[i].Split('=');

                    return splits[splits.Length-1];
                }
            }
            return null;
        }

        public void UpdateInterestArea(Vector3 position, bool force = false)
        {
            var dif = _interestCenter - position;

            if(dif.sqrMagnitude > 5f * 5f || force)
            {
                _interestCenter = position;
                //adjust position this way to not lose precision
                //var sendPos = PositionToServer(position);
                var sendPos = new Position() { X = (double)position.x, Y = (double)position.y, Z = (double)position.z, };
                Client.SendInterestChange(sendPos);
            }

            
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