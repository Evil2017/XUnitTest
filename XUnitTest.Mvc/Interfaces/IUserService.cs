using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XUnitTest.Models;

namespace XUnitTest.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="vm">视图模型</param>
        /// <returns>添加进数据库后的传输类</returns>
        Task<UserDto> AddAsync(UserVm vm);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="vm">视图模型</param>
        /// <returns>更新后的传输类</returns>
        Task<UserDto> UpdateAsync(int? id, UserVm vm);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>被删除的实体</returns>
        Task<UserEntity> RemoveAsync(int? id);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>bool true 存在 false 不存在</returns>
        Task<bool> ExistAsync(string name);
        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>用户实体</returns>
        Task<UserEntity> GetUserById(int? id);
        /// <summary>
        /// 根据名称查询
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Task<UserEntity> GetUserByName(string name);
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="expression">条件</param>
        /// <returns>UserEntity 用户实体</returns>
        Task<UserEntity> FirstOrDefaultAsync(Expression<Func<UserEntity, bool>> expression);
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="expression">条件</param>
        /// <returns>List 用户集合</returns>
        Task<List<UserEntity>> GetListAsync(Expression<Func<UserEntity, bool>> expression);
        /// <summary>
        /// 统计数量
        /// </summary>
        /// <param name="expression">条件</param>
        /// <returns>int 数量</returns>
        Task<int> GetCountAsync(Expression<Func<UserEntity, bool>> expression);
    }
}