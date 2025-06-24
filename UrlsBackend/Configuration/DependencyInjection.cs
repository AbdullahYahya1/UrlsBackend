using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UrlsBackend.Business.IService.cs;
using UrlsBackend.Business.Service.cs;
using UrlsBackend.Data.Context;
using UrlsBackend.Data.IRepositories;
using UrlsBackend.Data.Repositories;


public static class DependencyInjection
    {

        public static IServiceCollection AddRepositoriesInjections(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUrlRepository, UrlRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
        public static IServiceCollection AddServicesInjections(this IServiceCollection services)
        {
            //    services.AddScoped(typeof(IServicesDependency<>), typeof(ServicesDependency<>));
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUrlService, UrlService>();

        services.AddHttpClient();
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UrlsDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                };
             services.AddAuthorization();

                //options.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //    {
                //        var accessToken = context.Request.Query["access_token"];
                //        if (!string.IsNullOrEmpty(accessToken) &&
                //            context.HttpContext.Request.Path.StartsWithSegments("/userHub"))
                //        {
                //            context.Token = accessToken;
                //        }
                //        return Task.CompletedTask;
                //    }
                //};
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policyBuilder => policyBuilder.WithOrigins("http://localhost:4200/", "http://localhost:4200", "https://preeminent-pudding-02d0f8.netlify.app", "https://linguo.tech")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod()
                                                  .AllowCredentials());
            });
            return services;
        }
        //public static IServiceCollection AddJson(this IServiceCollection services)
        //{
        //    services.AddControllers().AddNewtonsoftJson(options =>
        //    {
        //        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //    });
        //    return services;
        //}
        //public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddHealthChecks()
        //     .AddSqlServer(configuration.GetConnectionString("DefaultConnection"))
        //     .AddDbContextCheck<ChatDpContext>();
        //    return services;
        //}


    }

