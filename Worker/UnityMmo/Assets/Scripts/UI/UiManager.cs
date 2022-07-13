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
        [SerializeField]
        ClientConnectText _clientConnectText;

        [SerializeField]
        ShipStatsPanel _shipStatsPanel;

        [SerializeField]
        PlayerControlsVisualizer _player;

        int _ping = 0;

        // Start is called before the first frame update
        void Start()
        {
            if(_clientHandler == null)
            {
                _clientHandler = Object.FindObjectOfType<ClientHandler>();
            }

            if(_clientHandler == null)
                return;

            _clientConnectText.Client = _clientHandler;

        }

        // Update is called once per frame
        void Update()
        {
            if (_clientHandler == null)
                return;

            string ping;
            if(_clientHandler.Status == Lidgren.Network.NetConnectionStatus.Connected)
            {
                if(_ping == _clientHandler.Ping)
                    return;
                _ping = _clientHandler.Ping;
                ping = _clientHandler.Ping.ToString();
            }
            else
            {
                if (_ping == 0)
                    return;

                _ping = 0;
                ping = "---";
            }

            _pingText.text = $"Ping: {ping}";
        }

        public void AttachPlayer(PlayerControlsVisualizer playerControlsVisualizer)
        {
            _player = playerControlsVisualizer;
            _shipStatsPanel.AttachPlayer(_player);
        }
    }
}
