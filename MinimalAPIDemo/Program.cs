using System.IdentityModel.Tokens.Jwt;
using System.Net;
using DataAccessLib.DbAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIDemo;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
  o.Authority = builder.Configuration["Jwt:Authority"];
  //o.Audience = builder.Configuration["Jwt:Audience"];
  o.RequireHttpsMetadata = false;
  o.TokenValidationParameters = new TokenValidationParameters()
  {
    //ValidAudience = builder.Configuration["Jwt:Audience"],
    ValidateAudience = false,
    ValidateIssuerSigningKey = false,
    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["Jwt:Authority"],
    ValidateLifetime = true
  };

  o.Events = new JwtBearerEvents();
  o.Events.OnChallenge = context =>
  {
    Console.WriteLine(context.Request.Headers["Authorization"]);
    // Skip the default logic.
    context.HandleResponse();

    var payload = new JObject
    {
      ["error"] = context.Error,
      ["error_description"] = context.ErrorDescription,
      ["error_uri"] = context.ErrorUri
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = 401;

    return context.Response.WriteAsync(payload.ToString());
  };

  o.Events.OnForbidden = c =>
  {
    //c.Response.StatusCode = 401;
    //c.Response.ContentType = "text/plain";
    //if (Environment.IsDevelopment())
    {
      return c.Response.WriteAsync(c.Result.Failure.ToString());
    }
    return c.Response.WriteAsync("An error occured processing your authentication.");
  };

});

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("Administrator", policy => policy.RequireClaim("user_roles", "developer"));
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddSingleton<IUserData, UserData>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.ConfigureApi();

app.Run();
