using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLib.DbAccess;
using DataAccessLib.Models;

namespace DataAccessLib.Data;
public class UserData : IUserData
{
  private readonly ISqlDataAccess m_Sql;

  public UserData(ISqlDataAccess sql)
  {
    this.m_Sql = sql;
  }

  /// <summary>
  /// Get User info from Database
  /// </summary>
  /// <param name="Id">User Id</param>
  /// <returns></returns>
  public async Task<UserModel?> GetUserById(string Id)
  {
    var results = await m_Sql.LoadData<UserModel, dynamic>("dbo.spUser_Get", new { Id }, "Default");
    return results.FirstOrDefault();
  }

  public Task<IEnumerable<UserModel>> GetUsers() =>
    m_Sql.LoadData<UserModel, dynamic>("dbo.spUser_GetAll", new { }, "Default");

  public Task CreateUser(UserModel user) =>
    m_Sql.SaveData(
        "dbo.spUser_Insert",
        new
        {
          user.LoginId,
          user.DeviceId
        },
        "Default");

  public Task UpdateUser(UserModel user) =>
    m_Sql.SaveData(
        "dbo.spUser_Update",
        user,
        "Default");

  public Task DeleteUser(int id) =>
    m_Sql.SaveData("dbo.spUser_Delete", new { Id = id }, "Default");

}
