using System;
using System.Collections.Generic;
using System.Linq;
using Abp;
using Abp.Dependency;
using Abp.Reflection;
using JetBrains.Annotations;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.DynamicProxy;

namespace Satrabel.Abp.BlobStoring
{
    public class DefaultBlobProviderSelector : IBlobProviderSelector, ITransientDependency
    {
        protected IBlobStoringConfiguration BlobStoringConfiguration { get; }

        protected IBlobContainerConfigurationProvider ConfigurationProvider { get; }

        private readonly IIocManager IocManager;

        public DefaultBlobProviderSelector(
            IIocManager iocManager,
            IBlobContainerConfigurationProvider configurationProvider, 
            IBlobStoringConfiguration blobStoringConfiguration)
        {
            IocManager = iocManager;
            ConfigurationProvider = configurationProvider;
            BlobStoringConfiguration = blobStoringConfiguration;
        }
        
        [NotNull]
        public virtual IBlobProvider Get([NotNull] string containerName)
        {
            Check.NotNull(containerName, nameof(containerName));
            var configuration = ConfigurationProvider.Get(containerName);
            if (!BlobStoringConfiguration.Providers.Any())
            {
                throw new AbpException("No BLOB Storage provider was registered! At least one provider must be registered to be able to use the Blog Storing System.");
            }
            foreach (var provider in BlobStoringConfiguration.Providers)
            {
                //if (ProxyHelper.GetUnProxiedType(provider).IsAssignableTo(configuration.ProviderType))
                //if (configuration.ProviderType.IsAssignableFrom(ProxyHelper.UnProxy(provider).GetType()))
                if (configuration.ProviderType.IsAssignableFrom(provider))
                {
                    return CreateProvider(provider);
                }
            }

            throw new AbpException(
                $"Could not find the BLOB Storage provider with the type ({configuration.ProviderType.AssemblyQualifiedName}) configured for the container {containerName} and no default provider was set."
            );
        }

        private IBlobProvider CreateProvider(Type providerType)
        {
            return IocManager.ResolveAsDisposable<IBlobProvider>(providerType).Object;
        }
    }
}