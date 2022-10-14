#if UNITY_EDITOR

using Mmogf.Core.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Mmogf.Core.Editor
{

	[Serializable]
	public class CommandDataElement : TreeElement
    {
        public DataBucket Bucket;

        public CommandDataElement(string name, int depth, int id, DataBucket bucket) : base(name, depth, id)
        {
            Bucket = bucket;
        }
    }

}

#endif
