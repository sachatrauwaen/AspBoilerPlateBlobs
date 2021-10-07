using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.Threading;
using System;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.Threading;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public class BlobContainerFactory : IBlobContainerFactory, ITransientDependency
    {
        protected IBlobProviderSelector ProviderSelector { get; }
        
        protected IBlobContainerConfigurationProvider ConfigurationProvider { get; }

        protected IAbpSession AbpSession { get; }

        //protected ICancellationTokenProvider CancellationTokenProvider { get; }

        protected IServiceProvider ServiceProvider { get; }

        public BlobContainerFactory(
            IBlobContainerConfigurationProvider configurationProvider,
            IAbpSession abpSession,
            //ICancellationTokenProvider cancellationTokenProvider,
            IBlobProviderSelector providerSelector,
            IServiceProvider serviceProvider)
        {
            ConfigurationProvider = configurationProvider;
            AbpSession = abpSession;
            //CancellationTokenProvider = cancellationTokenProvider;
            ProviderSelector = providerSelector;
            ServiceProvider = serviceProvider;
        }

        public virtual IBlobContainer Create(string name)
        {
            var configuration = ConfigurationProvider.Get(name);

            return new BlobContainer(
                name,
                configuration,
                ProviderSelector.Get(name),
                AbpSession,
                //CancellationTokenProvider,
                ServiceProvider
            );
        }
    }
}
