using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace Mmogf
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem Instance;
        [SerializeField]
        SmoothFollow _smoothFollow;
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetTarget(Transform target)
        {
            _smoothFollow.SetTarget(target);
        }
    }
}
