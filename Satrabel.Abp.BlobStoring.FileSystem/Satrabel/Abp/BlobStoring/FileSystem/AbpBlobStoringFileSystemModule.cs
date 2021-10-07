using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using System;
using System.Collections.Generic;
//using Volo.Abp.Modularity;

namespace Satrabel.Abp.BlobStoring.FileSystem
{
    [DependsOn(
        typeof(AbpBlobStoringModule)
        )]
    public class AbpBlobStoringFileSystemModule : AbpModule
    {
        public override void Initialize()
        {
        }

        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpBlobStoringFileSystemModule).GetAssembly());
            Configuration.BlobStoring().Providers.Add<FileSystemBlobProvider>();
        }
    }
}