//using Volo.Abp.Modularity;

using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Satrabel.AspBoilerPlate.BlobStoring.SharePoint
{
    [DependsOn(typeof(AbpBlobStoringModule))]
    public class AbpBlobStoringSharePointModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpBlobStoringSharePointModule).GetAssembly());
            Configuration.BlobStoring().Providers.Add<SharePointBlobProvider>();
        }
    }
}
