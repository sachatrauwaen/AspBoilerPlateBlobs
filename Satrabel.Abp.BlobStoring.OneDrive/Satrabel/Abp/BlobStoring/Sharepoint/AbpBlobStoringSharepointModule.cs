//using Volo.Abp.Modularity;

using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Satrabel.Abp.BlobStoring.Sharepoint
{
    [DependsOn(typeof(AbpBlobStoringModule))]
    public class AbpBlobStoringSharepointModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpBlobStoringSharepointModule).GetAssembly());
            Configuration.BlobStoring().Providers.Add<SharepointBlobProvider>();
        }
    }
}
