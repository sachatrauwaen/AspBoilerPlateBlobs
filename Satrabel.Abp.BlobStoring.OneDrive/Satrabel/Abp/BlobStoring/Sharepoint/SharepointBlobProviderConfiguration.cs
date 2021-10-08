using Abp;
using System;
using System.Globalization;

namespace Satrabel.AspBoilerPlate.BlobStoring.SharePoint
{
    public class SharePointBlobProviderConfiguration
    {
        public string ClientId
        {
            get => _containerConfiguration.GetConfiguration<string>(SharePointBlobProviderConfigurationNames.ClientId);
            set => _containerConfiguration.SetConfiguration(SharePointBlobProviderConfigurationNames.ClientId, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string ClientSecret
        {
            get => _containerConfiguration.GetConfiguration<string>(SharePointBlobProviderConfigurationNames.ClientSecret);
            set => _containerConfiguration.SetConfiguration(SharePointBlobProviderConfigurationNames.ClientSecret, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string Instance
        {
            get => _containerConfiguration.GetConfiguration<string>(SharePointBlobProviderConfigurationNames.Instance);
            set => _containerConfiguration.SetConfiguration(SharePointBlobProviderConfigurationNames.Instance, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string Tenant
        {
            get => _containerConfiguration.GetConfiguration<string>(SharePointBlobProviderConfigurationNames.Tenant);
            set => _containerConfiguration.SetConfiguration(SharePointBlobProviderConfigurationNames.Tenant, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string ApiUrl
        {
            get => _containerConfiguration.GetConfiguration<string>(SharePointBlobProviderConfigurationNames.ApiUrl);
            set => _containerConfiguration.SetConfiguration(SharePointBlobProviderConfigurationNames.ApiUrl, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }


        /// <summary>
        /// This name may only contain lowercase letters, numbers, and hyphens, and must begin with a letter or a number.
        /// Each hyphen must be preceded and followed by a non-hyphen character.
        /// The name must also be between 3 and 63 characters long.
        /// If this parameter is not specified, the ContainerName of the <see cref="BlobProviderArgs"/> will be used.
        /// </summary>
        public string ContainerName
        {
            get => _containerConfiguration.GetConfigurationOrDefault<string>(SharePointBlobProviderConfigurationNames.ContainerName);
            set => _containerConfiguration.SetConfiguration(SharePointBlobProviderConfigurationNames.ContainerName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Default value: false.
        /// </summary>
        //public bool CreateContainerIfNotExists
        //{
        //    get => _containerConfiguration.GetConfigurationOrDefault(OneDriveBlobProviderConfigurationNames.CreateContainerIfNotExists, false);
        //    set => _containerConfiguration.SetConfiguration(OneDriveBlobProviderConfigurationNames.CreateContainerIfNotExists, value);
        //}


        public string Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, Instance, Tenant);
            }
        }

        private readonly BlobContainerConfiguration _containerConfiguration;

        public SharePointBlobProviderConfiguration(BlobContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
