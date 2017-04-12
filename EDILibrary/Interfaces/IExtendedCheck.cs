namespace EDILibrary.Interfaces
{
    public interface IExtendedCheck
    {
        bool Check(IAPERAKExtensionPoint extension, EDIFileInfo fileInfo, IEdiObject vorgang,PathSelection selection);
        string Name { get; }
    }
}
