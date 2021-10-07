namespace Satrabel.AspBoilerPlate.BlobStoring.Sharepoint
{
    public interface ISharepointBlobNameCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
