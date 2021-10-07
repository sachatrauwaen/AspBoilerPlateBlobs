//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;

using Abp.Dependency;
using Abp.Runtime.Session;

namespace Satrabel.Abp.BlobStoring.Azure
{
    public class DefaultAzureBlobNameCalculator : IAzureBlobNameCalculator, ITransientDependency
    {
        protected IAbpSession AbpSession { get; }

        public DefaultAzureBlobNameCalculator(IAbpSession abpSession)
        {
            AbpSession = abpSession;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            return AbpSession.TenantId == null
                ? $"host/{args.BlobName}"
                : $"tenants/{AbpSession.TenantId.Value.ToString("D")}/{args.BlobName}";
        }
    }
}
