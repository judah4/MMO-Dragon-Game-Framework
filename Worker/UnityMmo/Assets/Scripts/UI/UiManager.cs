using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mmogf
{
    public class UiManager : MonoBehaviour
    {

        [SerializeField]
        private ClientHandler _clientHandler;
        [SerializeField]
        TMP_Text _pingText;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            string ping;
            if(_clientHandler.Status == Lidgren.Network.NetConnectionStatus.Connected)
            {
                ping = _clientHandler.Ping.ToString();
            }
            else
            {
                ping = "---";
            }

            _pingText.text = $"Ping: {ping}";
        }
    }
}
