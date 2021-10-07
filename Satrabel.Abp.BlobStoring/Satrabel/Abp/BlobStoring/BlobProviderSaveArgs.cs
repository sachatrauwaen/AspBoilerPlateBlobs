using System.IO;
using System.Threading;
using Abp;
using JetBrains.Annotations;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public class BlobProviderSaveArgs : BlobProviderArgs
    {
        [NotNull]
        public Stream BlobStream { get; }
        
        public bool OverrideExisting { get; }

        public BlobProviderSaveArgs(
            [NotNull] string containerName,
            [NotNull] BlobContainerConfiguration configuration,
            [NotNull] string blobName,
            [NotNull] Stream blobStream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default(CancellationToken))
            : base(
                containerName,
                configuration,
                blobName,
                cancellationToken)
        {
            BlobStream = Check.NotNull(blobStream, nameof(blobStream));
            OverrideExisting = overrideExisting;
        }
    }
}