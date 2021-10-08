//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;

using Abp.Dependency;
using Abp.Runtime.Session;

namespace Satrabel.AspBoilerPlate.BlobStoring.SharePoint
{
    public class DefaultSharePointBlobNameCalculator : ISharePointBlobNameCalculator, ITransientDependency
    {
        protected IAbpSession AbpSession { get; }

        public DefaultSharePointBlobNameCalculator(IAbpSession abpSession)
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
