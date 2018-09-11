using System;
using System.Text;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
using epm_api.Services;
using epm_api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace epm_api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        // ValidateIssuerSigningKey = true, sort later
                        // ValidIssuer = "yourdomain.com",
                        // ValidAudience = "yourdomain.com", sort later
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration["SecurityKey"])),
                    };
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IS3Service, S3Service>();
            services.AddSingleton<IDynamoDbService, DynamoDbService>();
            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IPackageService, PackageService>();
            services.AddSingleton<IVersionService, VersionService>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<ITeamService, TeamService>();
            // services.AddDefaultAWSOptions(Configuration.GetAWSOptions()); - want to get it working from development.json
            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonDynamoDB>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}
