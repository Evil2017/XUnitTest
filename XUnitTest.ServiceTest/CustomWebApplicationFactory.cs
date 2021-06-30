using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using XUnitTest.Data;
using XUnitTest.Implement;
using XUnitTest.Interfaces;

namespace XUnitTest.ServiceTest
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));

                services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                //   services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb"));
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
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        //  Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }

}
