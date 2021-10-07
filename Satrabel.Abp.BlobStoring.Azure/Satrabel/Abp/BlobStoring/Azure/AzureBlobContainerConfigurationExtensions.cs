﻿using System;

namespace Satrabel.Abp.BlobStoring.Azure
{
    public static class AzureBlobContainerConfigurationExtensions
    {
        public static AzureBlobProviderConfiguration GetAzureConfiguration(
            this BlobContainerConfiguration containerConfiguration)
        {
            return new AzureBlobProviderConfiguration(containerConfiguration);
        }

        public static BlobContainerConfiguration UseAzure(
            this BlobContainerConfiguration containerConfiguration,
            Action<AzureBlobProviderConfiguration> azureConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(AzureBlobProvider);
            containerConfiguration.NamingNormalizers.Add<AzureBlobNamingNormalizer>();

            azureConfigureAction(new AzureBlobProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}
