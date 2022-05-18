
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebAppOSLERLib.Consts;
using WebAppOSLERLib.CTRL;

namespace WebAppOSLER
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppCtrl appctrl = new AppCtrl();
            string key = appctrl.JwtSecret;
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //what to validate
                        ValidateIssuer = true, // Validate the server that generates the token
                        ValidateAudience = true, //Validate the recipient of the token is authorized to receive
                        ValidateLifetime = true, //Check if the token is not expired and the signing key of the issuer is valid
                        ValidateIssuerSigningKey = true, //Validate signature of the token
                        //setup validate data
                        ValidIssuer = "sysadmin",
                        ValidAudiences = Constantes.DefaultNiveisAcessoText.ToList(),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

            services.AddControllers();
            
            services.AddCors(options =>
            {
                options.AddPolicy(
                    Constantes.MyCorsPolicy,
                    policy  =>
                    {
                        policy.WithOrigins(appctrl.CORSlist.Split(","))
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAppOSLER", Version = "v1" });
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = Constantes.CTKAuth.ToLower().Trim()
                };
                
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = Constantes.CTKAuth.Trim()
                            }
                        },
                        new string[] {}
                    }
                };

                c.AddSecurityDefinition(Constantes.CTKAuth.Trim(), jwtSecurityScheme);
                c.AddSecurityRequirement(securityRequirement);
                
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppOSLER v1"));
            }
            app.UsePreflightRequestHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseMvc();
            app.UseCors(Constantes.MyCorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}