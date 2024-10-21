using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem.Flowchart.V2
{
    public class PossibleWorkItem
    {
        public PossibleWorkItem(string id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public string id;
        public string name;
    }
}