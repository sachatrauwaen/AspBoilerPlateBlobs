namespace Satrabel.Abp.BlobStoring.Azure
{
    public interface IAzureBlobNameCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
