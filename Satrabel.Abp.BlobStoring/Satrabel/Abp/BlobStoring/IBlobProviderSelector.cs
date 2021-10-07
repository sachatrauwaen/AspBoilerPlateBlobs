using JetBrains.Annotations;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public interface IBlobProviderSelector
    {
        [NotNull]
        IBlobProvider Get([NotNull] string containerName);
    }
}