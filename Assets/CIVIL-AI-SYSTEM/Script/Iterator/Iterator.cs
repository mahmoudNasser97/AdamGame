using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem.Civil.Iterators
{
    public interface Iterator : IEnumerator
    {
        public abstract int Key();

        public new abstract object Current();

        public new abstract bool MoveNext();

        public new abstract void Reset();
    }
}