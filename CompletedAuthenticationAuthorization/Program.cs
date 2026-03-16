using AuthenticationAuthorization;
using AuthenticationAuthorization.Data;
using AuthenticationAuthorization.Interfaces;
using AuthenticationAuthorization.Middleware;
using AuthenticationAuthorization.Repositories;
using AuthenticationAuthorization.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Strongly-typed JWT config — single source of truth
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<EmailService>(); 

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();


app.UseMiddleware<GlobalExceptionMiddleware>(); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();