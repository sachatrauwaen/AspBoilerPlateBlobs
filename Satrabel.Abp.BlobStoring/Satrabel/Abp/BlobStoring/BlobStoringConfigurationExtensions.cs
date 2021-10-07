using Abp;
using Abp.Configuration.Startup;
using JetBrains.Annotations;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public static class BlobStoringConfigurationExtensions
    {
        public static IBlobStoringConfiguration BlobStoring(
            [NotNull] this IAbpStartupConfiguration configuration)
        {
            
            var blobConfig = configuration.IocManager.IocContainer.Resolve<IBlobStoringConfiguration>();

            return blobConfig;
        }
    }
}