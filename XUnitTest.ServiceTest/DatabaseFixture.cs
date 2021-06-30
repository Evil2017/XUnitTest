﻿using AutoMapper;
using Enyim.Caching;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using XUnitTest.Data;
using XUnitTest.Implement;
using XUnitTest.Interfaces;
using XUnitTest.Mvc;

namespace XUnitTest.ServiceTest
{
    public class DatabaseFixture : IDisposable
    {
        public IServiceProvider ClientServices { get; private set; }
        public IServiceProvider ServerServices { get; private set; }
        private readonly TestServer _testServer;
        public DatabaseFixture()
        {

            IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder()
                .ConfigureLogging((logging) =>
                {
                    logging.AddConsole();
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IMemcachedClient, NullMemcachedClient>();
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb"));
                })
                .UseStartup<Startup>();

            _testServer = new TestServer(webHostBuilder);
            ServerServices = _testServer.Host.Services;
            ProvisionData(ServerServices.GetRequiredService<ApplicationDbContext>());
            ConfigureClientServices(_testServer.CreateClient());

        }



        private void ConfigureClientServices(HttpClient httpClient)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IMemcachedClient, NullMemcachedClient>();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton(httpClient);
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb"));
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



            services.AddSingleton<IUserService, UserService>();
            ClientServices = services.BuildServiceProvider();
        }

        private void ProvisionData(ApplicationDbContext dbContext)
        {
            //dbContext.Add();            
            dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _testServer.Dispose();
        }
    }
}
