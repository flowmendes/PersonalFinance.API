using PersonalFinance.Api.Data;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Api.Services.Goals;
using PersonalFinance.Api.Services.Auth;
using PersonalFinance.Api.Services.Transactions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DE SERVIÇOS (DI) ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Registro de serviços
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IGoalServices, GoalServices>();
builder.Services.AddScoped<IFinancialService, FinancialService>();

// Configuração do DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
);

// Configuração de Autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Minha_Chave_Ultra_Secreta_Com_Mais_De_32_Caracteres_Esta_Aqui")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Configuração do Swagger com Cadeado
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Coloque o token assim: Bearer SEU_TOKEN_AQUI"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// --- 2. CONFIGURAÇÃO DO PIPELINE (MIDDLEWARES) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // 1º: Verifica quem você é (Token)
app.UseAuthorization();  // 2º: Verifica o que você pode fazer ([Authorize])

app.MapControllers();

app.Run();