using System.Threading;
using JetBrains.Annotations;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public class BlobProviderExistsArgs : BlobProviderArgs
    {
        public BlobProviderExistsArgs(
            [NotNull] string containerName,
            [NotNull] BlobContainerConfiguration configuration,
            [NotNull] string blobName,
            CancellationToken cancellationToken = default(CancellationToken))
        : base(
            containerName,
            configuration,
            blobName,
            cancellationToken)
        {
        }
    }
}