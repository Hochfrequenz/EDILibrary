// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
namespace EDILibrary.Interfaces
{
    public interface IExtendedCheck
    {
        bool Check(IAPERAKExtensionPoint extension, EDIFileInfo fileInfo, IEdiObject vorgang, PathSelection selection);
        string Name { get; }
    }
}
