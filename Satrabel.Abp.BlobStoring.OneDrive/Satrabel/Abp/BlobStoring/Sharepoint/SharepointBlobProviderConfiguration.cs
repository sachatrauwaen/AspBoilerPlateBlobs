using Abp;

namespace Satrabel.AspBoilerPlate.BlobStoring.Sharepoint
{
    public class SharepointBlobProviderConfiguration
    {
        public string ClientId
        {
            get => _containerConfiguration.GetConfiguration<string>(SharepointBlobProviderConfigurationNames.ClientId);
            set => _containerConfiguration.SetConfiguration(SharepointBlobProviderConfigurationNames.ClientId, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string ClientSecret
        {
            get => _containerConfiguration.GetConfiguration<string>(SharepointBlobProviderConfigurationNames.ClientSecret);
            set => _containerConfiguration.SetConfiguration(SharepointBlobProviderConfigurationNames.ClientSecret, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string Authority
        {
            get => _containerConfiguration.GetConfiguration<string>(SharepointBlobProviderConfigurationNames.Authority);
            set => _containerConfiguration.SetConfiguration(SharepointBlobProviderConfigurationNames.Authority, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        public string ApiUrl
        {
            get => _containerConfiguration.GetConfiguration<string>(SharepointBlobProviderConfigurationNames.ApiUrl);
            set => _containerConfiguration.SetConfiguration(SharepointBlobProviderConfigurationNames.ApiUrl, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }


        /// <summary>
        /// This name may only contain lowercase letters, numbers, and hyphens, and must begin with a letter or a number.
        /// Each hyphen must be preceded and followed by a non-hyphen character.
        /// The name must also be between 3 and 63 characters long.
        /// If this parameter is not specified, the ContainerName of the <see cref="BlobProviderArgs"/> will be used.
        /// </summary>
        public string ContainerName
        {
            get => _containerConfiguration.GetConfigurationOrDefault<string>(SharepointBlobProviderConfigurationNames.ContainerName);
            set => _containerConfiguration.SetConfiguration(SharepointBlobProviderConfigurationNames.ContainerName, Check.NotNullOrWhiteSpace(value, nameof(value)));
        }

        /// <summary>
        /// Default value: false.
        /// </summary>
        //public bool CreateContainerIfNotExists
        //{
        //    get => _containerConfiguration.GetConfigurationOrDefault(OneDriveBlobProviderConfigurationNames.CreateContainerIfNotExists, false);
        //    set => _containerConfiguration.SetConfiguration(OneDriveBlobProviderConfigurationNames.CreateContainerIfNotExists, value);
        //}

        private readonly BlobContainerConfiguration _containerConfiguration;

        public SharepointBlobProviderConfiguration(BlobContainerConfiguration containerConfiguration)
        {
            _containerConfiguration = containerConfiguration;
        }
    }
}
