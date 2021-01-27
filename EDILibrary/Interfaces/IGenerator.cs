// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using EDILibrary.Generatoren;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace EDILibrary.Interfaces
{
    public interface IGenerator
    {
        Task Generate(JToken jsonRoot, IGeneratorResolver resolver, NodeStack stack);
    }
}
