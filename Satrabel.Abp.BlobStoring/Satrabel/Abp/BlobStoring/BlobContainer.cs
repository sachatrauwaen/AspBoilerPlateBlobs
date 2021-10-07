using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp;
using Abp.Runtime.Session;
using Abp.Threading;


using Microsoft.Extensions.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.Threading;

namespace Satrabel.Abp.BlobStoring
{
    public class BlobContainer<TContainer> : IBlobContainer<TContainer>
        where TContainer : class
    {
        
        private readonly IBlobContainer _container;

        public BlobContainer(IBlobContainerFactory blobContainerFactory)
        {
            _container = blobContainerFactory.Create<TContainer>();
        }

        public Task SaveAsync(
            string name,
            Stream stream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _container.SaveAsync(
                name,
                stream,
                overrideExisting,
                cancellationToken
            );
        }

        public Task<bool> DeleteAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _container.DeleteAsync(
                name,
                cancellationToken
            );
        }

        public Task<bool> ExistsAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _container.ExistsAsync(
                name,
                cancellationToken
            );
        }

        public Task<Stream> GetAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _container.GetAsync(
                name,
                cancellationToken
            );
        }

        public Task<Stream> GetOrNullAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _container.GetOrNullAsync(
                name,
                cancellationToken
            );
        }

        public Task<List<string>> GetListAsync(string prefix, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _container.GetListAsync(
                prefix,
                cancellationToken
            );
        }
    }

    public class BlobContainer : IBlobContainer
    {
        protected string ContainerName { get; }

        protected BlobContainerConfiguration Configuration { get; }

        protected IBlobProvider Provider { get; }

        protected IAbpSession AbpSession { get; }

        //protected ICancellationTokenProvider CancellationTokenProvider { get; }

        protected IServiceProvider ServiceProvider { get; }

        
        public BlobContainer(
            string containerName,
            BlobContainerConfiguration configuration,
            IBlobProvider provider,
            IAbpSession abpSession,
            //ICancellationTokenProvider cancellationTokenProvider,
            IServiceProvider serviceProvider)
        {
            ContainerName = containerName;
            Configuration = configuration;
            Provider = provider;
            AbpSession = abpSession;
            //CancellationTokenProvider = cancellationTokenProvider;
            ServiceProvider = serviceProvider;
        }

        public virtual async Task SaveAsync(
            string name,
            Stream stream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (AbpSession.Use(GetTenantIdOrNull(), null))
            {
                var (normalizedContainerName, normalizedBlobName) = NormalizeNaming(ContainerName, name);

                await Provider.SaveAsync(
                    new BlobProviderSaveArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedBlobName,
                        stream,
                        overrideExisting//,
                        //CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }

        public virtual async Task<bool> DeleteAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (AbpSession.Use(GetTenantIdOrNull(),null))
            {
                var (normalizedContainerName, normalizedBlobName) =
                    NormalizeNaming(ContainerName, name);

                return await Provider.DeleteAsync(
                    new BlobProviderDeleteArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedBlobName//,
                        //CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }

        public virtual async Task<bool> ExistsAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (AbpSession.Use(GetTenantIdOrNull(), null))
            {
                var (normalizedContainerName, normalizedBlobName) =
                    NormalizeNaming(ContainerName, name);

                return await Provider.ExistsAsync(
                    new BlobProviderExistsArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedBlobName//,
                        //CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }

        public virtual async Task<Stream> GetAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stream = await GetOrNullAsync(name, cancellationToken);

            if (stream == null)
            {
                //TODO: Consider to throw some type of "not found" exception and handle on the HTTP status side
                throw new AbpException(
                    $"Could not found the requested BLOB '{name}' in the container '{ContainerName}'!");
            }

            return stream;
        }

        public virtual async Task<Stream> GetOrNullAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (AbpSession.Use(GetTenantIdOrNull(), null))
            {
                var (normalizedContainerName, normalizedBlobName) =
                    NormalizeNaming(ContainerName, name);
                
                return await Provider.GetOrNullAsync(
                    new BlobProviderGetArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedBlobName//,
                        //CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }

        protected virtual int? GetTenantIdOrNull()
        {
            if (!Configuration.IsMultiTenant)
            {
                return null;
            }

            return AbpSession.TenantId;
        }
        
        protected virtual (string, string) NormalizeNaming(string containerName,  string blobName)
        {
            if (!Configuration.NamingNormalizers.Any())
            {
                return (containerName, blobName);
            }

            using (var scope = ServiceProvider.CreateScope())
            {
                foreach (var normalizerType in Configuration.NamingNormalizers)
                {
                    var normalizer = scope.ServiceProvider
                        .GetRequiredService(normalizerType) as IBlobNamingNormalizer;

                    containerName = normalizer.NormalizeContainerName(containerName);
                    blobName = normalizer.NormalizeBlobName(blobName);
                }

                return (containerName, blobName);
            }
        }

        public virtual async Task<List<string>> GetListAsync(string prefix, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (AbpSession.Use(GetTenantIdOrNull(), null))
            {
                var (normalizedContainerName, normalizedBlobName) =
                    NormalizeNaming(ContainerName, prefix);

                return await Provider.GetListAsync(
                    new BlobProviderGetArgs(
                        normalizedContainerName,
                        Configuration,
                        normalizedBlobName//,
                                          //CancellationTokenProvider.FallbackToProvider(cancellationToken)
                    )
                );
            }
        }
    }
}
