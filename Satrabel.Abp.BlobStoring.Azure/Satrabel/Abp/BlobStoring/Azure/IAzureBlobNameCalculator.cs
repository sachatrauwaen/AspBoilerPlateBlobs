namespace Satrabel.AspBoilerPlate.BlobStoring.Azure
{
    public interface IAzureBlobNameCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
