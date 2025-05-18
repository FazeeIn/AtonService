using Aton.Api.middleware;
using Aton.Core.Models;
using Aton.Core.Validators;
using Aton.DataBase;
using Aton.DataBase.Abstractions;
using Aton.DataBase.Repositories;
using Aton.Services.Abstractions;
using Aton.Services.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<DataBaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IValidator<User>,UserValidator>();
builder.Services.AddScoped<IValidator<UpdateUser>,UpdateUserValidator>();
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.Run();