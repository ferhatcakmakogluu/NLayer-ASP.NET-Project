using Autofac;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Repository;
using NLayer.Repository.Repositories;
using NLayer.Repository.UnitOfWorks;
using NLayer.Service.Mapping;
using NLayer.Service.Services;
using System.Reflection;
using Module = Autofac.Module;

namespace NLayer.Web.Modules
{
    public class RepoServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //sadece bir tane olan Generic olanları manuel tek tek ekledik
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Service<>)).As(typeof(IService<>)).InstancePerLifetimeScope();

            //Sdece bir tane olan ama generic olmayanı manuel ekledik
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            //tüm assembly leri al (katmanlar)
            var apiAssembly = Assembly.GetExecutingAssembly(); //mevcutta calısılan assembly
            var repoAssembly = Assembly.GetAssembly(typeof(AppDbContext));
            var serviceAssembly = Assembly.GetAssembly(typeof(MapProfile));


            //AsImplementedInterfaces() => interfaceleri de al
            //InstancePerLifetimeScope() => AddScoped anlamında
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly)
                .Where(x=>x.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly)
                .Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope();


            //builder.RegisterType<ProductService>().As<IProductService>();
        }
    }
}
