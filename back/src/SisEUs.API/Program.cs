using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SisEUs.API.Token;
using SisEUs.Application.Comum.Configuracoes;
using SisEUs.Domain.Comum.Token;
using SisEUs.Infrastructure;
using SisEUs.Infrastructure.Migracao;
using SisEUs.Infrastructure.Repositorios;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var jwtSigningKey = configuration["Settings:Jwt:SigningKey"];

if (string.IsNullOrEmpty(jwtSigningKey))
{
    throw new InvalidOperationException("A chave de assinatura (Settings:Jwt:SigningKey) não foi configurada em appsettings.json.");
}

// Configuração de Geolocalização
builder.Services.Configure<GeolocalizacaoConfig>(
    configuration.GetSection(GeolocalizacaoConfig.SectionName));

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
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SisEUs API",
        Version = "v1",
        Description = "API para gerenciamento de eventos acadêmicos, presenças e avaliações",
        Contact = new OpenApiContact
        {
            Name = "SisEUs Team"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using Bearer scheme. 
                      Insira 'Bearer' [espaço] e então seu token na caixa de texto abaixo.
                      Exemplo: 'Bearer 12345abcdef'",
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

    // Habilita os comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Agrupa endpoints por Tags
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    options.DocInclusionPredicate((name, api) => true);
});

builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        logger.LogInformation("Verificando conexão com o banco de dados...");
        
        // Verifica se pode conectar ao banco
        var canConnect = await context.Database.CanConnectAsync();
        if (!canConnect)
        {
            logger.LogError("Não foi possível conectar ao banco de dados. Verifique a string de conexão.");
            throw new InvalidOperationException("Não foi possível conectar ao banco de dados.");
        }
        
        logger.LogInformation("Conexão estabelecida com sucesso!");
        logger.LogInformation("Aplicando migrations pendentes...");
        
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation($"Encontradas {pendingMigrations.Count()} migrations pendentes:");
            foreach (var migration in pendingMigrations)
            {
                logger.LogInformation($"  - {migration}");
            }
            
            logger.LogInformation("Aplicando migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations aplicadas com sucesso!");
        }
        else
        {
            logger.LogInformation("Nenhuma migration pendente encontrada.");
        }

        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Verificando necessidade de seed...");
            
            try
            {
                // Verifica se há usuários no banco
                var hasUsuarios = await context.Usuarios.AnyAsync();
                
                if (!hasUsuarios)
                {
                    logger.LogInformation("Executando seed do banco de dados...");
                    await InitBD.SeedAsync(context);
                    logger.LogInformation("Seed concluído com sucesso!");
                }
                else
                {
                    logger.LogInformation("Banco de dados já contém dados. Seed ignorado.");
                }
            }
            catch (Exception seedEx)
            {
                logger.LogWarning(seedEx, "Erro ao verificar/executar seed. Continuando a aplicação...");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao inicializar o banco de dados");
        
        // Se for ambiente de desenvolvimento, mostra mais detalhes
        if (app.Environment.IsDevelopment())
        {
            logger.LogError("Detalhes do erro: {Message}", ex.Message);
            logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
            
            if (ex.InnerException != null)
            {
                logger.LogError("Inner exception: {InnerMessage}", ex.InnerException.Message);
            }
            
            logger.LogInformation(@"
================================================================================
SOLUÇÃO DO PROBLEMA:
1. Exclua o banco de dados 'siseus' no MySQL
2. Execute os seguintes comandos no terminal:
   cd src/SisEUs.API
   dotnet ef database update
   
Ou recrie o banco manualmente com:
   DROP DATABASE IF EXISTS siseus;
   CREATE DATABASE siseus;
================================================================================
");
        }
        
        throw;
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