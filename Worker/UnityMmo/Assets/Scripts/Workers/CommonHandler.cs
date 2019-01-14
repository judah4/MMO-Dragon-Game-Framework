using System.Collections;
using System.Collections.Generic;
using MessageProtocols.Core;
using MessageProtocols.Server;
using MmoWorker;
using UnityEngine;

public class CommonHandler : MonoBehaviour
{
    protected GameObjectRepresentation GameObjectRepresentation;
    protected MmoClient Client;
    protected MmoClientMessageSender MessageSender;

    public string WorkerType = "worker";
    public string ipAddress = "localhost";
    public short port = 1337;

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

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Logger.LogMethod = Debug.Log;
        Telepathy.Logger.LogWarningMethod = Debug.LogWarning;
        Telepathy.Logger.LogErrorMethod = Debug.LogError;

        GameObjectRepresentation = new GameObjectRepresentation(this);

        Client = new MmoClient();
        Client.LoopOtherThread = false;
        Client.OnEntityCreation += GameObjectRepresentation.OnEntityCreation;
        Client.OnConnect += OnConnect;

        Client.Connect(ipAddress, port);


        MessageSender = new MmoClientMessageSender(Client);
    }

    void OnConnect()
    {
        UpdateInterestArea(new Vector3(0,0,0));
    }

    // Update is called once per frame
    void Update()
    {
        Client.Update();
    }

    void OnApplicationQuit()
    {
        Client.Disconnect();
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
        var sendPos = new Position() {X = (double)position.x - transform.position.x, Y = (double)position.y - transform.position.y, Z = (double)position.z - transform.position.z, };
        MessageSender.SendInterestChange(sendPos);
    }
}
