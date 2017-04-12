// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDILibrary.Interfaces
{
    public interface IGeneratorResolver
    {
        Task<object> GetParameterAsync(string name);
        Task<T> GetParameterAsync<T>(string name);
        int DetermineCommonPath(string name);
    }
}
