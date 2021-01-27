// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

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
