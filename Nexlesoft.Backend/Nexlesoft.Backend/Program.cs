using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nexlesoft.Application.Interfaces;
using Nexlesoft.Backend.Services.Interfaces;
using Nexlesoft.Backend.Services;
using Nexlesoft.Infrastructure.Data;
using Nexlesoft.Infrastructure.Repositories;
using System;
using static Nexlesoft.Infrastructure.Data.NexlesoftDbContext;
using Nexlesoft.Backend.Mappers;
using Microsoft.AspNetCore.Identity;
using Nexlesoft.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Nexlesoft.Backend.Helper;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddDbContext<NexlesoftDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                            new MySqlServerVersion(new Version(8, 0, 42)))
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors());

builder.Services.AddLogging(logging => logging.AddConsole());

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])), 
        ClockSkew= TimeSpan.Zero
    };   
});

// Register repository AND service ***
builder.AddInfrastructureServices();
// Register repository AND service ***

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Swagger config
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Test API",
        Version = "v1"
    });
});



// add controllers
builder.Services.AddControllers();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("backend/swagger/v1/swagger.json", "Entrance Test API v1");
    //    c.RoutePrefix = "";
    //});
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer>
        {
            new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/backend" }
        };
        });
    });

    // Enable middleware to serve Swagger UI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/backend/swagger/v1/swagger.json", "Entrance Test API v1");
        c.RoutePrefix = "swagger"; 
    });

    app.UseCors(x => x.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader());

}
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
