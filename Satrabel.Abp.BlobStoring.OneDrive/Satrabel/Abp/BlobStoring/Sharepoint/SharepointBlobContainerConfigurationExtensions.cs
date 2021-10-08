using System;

namespace Satrabel.AspBoilerPlate.BlobStoring.SharePoint
{
    public static class SharePointBlobContainerConfigurationExtensions
    {
        public static SharePointBlobProviderConfiguration GetSharePointConfiguration(
            this BlobContainerConfiguration containerConfiguration)
        {
            return new SharePointBlobProviderConfiguration(containerConfiguration);
        }

        public static BlobContainerConfiguration UseSharePoint(
            this BlobContainerConfiguration containerConfiguration,
            Action<SharePointBlobProviderConfiguration> sharePointConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(SharePointBlobProvider);
            containerConfiguration.NamingNormalizers.Add<SharePointBlobNamingNormalizer>();

            sharePointConfigureAction(new SharePointBlobProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
