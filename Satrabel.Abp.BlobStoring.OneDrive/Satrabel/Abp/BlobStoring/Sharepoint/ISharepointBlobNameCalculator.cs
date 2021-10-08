namespace Satrabel.AspBoilerPlate.BlobStoring.SharePoint
{
    public interface ISharePointBlobNameCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}
