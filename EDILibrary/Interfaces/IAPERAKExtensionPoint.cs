// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

namespace EDILibrary.Interfaces
{
    public interface IAPERAKExtensionPoint
    {
        string getSparte(EDIFileInfo info); //return Strom / Gas
        string getEmpfängerRolle(EDIFileInfo info); // return VNB / LIEF / MSB / MDL
        string getAbsenderRolle(EDIFileInfo info); // return VNB / LIEF / MSB / MDL
    }
}
