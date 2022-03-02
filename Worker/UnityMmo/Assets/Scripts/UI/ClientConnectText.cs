using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Mmogf
{

    public class ClientConnectText : MonoBehaviour
    {
        public ClientHandler Client;
        public Text Text;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Text.text = $"ClientId: {Client.ClientId}";
        }
    }
}