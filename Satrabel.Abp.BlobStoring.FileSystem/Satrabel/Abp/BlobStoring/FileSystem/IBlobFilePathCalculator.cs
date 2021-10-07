namespace Satrabel.AspBoilerPlate.BlobStoring.FileSystem
{
    public interface IBlobFilePathCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}