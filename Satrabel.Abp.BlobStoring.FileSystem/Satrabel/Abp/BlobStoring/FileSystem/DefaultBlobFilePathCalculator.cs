using Abp.Dependency;
using Abp.Runtime.Session;
using System.IO;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;

namespace Satrabel.AspBoilerPlate.BlobStoring.FileSystem
{
    public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator, ITransientDependency
    {
        protected IAbpSession AbpSession { get; }

        public DefaultBlobFilePathCalculator(IAbpSession abpSession)
        {
            AbpSession = abpSession;
        }
        
        public virtual string Calculate(BlobProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var blobPath = fileSystemConfiguration.BasePath;

            if (AbpSession.TenantId == null)
            {
                blobPath = Path.Combine(blobPath, "host");
            }
            else
            {
                blobPath = Path.Combine(blobPath, "tenants", AbpSession.TenantId.Value.ToString("D"));
            }

            if (fileSystemConfiguration.AppendContainerNameToBasePath)
            {
                blobPath = Path.Combine(blobPath, args.ContainerName);
            }
            
            blobPath = Path.Combine(blobPath, args.BlobName);

            return blobPath;
        }
    }
}