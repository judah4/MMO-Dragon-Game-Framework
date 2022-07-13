using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mmogf
{
    public class ShipStatsPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _healthText;
        [SerializeField]
        private TMP_Text _cannonText;
        [SerializeField]
        PlayerControlsVisualizer _player;

        // Update is called once per frame
        void Update()
        {
            if(_player == null)
                return;

            var health = _player.GetEntityComponent<Health>(Health.ComponentId).Value;

            _healthText.text = $"Health: {health.Current}/{health.Max}";
            _cannonText.text = $"Cannons: 1"; //add variable eventually
        }

        public void AttachPlayer(PlayerControlsVisualizer player)
        {
            _player = player;
        }
    }
}
