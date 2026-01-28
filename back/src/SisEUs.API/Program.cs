using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SisEUs.API.Token;
using SisEUs.Application;
using SisEUs.Domain.Comum.Token;
using SisEUs.Infrastructure;
using SisEUs.Infrastructure.Migracao;
using System.Text;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Reflection; 
using SisEUs.Apresentation.Authenticacoes; 
using SisEUs.Apresentation.Checkin;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var jwtSigningKey = configuration["Settings:Jwt:SigningKey"];

if (string.IsNullOrEmpty(jwtSigningKey))
{
    throw new InvalidOperationException("A chave de assinatura (Settings:Jwt:SigningKey) não foi configurada em appsettings.json.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
            
            ValidateIssuer = false,
            ValidIssuer = configuration["Settings:Jwt:Issuer"],

            ValidateAudience = false,
            ValidAudience = configuration["Settings:Jwt:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
});

builder.Services.AddControllers(); 

builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicyDevelopment", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });

    options.AddPolicy("CorsPolicyProduction", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000")
                     .WithMethods("GET", "POST", "PUT", "DELETE")
                     .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Aguardar o banco de dados estar pronto e inicializar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<SisEUs.Infrastructure.Repositorios.AppDbContext>();

    var maxRetries = 30;
    var delay = TimeSpan.FromSeconds(3);
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Tentando conectar ao banco de dados... (Tentativa {Attempt}/{MaxRetries})", i + 1, maxRetries);
            
            // Testa a conexão
            await context.Database.CanConnectAsync();
            
            logger.LogInformation("Conexão com o banco de dados estabelecida com sucesso!");
            
            // Aguarda mais um pouco para garantir que o banco está completamente pronto
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            // Recria o banco de dados
            logger.LogInformation("Recriando o banco de dados...");
            context.Database.EnsureDeleted(); 
            context.Database.EnsureCreated();
            
            // Popula dados iniciais
            logger.LogInformation("Populando dados iniciais...");
            await InitBD.SeedAsync(context);
            
            logger.LogInformation("Banco de dados inicializado com sucesso!");
            break;
        }
        catch (Exception ex)
        {
            if (i == maxRetries - 1)
            {
                logger.LogError(ex, "Não foi possível conectar ao banco de dados após {MaxRetries} tentativas.", maxRetries);
                throw;
            }
            
            logger.LogWarning(ex, "Falha ao conectar ao banco de dados. Aguardando {Delay} segundos antes de tentar novamente...", delay.TotalSeconds);
            await Task.Delay(delay);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("CorsPolicyDevelopment");
}
else
{
    app.UseCors("CorsPolicyProduction");
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();