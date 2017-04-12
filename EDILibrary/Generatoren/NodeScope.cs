using EDILibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDILibrary.Generatoren
{
    public class NodeScope : INodeScope
    {

        public NodeScope(string node,int counter,int maxCounter)
        {
            Node = node;
            Counter = counter;
            MaxCounter = maxCounter;
        }
        public string Node
        {
            get;
            protected set;
        }

        public int Counter
        {
            get;
            set;
        }

        public int MaxCounter
        {
            get;
            protected set;
        }


        public void Dispose()
        {
        }
        public int Current
        {
            get
            {
                return Counter;
            }
        }
        object System.Collections.IEnumerator.Current
        {
            get
            {
                return Counter;
            }
        }

        public bool MoveNext()
        {
            if (Counter < MaxCounter-1)
            {
                Counter++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Counter = -1;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
