using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mmogf.Core
{
    public class BaseEntityBehavior : MonoBehaviour
    {
        public CommonHandler Server { get; set; }
        public EntityGameObject Entity { get; set; }
    }
}
