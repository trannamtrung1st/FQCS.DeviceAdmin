using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FQCS.DeviceAdmin.Business;
using FQCS.DeviceAdmin.Business.Helpers;
using FQCS.DeviceAdmin.Data;
using FQCS.DeviceAdmin.Data.Models;
using TNT.Core.Helpers.DI;
using FQCS.DeviceAdmin.WebApi.Policies;
using Microsoft.AspNetCore.Authorization;

namespace FQCS.DeviceAdmin.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            configuration.Bind("BusinessSettings", Business.Settings.Instance);
            configuration.Bind("WebApiSettings", WebApi.Settings.Instance);
        }

        public IConfiguration Configuration { get; }
        public static string WebRootPath { get; private set; }
        public static string MapPath(string path, string basePath = null)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = WebRootPath;
            }

            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(basePath, path);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            });
            services.AddServiceInjection();
            var connStr = Configuration.GetConnectionString("DataContext");
#if TEST
            connStr = connStr.Replace("{envConfig}", ".Test");
#else
            connStr = connStr.Replace("{envConfig}", "");
#endif
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connStr).UseLazyLoadingProxies());
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            Data.Global.Init(services);
            Business.Global.Init(services);
            #region OAuth
            services.AddIdentityCore<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            }).AddRoles<AppRole>()
            .AddDefaultTokenProviders()
            .AddSignInManager()
            .AddEntityFrameworkStores<DataContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            //required
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Business.Constants.JWT.ISSUER,
                        ValidAudience = Business.Constants.JWT.AUDIENCE,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.Default.GetBytes(Business.Constants.JWT.SECRET_KEY)),
                        ClockSkew = TimeSpan.Zero
                    };
                    //jwtBearerOptions.Events = new JwtBearerEvents
                    //{
                    //    OnMessageReceived = (context) =>
                    //    {
                    //        StringValues values;
                    //        if (!context.Request.Query.TryGetValue("access_token", out values))
                    //            return Task.CompletedTask;
                    //        var token = values.FirstOrDefault();
                    //        context.Token = token;
                    //        return Task.CompletedTask;
                    //    }
                    //};
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Policy.APP_CLIENT, policy =>
                    policy.Requirements.Add(new AppClientRequirement()));
            });
            services.AddScoped<IAuthorizationHandler, AppClientAuthHandler>();
            #endregion
            services.AddSingleton(new DefaultDateTimeModelBinder());
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new QueryObjectModelBinderProvider());
            }).AddNewtonsoftJson();
            services.AddSwaggerGenNewtonsoftSupport();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "My API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                var requirement = new OpenApiSecurityRequirement();
                requirement[new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                }] = new string[] { };
                c.AddSecurityRequirement(requirement);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            PrepareEnvironment(env);
            WebRootPath = env.WebRootPath;
            Settings.Instance.WebRootPath = env.WebRootPath;
            app.UseExceptionHandler($"/{Business.Constants.ApiEndpoint.ERROR}");
            app.UseStaticFiles();
            app.UseRouting();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
                builder.WithExposedHeaders(Microsoft.Net.Http.Headers.HeaderNames.ContentDisposition);
                //builder.AllowAnyOrigin();
                builder.SetIsOriginAllowed(origin =>
                {
                    return true;
                });
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void PrepareEnvironment(IWebHostEnvironment env)
        {
            var uploadPath = Settings.Instance.UploadFolderPath;
            uploadPath = Path.Combine(env.WebRootPath, uploadPath);
            Directory.CreateDirectory(uploadPath);
        }
    }
}
