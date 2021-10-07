//using Volo.Abp.Modularity;

using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Satrabel.Abp.BlobStoring.Azure
{
    [DependsOn(typeof(AbpBlobStoringModule))]
    public class AbpBlobStoringAzureModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpBlobStoringAzureModule).GetAssembly());
            Configuration.BlobStoring().Providers.Add<AzureBlobProvider>();
        }
    }
}
