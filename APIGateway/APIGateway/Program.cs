using APIGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Učitaj URL-ove servisa
builder.Services.Configure<Urls>(builder.Configuration.GetSection("Urls"));

// Dodaj HttpClient za međuservisnu komunikaciju
builder.Services.AddHttpClient();

// Dodaj JWT autentifikaciju
var secretKey = "eqomOaEvtJ3Vn3LxnAyHvrcpNH4LQL7fbhrSqehXMeo="; // Tvoj tajni ključ
var issuer = "http://localhost:8080"; // Tvoj issuer
var audience = "api-users"; // Tvoja publika

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Dodaj kontrolere i Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger sa podrškom za JWT autentifikaciju
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Unesite JWT token u polju 'Authorization' koristeći prefiks 'Bearer'. \nPrimer: 'Bearer {tvoj-token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Swagger za razvojnu okolinu
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Uključi autentifikaciju i autorizaciju
app.UseAuthentication();
app.UseAuthorization();

// Ruta za generisanje JWT tokena
app.MapPost("/generate-token", (string username) =>
{
    if (string.IsNullOrWhiteSpace(username))
    {
        return Results.BadRequest("Korisničko ime je obavezno.");
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(secretKey);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }),
        Expires = DateTime.UtcNow.AddHours(1), // Vreme trajanja tokena
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var jwt = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = jwt });
});

// Mapiraj kontrolere
app.MapControllers();
app.Run();
