using AdminTool.Data;
using AdminTool.Services;
using AdminTool.Services.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.ResponseCompression;
using AdminTool.Hubs;
using Akka.Actor;
using log4net.Config;
using log4net;
using System.Reflection;
using Library.Helper;


namespace AdminTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // log4net로그 - AssemblyInfo.cs
            var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepo, new FileInfo("log4net.config"));

            // Add configuration
            var configuration = builder.Configuration;

            // Configure JWT settings
            builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            builder.Configuration.AddEnvironmentVariables();

            var jwtSettings = new JwtSettings();
            configuration.Bind("JwtSettings", jwtSettings);

            if (string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new ArgumentNullException(nameof(jwtSettings.Key), "JWT key cannot be null or empty.");
            }

            var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddScoped<CustomAuthStateProvider>();

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                      new[] { "application/octet-stream" });
            });

            builder.Services.AddControllerServices();

            var configInstance = ConfigService.Instance;
            var https = configInstance.Https;
            var port = configInstance.Port;

            // Keep Alive시간 설정
            builder.Services.AddSignalR()
                .AddHubOptions<MessageHub>(options =>
                {
                    options.EnableDetailedErrors = true;
                    // 30초 초과시, 서버에서 연결을 끊어버림
                    options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
                })
                .AddHubOptions<ChatHub>(options =>
                {
                    options.EnableDetailedErrors = true;
                    // 30초 초과시, 서버에서 연결을 끊어버림
                    options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
                })
                .AddHubOptions<AiChatBotHub>(options =>
                {
                    options.EnableDetailedErrors = true;
                    // 30초 초과시, 서버에서 연결을 끊어버림
                    options.ClientTimeoutInterval = System.TimeSpan.FromSeconds(30);
                });

            // Akka.net 관련 추가
            builder.Services.AddSingleton<ActorSystem>(provider =>
            {
                var actorSystem = ActorSystem.Create("AdminToolSystem");
                return actorSystem;
            });
            builder.Services.AddHostedService<AkkaService>();

            // Configure Kestrel server to listen on a specific port
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(port);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (https)
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.UseResponseCompression();
            
            app.MapHub<ChatHub>("/chathub");
            app.MapHub<AiChatBotHub>("/aibothub"); // Ai Bot 처리
            app.MapHub<MessageHub>("/messagehub");

            app.Run();

        }
    }
}
