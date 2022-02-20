using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonHandler : MonoBehaviour
{
    protected GameObjectRepresentation GameObjectRepresentation;
    protected MmoWorker Client;

    public string WorkerType = "worker";
    public string ipAddress = "localhost";
    public short port = 1337;

    public long ClientId
    {
        get { return Client?.ClientId ?? 0; }
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

        Client = new MmoWorker();
        Client.OnLog += Debug.Log;
        Client.OnEntityCreation += GameObjectRepresentation.OnEntityCreation;
        Client.OnEntityUpdate += GameObjectRepresentation.OnEntityUpdate;

        Client.OnConnect += OnConnect;

        Client.Connect(ipAddress, port);

    }

    void OnConnect()
    {
        UpdateInterestArea(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Client.Update();
    }

    void OnApplicationQuit()
    {
        Client.Stop();
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

    public void UpdateEntity(int entityId, int componentId, object position)
    {
        GameObjectRepresentation.UpdateEntity(entityId, componentId, position);
        Client.SendEntityUpdate(entityId, componentId, position);
    }
}
