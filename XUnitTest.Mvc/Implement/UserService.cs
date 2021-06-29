using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XUnitTest.Data;
using XUnitTest.Interfaces;
using XUnitTest.Models;

namespace XUnitTest.Implement
{

    public class UserService : IUserService
    {
        private readonly UserDbContext _dbContext;
        private IMapper _mapper;

        public UserService(UserDbContext dbContext, IMapper mapper)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public async Task<UserDto> AddAsync(UserVm vm)
        {
            if (vm.Age < 0 || string.IsNullOrEmpty(vm.Name))
            {
                throw new FormatException("Age or Name is invalid.");
            }
            else
            {
                var entity = _mapper.Map<UserEntity>(vm);
                _dbContext.Add(entity);
                var result = await _dbContext.SaveChangesAsync();
                var dto = _mapper.Map<UserDto>(entity);
                return dto;
            }
        }
        public async Task<UserDto> UpdateAsync(int? id, UserVm vm)
        {
            if (id.HasValue)
            {

                var entity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                {
                    _mapper.Map(vm, entity);
                    _dbContext.Update(entity);
                }
                else
                {
                    entity = _mapper.Map<UserEntity>(vm);
                    _dbContext.Add(entity);
                }
                var result = await _dbContext.SaveChangesAsync();
                var dto = _mapper.Map<UserDto>(entity);
                return dto;
            }
            else
            {
                return null;
            }
        }
        public async Task<UserEntity> GetUserByName(string name)
        {
            var entity = new UserEntity();
            if (!string.IsNullOrWhiteSpace(name))
            {
                entity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Name == name.Trim());
            }
            return entity;
        }

        public async Task<UserEntity> GetUserById(int? id)
        {
            var entity = new UserEntity();
            if (id.HasValue)
            {

                entity = await _dbContext.Users.FindAsync(id);
                return entity;
            }
            else
            {
                return entity;
            }
        }
        public async Task<int> GetCountAsync(Expression<Func<UserEntity, bool>> expression)
        {
            return await _dbContext.Users.AsQueryable().AsNoTracking().CountAsync(expression);
        }

        public async Task<List<UserEntity>> GetListAsync(Expression<Func<UserEntity, bool>> expression)
        {
            var query = _dbContext.Users.AsQueryable().AsNoTracking().Where(expression);
            var users = await query.ToListAsync();
            return users;
        }

        public async Task<UserEntity> FirstOrDefaultAsync(Expression<Func<UserEntity, bool>> expression)
        {
            var entity = await _dbContext.Users.FirstOrDefaultAsync(expression);
            return entity;
        }


        public async Task<UserEntity> RemoveAsync(int? id)
        {
            if (id.HasValue)
            {
                var entity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id.Value);
                _dbContext.Remove(entity);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {

                    return entity;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> ExistAsync(string name)
        {

            var result = await _dbContext.Users.AnyAsync(x => x.Name == name);
            return result;
        }
    }
}
