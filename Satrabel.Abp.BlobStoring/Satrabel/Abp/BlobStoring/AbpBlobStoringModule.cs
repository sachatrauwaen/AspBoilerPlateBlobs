using Abp;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
//using Volo.Abp.Modularity;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.Threading;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    [DependsOn(
        //typeof(AbpMultiTenancyModule),
        //typeof(AbpThreadingModule)

        )]
    [DependsOn(typeof(AbpKernelModule))]
    [DependsOn(typeof(AbpWebCommonModule))]


    public class AbpBlobStoringModule : AbpModule
    {
        public override void Initialize()
        {

            
           

            //IocManager.Register(typeof(IBlobContainerConfigurationProvider), typeof(DefaultBlobContainerConfigurationProvider), DependencyLifeStyle.Transient);
            //IocManager.Register(typeof(IBlobProviderSelector), typeof(DefaultBlobProviderSelector), DependencyLifeStyle.Transient);
            //IocManager.Register(typeof(IBlobContainerFactory), typeof(BlobContainerFactory), DependencyLifeStyle.Transient);

            IocManager.Register(typeof(IBlobContainer<>), typeof(BlobContainer<>), DependencyLifeStyle.Transient);
            ////context.Services.AddTransient(
            ////    typeof(IBlobContainer<>),
            ////    typeof(BlobContainer<>)
            ////);


            var personService = IocManager.IocContainer.Resolve<IBlobContainer<DefaultContainer>>();

            IocManager.Register(typeof(IBlobContainer), personService.GetType()
                , DependencyLifeStyle.Transient);




            //IocManager.Register(typeof(IBlobContainer<>), typeof(BlobContainer<>), DependencyLifeStyle.Transient);
            ////context.Services.AddTransient(
            ////    typeof(IBlobContainer<>),
            ////    typeof(BlobContainer<>)
            ////);


            //var personService = IocManager.IocContainer.Resolve<IBlobContainer<DefaultContainer>>();

            //IocManager.Register(typeof(IBlobContainer), personService.GetType()
            //    , DependencyLifeStyle.Transient);


            //context.Services.AddTransient(
            //    typeof(IBlobContainer),
            //    serviceProvider => serviceProvider
            //        .GetRequiredService<IBlobContainer<DefaultContainer>>()
            //);
        }

        public override void PreInitialize()
        {
            //base.PreInitialize();

            IocManager.Register<IBlobStoringConfiguration, BlobStoringConfiguration>(DependencyLifeStyle.Singleton);

            IocManager.RegisterAssemblyByConvention(typeof(AbpBlobStoringModule).GetAssembly());

        }

        //public override void ConfigureServices(ServiceConfigurationContext context)
        //{
        //    context.Services.AddTransient(
        //        typeof(IBlobContainer<>),
        //        typeof(BlobContainer<>)
        //    );

        //    context.Services.AddTransient(
        //        typeof(IBlobContainer),
        //        serviceProvider => serviceProvider
        //            .GetRequiredService<IBlobContainer<DefaultContainer>>()
        //    );
        //}
    }
}