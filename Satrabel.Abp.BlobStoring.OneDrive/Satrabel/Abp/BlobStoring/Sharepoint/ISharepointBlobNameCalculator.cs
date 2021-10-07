namespace Satrabel.Abp.BlobStoring.Sharepoint
{
    public interface ISharepointBlobNameCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
