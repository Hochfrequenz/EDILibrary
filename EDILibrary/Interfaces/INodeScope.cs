using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDILibrary.Interfaces
{
    public interface INodeScope : IEnumerator<int>, IEnumerable<int>
    {
        string Node { get; }
        int Counter { get; set; }
        int MaxCounter { get; }
    }
}
