using System;

namespace Satrabel.Abp.BlobStoring.Sharepoint
{
    public static class SharepointBlobContainerConfigurationExtensions
    {
        public static SharepointBlobProviderConfiguration GetOneDriveConfiguration(
            this BlobContainerConfiguration containerConfiguration)
        {
            return new SharepointBlobProviderConfiguration(containerConfiguration);
        }

        public static BlobContainerConfiguration UseOneDrive(
            this BlobContainerConfiguration containerConfiguration,
            Action<SharepointBlobProviderConfiguration> oneDriveConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(SharepointBlobProvider);
            containerConfiguration.NamingNormalizers.Add<SharepointBlobNamingNormalizer>();

            oneDriveConfigureAction(new SharepointBlobProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
