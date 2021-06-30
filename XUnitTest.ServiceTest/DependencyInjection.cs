using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using XUnitTest.Data;
using XUnitTest.Implement;
using XUnitTest.Interfaces;
using XUnitTest.Mvc;

namespace XUnitTest.ServiceTest
{
    public static class DependencyInjection
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
      .AddEnvironmentVariables()
      .Build();
        public static Autofac.IContainer DICollections()
        {

            IServiceCollection services = new ServiceCollection();
            services.AddAutoMapper(typeof(Startup));

            // var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb"));
            // services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var basePath = AppContext.BaseDirectory;
            var servicesDllFile = Path.Combine(basePath, "XUnitTest.Mvc.dll");
            var assembly = Assembly.LoadFrom(servicesDllFile);
            var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(assembly); });
            Action<IMapperConfigurationExpression> action = cfg =>
            {
                // cfg.AddMaps(assembly);
                cfg.ForAllMaps((a, b) =>
                {
                    b.ForAllMembers(opt =>
                    {
                        opt.DoNotAllowNull();
                        //opt.Condition((src, dest, sourceMember) => sourceMember != null);
                    });
                });
            };
            services.AddAutoMapper(action, assembly);
            services.AddScoped<IUserService, UserService>();



            //实例化 AutoFac  容器   
            var builder = new ContainerBuilder();
            //builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();

            //指定已扫描程序集中的类型注册为提供所有其实现的接口。

            //builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();//注册仓储


            //var servicesDllFile = Path.Combine(basePath, "Blog.Core.Services.dll");
            //var assemblysServices = Assembly.LoadFrom(servicesDllFile);
            //builder.RegisterAssemblyTypes(assemblysServices)
            //             .AsImplementedInterfaces()
            //             .InstancePerLifetimeScope()
            //             .EnableInterfaceInterceptors();

            //var repositoryDllFile = Path.Combine(basePath, "Blog.Core.Repository.dll");
            //var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            //builder.RegisterAssemblyTypes(assemblysRepository).AsImplementedInterfaces();

            //将services填充到Autofac容器生成器中
            builder.Populate(services);

            //使用已进行的组件登记创建新容器
            var ApplicationContainer = builder.Build();

            return ApplicationContainer;
        }

    }
}
