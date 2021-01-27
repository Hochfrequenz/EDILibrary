// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using System.Collections.Generic;

namespace EDILibrary.Interfaces
{
    public interface INodeScope : IEnumerator<int>, IEnumerable<int>
    {
        string Node { get; }
        int Counter { get; set; }
        int MaxCounter { get; }
    }
}
