using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using SBSC_Store.Extensions;
using Serilog;
using Serilog.Events;
using Presentation;

namespace SBSC_Store
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel
                .Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Filter.ByExcluding(logEvent =>
                {
                    if (!logEvent.Properties.TryGetValue("SourceContext", out var sourceContext))
                        return false;
                
                    var source = sourceContext.ToString();
                    return source.Contains("Microsoft.") ||
                           source.Contains("System.") ||
                           source.Contains("LuckyPennySoftware.");
                })
                .WriteTo.Console()
                .WriteTo.File("log/sbsc_store_logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.ConfigureCors();
            builder.Services.ConfigureIISIntegration();
            builder.Services.ConfigureLoggerService();
            builder.Services.ConfigureSqlContext(builder.Configuration);
            builder.Services.ConfigureRepositoryManager();
            builder.Services.ConfigureServiceManager();
            NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
            {
                var formatter = new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
                    .Services.BuildServiceProvider()
                    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
                    .OfType<NewtonsoftJsonPatchInputFormatter>().First();
                // Accept both application/json-patch+json (RFC standard) and plain application/json
                formatter.SupportedMediaTypes.Add(
                    Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json"));
                return formatter;
            }
            // builder.Services.Configure<ApiBehaviorOptions>(options => 
            // { 
            //     options.SuppressModelStateInvalidFilter = true; 
            // }); 
            builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);
            // builder.Services.AddControllers().AddXmlDataContractSerializerFormatters();
            builder.Services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            })
            .AddXmlDataContractSerializerFormatters()
            .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthentication();
            builder.Services.ConfigureIdentity();
            builder.Services.ConfigureJWT(builder.Configuration);
            // builder.Services.AddSwaggerGen();
            builder.Services.ConfigureSwagger();
            
            var app = builder.Build();
            
            var logger = app.Services.GetRequiredService<ILoggerManager>();
            app.ConfigureExceptionHandler(logger);
            if(app.Environment.IsProduction())
                app.UseHsts();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "SBSC STORE v1");
            });

            // if (app.Environment.IsDevelopment()) 
            //     app.UseDeveloperExceptionPage(); 
            // else 
            //     app.UseHsts(); 
            app.UseForwardedHeaders(new ForwardedHeadersOptions 
            { 
                ForwardedHeaders = ForwardedHeaders.All 
            }); 
            app.ApplyMigrations();
            app.UseHttpsRedirection();
            if (Directory.Exists(Path.Combine(app.Environment.ContentRootPath, "wwwroot")))
                app.UseStaticFiles();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
