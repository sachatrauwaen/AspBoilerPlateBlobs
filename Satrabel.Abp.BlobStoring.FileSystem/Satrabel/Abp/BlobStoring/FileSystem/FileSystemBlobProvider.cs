using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.IO;
using Polly;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.IO;

namespace Satrabel.AspBoilerPlate.BlobStoring.FileSystem
{
    public class FileSystemBlobProvider : BlobProviderBase, ITransientDependency
    {
        protected IBlobFilePathCalculator FilePathCalculator { get; }

        public FileSystemBlobProvider(IBlobFilePathCalculator filePathCalculator)
        {
            FilePathCalculator = filePathCalculator;
        }

        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            if (!args.OverrideExisting && await ExistsAsync(filePath))
            {
                throw new BlobAlreadyExistsException($"Saving BLOB '{args.BlobName}' does already exists in the container '{args.ContainerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            DirectoryHelper.CreateIfNotExists(Path.GetDirectoryName(filePath));

            var fileMode = args.OverrideExisting
                ? FileMode.Create
                : FileMode.CreateNew;

            await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    using (var fileStream = File.Open(filePath, fileMode, FileAccess.Write))
                    {
                        await args.BlobStream.CopyToAsync(
                            fileStream//,
                            //args.CancellationToken
                        );

                        await fileStream.FlushAsync();
                    }
                });
        }

        public override Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            FileHelper.DeleteIfExists(filePath);
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            return ExistsAsync(filePath);
        }

        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return await Policy.Handle<IOException>()
                .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
                .ExecuteAsync(async () =>
                {
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var memoryStream = new MemoryStream();
                        await fileStream.CopyToAsync(memoryStream /*, args.CancellationToken*/);
                        return memoryStream;
                    }
                });
        }

        protected virtual Task<bool> ExistsAsync(string filePath)
        {
            return Task.FromResult(File.Exists(filePath));
        }

        public override async Task<List<string>> GetListAsync(BlobProviderGetArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            if (!Directory.Exists(filePath))
            {
                return new List<string>();
            }
            return Directory.GetFiles(filePath).ToList();
        }
    }
}
