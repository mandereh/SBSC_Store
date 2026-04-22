using System.Text;
using Contracts;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Repository;
using SBSC_Store.Configurations;
using SBSC_Store.Swagger;
using Service.Contracts;
using Service;

namespace SBSC_Store.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) => services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    
    public static void ConfigureIISIntegration(this IServiceCollection services) => 
        services.Configure<IISOptions>(options => 
        { 
        });

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();
    
    public static void ConfigureRepositoryManager(this IServiceCollection services) => 
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) => 
        services.AddScoped<IServiceManager, ServiceManager>();

    public static void ConfigureFileServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
        services.Configure<StorageSettings>(configuration.GetSection("StorageSettings"));
        services.AddSingleton<IFileServiceFactory, FileServiceFactory>();
        services.AddSingleton<CloudinaryFileService>();
        services.AddSingleton<LocalFileService>();
    }
    
    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("sqlConnection"),
                b => b.MigrationsAssembly("SBSC_Store")));

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<User, IdentityRole>(o => 
            { 
                o.Password.RequireDigit = true; 
                o.Password.RequireLowercase = false; 
                o.Password.RequireUppercase = false; 
                o.Password.RequireNonAlphanumeric = false; 
                o.Password.RequiredLength = 8; 
                o.User.RequireUniqueEmail = true; 
            }) 
            .AddEntityFrameworkStores<RepositoryContext>() 
            .AddDefaultTokenProviders(); 
    }
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration 
        configuration) 
    { 
        var jwtSettings = configuration.GetSection("JwtSettings"); 
        var secretKey = Environment.GetEnvironmentVariable("SECRET"); 
 
        services.AddAuthentication(opt => 
            { 
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            }) 
            .AddJwtBearer(options => 
            { 
                options.TokenValidationParameters = new TokenValidationParameters() 
                { 
                    ValidateIssuer = true, 
                    ValidateAudience = true, 
                    ValidateLifetime = true, 
                    ValidateIssuerSigningKey = true, 
 
                    ValidIssuer = jwtSettings["validIssuer"], 
                    ValidAudience = jwtSettings["validAudience"], 
                    IssuerSigningKey = new 
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) 
                }; 
            }); 
    }
    
    public static void ConfigureSwagger(this IServiceCollection services)
        {
          services.AddSwaggerGen(s =>
          {
              s.SwaggerDoc("v1", new OpenApiInfo()
              {
                  Title = "SBSC STORE", 
                  Version = "v1",
                  Description = "Ecommerce store API by SBSC", 
                  TermsOfService = new Uri("https://example.com/terms"), 
                  Contact = new OpenApiContact 
                  { 
                      Name = "Daerego Braide", 
                      Email = "phronesis4xt@gmail.com", 
                      Url = new Uri("https://twitter.com/johndoe"), 
                  }, 
                  License = new OpenApiLicense 
                  { 
                      Name = "SBSC STORE API LICX", 
                      Url = new Uri("https://example.com/license"), 
                  } 
              });
              // s.SwaggerDoc("v2", new OpenApiInfo
              // {
              //     Title = "MainsquareCompanyEmployeeAPI", 
              //     Version = "v1",
              //     Description = "CompanyEmployees API by Mainsquare", 
              //     TermsOfService = new Uri("https://example.com/terms"), 
              //     Contact = new OpenApiContact 
              //     { 
              //         Name = "Daerego Braide", 
              //         Email = "mainsquarehq@gmail.com", 
              //         Url = new Uri("https://twitter.com/johndoe"), 
              //     }, 
              //     License = new OpenApiLicense 
              //     { 
              //         Name = "CompanyEmployees API LICX", 
              //         Url = new Uri("https://example.com/license"), 
              //     } 
              // });
              
              var xmlFile = $"{typeof(Presentation.AssemblyReference).Assembly.GetName().Name}.xml"; 
              var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile); 
              if (File.Exists(xmlPath))
                s.IncludeXmlComments(xmlPath); 
              
              
              s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
              { 
                  In = ParameterLocation.Header, 
                  Description = "Place to add JWT with Bearer", 
                  Name = "Authorization", 
                  Type = SecuritySchemeType.ApiKey, 
                  Scheme = "Bearer" 
              }); 
              s.AddSecurityRequirement(new OpenApiSecurityRequirement() 
              { 
                  { 
                      new OpenApiSecurityScheme 
                      { 
                          Reference = new OpenApiReference 
                          { 
                              Type = ReferenceType.SecurityScheme, 
                              Id = "Bearer"
                          }, 
                          Name = "Bearer", 
                      }, 
                      new List<string>() 
                  } 
              });
              // enable multipart/form-data form fields (files and simple fields)
              // s.OperationFilter<FileUploadOperationFilter>();
          });
        }
    

        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
            dbContext.Database.Migrate();
        }
    

}