using System;
using System.Collections;
using System.Collections.Generic;
using MmoWorker;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    private GameObjectRepresentation _gameObjectRepresentation = new GameObjectRepresentation();
    private MmoClient _client;

    public string ipAddress = "localhost";
    public short port = 1337;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor == false)
        {
            var newIp = GetArg("hostIp");
            var newPort = GetArg("hostPort");

            if (newIp!= null)
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

        _client = new MmoClient();
        _client.LoopOtherThread = false;
        _client.OnEntityCreation += _gameObjectRepresentation.OnEntityCreation;
        _client.Connect(ipAddress, port);
    }

    // Update is called once per frame
    void Update()
    {
        _client.Update();
    }

    void OnApplicationQuit()
    {
        _client.Disconnect();
    }

    // Helper function for getting the command line arguments
    private static string GetArg(string name)
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
}
