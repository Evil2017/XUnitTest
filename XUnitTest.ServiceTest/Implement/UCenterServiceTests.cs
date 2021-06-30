using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using XUnitTest.Data;
using XUnitTest.Interfaces;
using XUnitTest.Models;
using XUnitTest.ServiceTest;

namespace XUnitTest.Implement.Tests
{
    [Collection("Database collection")]
    public class UCenterServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly IUserService _service;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UCenterServiceTests(DatabaseFixture fixture)
        {
            _service = fixture.ClientServices.GetService<IUserService>();
            _mapper = fixture.ClientServices.GetService<IMapper>();
            _dbContext = fixture.ServerServices.GetService<ApplicationDbContext>();
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


        [Theory]
        [ExcelData(fileName: @"TestCases/AdditionTestCase.xls")]
        public async Task AddAsyncTest(string name, int age)
        {
            // Arrange
            //var container = DependencyInjection.DICollections();
            //var _service = container.Resolve<IUserService>();
            UserVm viewmodel = new UserVm { Name = name, Age = age };
            // Act
            var oldCount = await _service.GetCountAsync(x => true);
            var dto = await _service.AddAsync(viewmodel);
            var list = await _service.GetListAsync(x => true);
            var newCount = await _service.GetCountAsync(x => true);

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
            //var container = DependencyInjection.DICollections();
            // var _service = container.Resolve<IUserService>();
            // var mapper = container.Resolve<IMapper>();
            UserVm viewmodel = new UserVm { Name = name, Age = age };
            // Act
            var list = await _service.GetListAsync(x => true);
            var entity = await _service.GetUserByName(name);
            // Assert
            Assert.NotNull(entity);
            _mapper.Map(viewmodel, entity);
            var newEntity = await _service.UpdateAsync(entity.Id, viewmodel);
            Assert.NotNull(newEntity);
            Assert.Equal(newEntity.Name, viewmodel.Name);
            Assert.Equal(newEntity.Age, viewmodel.Age);
        }
    }
}