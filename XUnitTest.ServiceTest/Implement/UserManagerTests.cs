using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using XUnitTest.Data;
using XUnitTest.Interfaces;
using XUnitTest.Models;

namespace XUnitTest.Implement.Tests
{
    [Collection("Database collection")]
    public class UserManagerTest //: IClassFixture<DatabaseFixture>
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
           .AddEnvironmentVariables()
           .Build();
        public UserManagerTest()
        {

        }




        [Theory]
        [InlineData("赵一", 11)]
        [InlineData("钱二", 22)]
        [InlineData("孙三", 33)]
        public async Task CreateUserWithDBTest(string name, int age)
        {

            IServiceCollection services = new ServiceCollection();
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connection));
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
            services.AddScoped<IUserService, UserSerivce>();

            //实例化 AutoFac  容器   
            var builder = new ContainerBuilder();
            //将services填充到Autofac容器生成器中
            builder.Populate(services);
            //使用已进行的组件登记创建新容器
            var ApplicationContainer = builder.Build();
            Random random = new Random();
            var user = new UserVm()
            {
                Name = name,
                Age = age
            };
            var userManager = ApplicationContainer.Resolve<IUserService>();
            var result = await userManager.AddAsync(user);
            Assert.NotNull(result);
            var u = await userManager.GetUserByName(name);

            Assert.Equal(user.Name, u.Name);
            Assert.Equal(user.Age, u.Age);
            var b = await userManager.ExistAsync(name);
            Assert.True(b);
            var count = await userManager.GetCountAsync(x => true);
            Assert.True(count > 0);
        }
    }
}