using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Mmogf
{

    public class ClientConnectText : MonoBehaviour
    {
        public ClientHandler Client;
        public TMP_Text Text;

        long _clientId = 0;

        // Start is called before the first frame update
        void Start()
        {
            if(Client == null)
                return;

        }

        // Update is called once per frame
        void Update()
        {
            if (Client == null)
                return;

            if(_clientId == Client.ClientId)
                return;

            _clientId = Client.ClientId;
            Text.text = $"ClientId: {Client.ClientId}";
        }
    }
}