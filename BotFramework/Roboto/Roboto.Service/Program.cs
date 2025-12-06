using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Roboto.Repository;
using Roboto.Service.Auth;
using Roboto.Service.Dto;
using Roboto.Models;


var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext (Pomelo MySQL)
builder.Services.AddDbContext<RobotoDbContext>(options =>
    options.UseMySQL(configuration.GetConnectionString("MySql")?? throw new InvalidOperationException("Connection string 'MySql' not found.")));

// Services
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();

// JWT Auth
var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
var issuer = configuration["Jwt:Issuer"];
var audience = configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrEmpty(issuer),
            ValidIssuer = issuer,
            ValidateAudience = !string.IsNullOrEmpty(audience),
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Enable Swagger only in development (recommended)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/register", async (RegisterDto dto, RobotoDbContext db, IPasswordHasher hasher) =>
{
    // basic validation
    if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Email))
        return Results.BadRequest("username, email and password are required");

    var exists = await db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
    if (exists) return Results.Conflict("username or email already in use");

    var (hash, salt) = hasher.HashPassword(dto.Password);

    var user = new User
    {
        Username = dto.Username,
        Email = dto.Email,
        PasswordHash = hash,
        Salt = salt,
        CreatedAt = DateTime.UtcNow
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Username, user.Email });
});

app.MapPost("/api/auth/login", async (LoginDto dto, RobotoDbContext db, IPasswordHasher hasher, IJwtService jwt) =>
{
    var user = await db.Users
        .FirstOrDefaultAsync(u => u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

    if (user == null) return Results.Unauthorized();

    if (!hasher.VerifyPassword(dto.Password, user.PasswordHash, user.Salt))
        return Results.Unauthorized();

    var token = jwt.GenerateToken(user);

    return Results.Ok(new { token });
});

app.MapGet("/api/protected", [Microsoft.AspNetCore.Authorization.Authorize] () =>
{
    return Results.Ok(new { message = "You are authenticated." });
});

app.Run();
