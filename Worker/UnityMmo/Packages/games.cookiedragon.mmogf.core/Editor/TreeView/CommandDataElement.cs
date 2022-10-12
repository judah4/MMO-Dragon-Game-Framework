using Mmogf.Core.Networking;
using System;
using System.Collections.Generic;
using UnityEditor.TreeViewExamples;
using UnityEngine;


namespace Mmogf.Unity.Editor
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


