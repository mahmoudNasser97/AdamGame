using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AISystem.Civil.Iterators.NodeSystem
{
    public interface Iterator 
    {
        public abstract int Key();

        public abstract NodeConnection Current();

        public abstract NodeConnection MoveBack(AIDataBoard databoard);

        public abstract NodeConnection MoveNext(AIDataBoard databoard);

        public abstract void AddCollection(NodeConnection[] nodesList);

        public abstract bool HasNext();

        public abstract void Reset();

        public abstract int GetLength();
    }

}
