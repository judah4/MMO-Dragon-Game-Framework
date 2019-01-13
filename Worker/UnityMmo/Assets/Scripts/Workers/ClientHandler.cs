using System.Collections;
using System.Collections.Generic;
using MmoWorker;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    private MmoClient _client;

    public string ipAddress = "localhost";
    public short port = 1337;

    // Start is called before the first frame update
    void Start()
    {
        // update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Logger.LogMethod = Debug.Log;
        Telepathy.Logger.LogWarningMethod = Debug.LogWarning;
        Telepathy.Logger.LogErrorMethod = Debug.LogError;

        _client = new MmoClient();
        _client.LoopOtherThread = false;
        _client.Connect(ipAddress, port);
    }

    // Update is called once per frame
    void Update()
    {
        _client.Update();
    }
}
