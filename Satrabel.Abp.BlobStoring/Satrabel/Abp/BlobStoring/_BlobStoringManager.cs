using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Satrabel.Abp.BlobStoring
{
    internal class BlobStoringManager : IBlobStoringManager, ISingletonDependency
    {
        private readonly IIocManager _iocManager;
        private readonly IBlobStoringConfiguration _blobStoringConfiguration;
        //private readonly IDictionary<string, SettingDefinition> _settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BlobStoringManager(IIocManager iocManager, IBlobStoringConfiguration blobStoringConfiguration)
        {
            _iocManager = iocManager;
            _blobStoringConfiguration = blobStoringConfiguration;
            //_settings = new Dictionary<string, SettingDefinition>();
        }

        public void Initialize()
        {
            //var context = new SettingDefinitionProviderContext(this);

            foreach (var providerType in _blobStoringConfiguration.Providers)
            {
                using (var provider = CreateProvider(providerType))
                {
                    //foreach (var settings in provider.Object.GetSettingDefinitions(context))
                    //{
                    //    _settings[settings.Name] = settings;
                    //}
                }
            }
        }

        //public SettingDefinition GetSettingDefinition(string name)
        //{
        //    if (!_settings.TryGetValue(name, out var settingDefinition))
        //    {
        //        throw new AbpException("There is no setting defined with name: " + name);
        //    }

        //    return settingDefinition;
        //}

        //public IReadOnlyList<SettingDefinition> GetAllSettingDefinitions()
        //{
        //    return _settings.Values.ToImmutableList();
        //}

        private IDisposableDependencyObjectWrapper<IBlobProvider> CreateProvider(Type providerType)
        {
            return _iocManager.ResolveAsDisposable<IBlobProvider>(providerType);
        }
    }
}
