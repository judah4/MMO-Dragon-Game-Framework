using Mmogf.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mmogf
{
    public class ShipVisualizer : BaseEntityBehavior
    {
        [SerializeField]
        GameObject _visual;

        [SerializeField]
        bool _dead = false;

        [SerializeField]
        Vector3 _deadPosition;
        [SerializeField]
        Vector3 _deadRotation;

        [SerializeField]
        Vector3 _startRot;

        [Range(0f, 2f)]
        [SerializeField]
        float _deadTime;

        private void OnEnable()
        {
            _startRot = _visual.transform.localRotation.eulerAngles;
        }

        // Update is called once per frame
        void Update()
        {
            var health = (Health)Entity.Data[Health.ComponentId];
            _dead = health.Current <= 0;

            HandleDeath();
        }

        void HandleDeath()
        {
            if (!_dead)
            {
                _deadTime = 0;
            }
            else if(_deadTime < 4f)
            {
                _deadTime += Time.deltaTime;
                _deadTime = Mathf.Min(_deadTime, 4f);
            }

            var smooth = 0.1f;

            var posX = Damp(0, _deadPosition.x, smooth, _deadTime);
            var posY = Damp(0, _deadPosition.y, smooth, _deadTime);
            var posZ = Damp(0, _deadPosition.z, smooth, _deadTime);

            _visual.transform.localPosition = new Vector3(posX, posY, posZ);

            var rotX = Damp(_startRot.x, _deadRotation.x, smooth, _deadTime);
            var rotY = Damp(_startRot.y, _deadRotation.y, smooth, _deadTime);
            var rotZ = Damp(_startRot.z, _deadRotation.z, smooth, _deadTime);

            _visual.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);

        }

        //https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
        // Smoothing rate dictates the proportion of source remaining after one second
        public static float Damp(float source, float target, float smoothing, float dt)
        {
            return Mathf.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
        }
    }
}
