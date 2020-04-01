using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AsicServer.Core.Entities;
using AsicServer.Core.Models;
using AsicServer.Core.Utils;
using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using DataService.Repository;
using DataService.Service;
using DataService.UoW;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using DataService.Service.UserService;
using DataService.Service.RecordService;

namespace AsicServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private AppSettings appSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            SetupSwagger(services);
            SetupDbContext(services);
            SetupAutoMapper();
            SetupAuthentication(services);
            SetupDI(services);
            SetupFirebaseAuthentication(services);
        }

        private void SetupFirebaseAuthentication(IServiceCollection services)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromStream(File.OpenRead(appSettings.FirebaseConfigurationFile))
            });
        }

        private void SetupDbContext(IServiceCollection services)
        {
            services.AddDbContext<MainDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("AsicServerConn"));
            });
        }

        private void SetupSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Server ASIC API", Version = "v1" });

                var scheme = new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                };

                c.AddSecurityDefinition("Bearer", scheme);
                var openApiSecurityReq = new OpenApiSecurityRequirement();
                openApiSecurityReq.Add(scheme, new List<string>());
                c.AddSecurityRequirement(openApiSecurityReq);
            });
        }

        private void SetupAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<RegisteredUser, User>();
                cfg.CreateMap<User, UserViewModel>().ReverseMap();
            });
        }

        private void SetupAuthentication(IServiceCollection services)
        {
            const string scheme = JwtBearerDefaults.AuthenticationScheme;

            // Add authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = scheme;
                options.DefaultChallengeScheme = scheme;
                options.DefaultScheme = scheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = appSettings.Audience,
                    ValidIssuer = appSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(appSettings.SecretKey))
                };
            });
        }

        private void SetupDI(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton(Configuration);
            services.AddScoped<ExtensionSettings>();
            services.AddScoped<JwtTokenProvider>();
            services.AddScoped<UnitOfWork>();
            services.AddScoped<DbContext, MainDbContext>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRecordService, RecordService>();
            services.AddScoped<IRecordStagingRepository, RecordStagingRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors(options =>
            {
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server ASIC API"));

            app.UseRouting();
            app.UseAuthorization(); //the middleware is set between app.UseRouting and app.UseEndpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            loggerFactory.AddFile("Logs/server-log-{Date}.txt");

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseReactDevelopmentServer(npmScript: "start");
            //    }
            //});

        }
    }
}
