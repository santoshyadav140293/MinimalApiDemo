using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MinimalAPIDemo;

public static class Api
{
  public static void ConfigureApi(this WebApplication app)
  {
    // All of API endpoint mapping
    app.MapGet("/Users", GetUsers);//.RequireAuthorization("Administrator");
    app.MapPost("/Users", GetUserInfo);
    //app.MapPost("/Users", CreateUser);
    app.MapPut("/Users", UpdateUser);
    app.MapDelete("/Users", DeleteUser);
  }

  private static async Task<IResult> GetUsers(IUserData data)
  {
    try
    {
      Console.WriteLine("Yay! I am able to be here.");
      //return Results.Ok("Santosh");
      return Results.Ok(await data.GetUsers());
    }
    catch (Exception ex)
    {
      return Results.Problem(ex.Message);
    }
  }

 
  private static async Task<IResult> GetUserInfo(UserModel user, IUserData data)
  {
    try
    {
      var results = await data.GetUserById(user.LoginId);

      if (results is null) return Results.NotFound();


      if (results.DeviceId == user.DeviceId)
      {
        return Results.Ok(results);
      }
      else
      {
        return Results.Problem("Invalid DeviceId");
      }

      //return Results.Ok(results);
    }
    catch (Exception ex)
    {
      return Results.Problem(ex.Message);
    }
  }

  private static async Task<IResult> CreateUser(UserModel user, IUserData data)
  {
    try
    {
      await data.CreateUser(user);
      return Results.Ok();
    }
    catch (Exception ex)
    {
      return Results.Problem(ex.Message);
    }
  }

  private static async Task<IResult> UpdateUser(UserModel user, IUserData data)
  {
    try
    {
      await data.UpdateUser(user);
      return Results.Ok();
    }
    catch (Exception ex)
    {
      return Results.Problem(ex.Message);
    }
  }

  private static async Task<IResult> DeleteUser(int id, IUserData data)
  {
    try
    {
      await data.DeleteUser(id);
      return Results.Ok();
    }
    catch (Exception ex)
    {
      return Results.Problem(ex.Message);
    }
  }


}
