using XUnitTest.Implement;
using Autofac;
using System.Threading.Tasks;
using Xunit;
using XUnitTest.Interfaces;
using XUnitTest.Models;
using XUnitTest.ServiceTest;
using AutoMapper;
using XUnitTest.Data;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace XUnitTest.Implement.Tests
{
    [Collection("Database collection")]
    public class UserSerivceTests
    {
        [Theory]
        [ExcelData(fileName: @"TestCases/AdditionTestCase.xls")]
        public async Task AddAsyncTest(string name, int age)
        {
            // Arrange
            var container = DependencyInjection.DICollections();
            var service = container.Resolve<IUserService>();
            UserVm viewmodel = new UserVm { Name = name, Age = age };
            // Act
            var oldCount = await service.GetCountAsync(x => true);
            var dto = await service.AddAsync(viewmodel);
            var list = await service.GetListAsync(x => true);
            var newCount = await service.GetCountAsync(x => true);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(dto.Name, viewmodel.Name);
            Assert.Equal(dto.Age, viewmodel.Age);
            Assert.True(newCount - oldCount == 1);
        }

        [Theory]
        [ExcelData(fileName: @"TestCases/UpdateTestCase.xls")]
        public async Task UpdateAsyncTest(string name, int age)
        {
            // Arrange
            var container = DependencyInjection.DICollections();
            var service = container.Resolve<IUserService>();
            var mapper = container.Resolve<IMapper>();
            UserVm viewmodel = new UserVm { Name = name, Age = age };
            // Act
            var list = await service.GetListAsync(x => true);
            var entity = await service.GetUserByName(name);
            // Assert
            Assert.NotNull(entity);
            mapper.Map(viewmodel, entity);
            var newEntity = await service.UpdateAsync(entity.Id, viewmodel);
            Assert.NotNull(newEntity);
            Assert.Equal(newEntity.Name, viewmodel.Name);
            Assert.Equal(newEntity.Age, viewmodel.Age);
        }
    }

    public class UCenterServiceTests : IClassFixture<WebApiTestFixture>
    {
        private readonly IUserService _service;
        private readonly UserDbContext _dbContext;

        public UCenterServiceTests(WebApiTestFixture fixture)
        {
            _service = fixture.ClientServices.GetService<IUserService>();
            _dbContext = fixture.ServerServices.GetService<UserDbContext>();
        }

        [Fact]
        public async Task GetUserTest()
        {
            // Arrage
            var fakeUser = await _dbContext.Users.FirstOrDefaultAsync();
            // Act
            var user = await _service.GetUserByName(fakeUser.Name);

            // Assert
            Assert.NotNull(user);
        }
    }
}