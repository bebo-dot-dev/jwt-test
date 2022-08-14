using System;
using System.Collections.Generic;
using jwt_test.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace jwt_test
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT test API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                                  Enter your token in the text input below",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
            
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            In = ParameterLocation.Header,
            
                        },
                        new List<string>()
                    }
                });
                
                c.DescribeAllParametersInCamelCase();
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                
                const string issuer = "https://xxxxxxxxxxxxxxxxxx/oauth2/xxxxxxxxxxxxxxxxx";
                
                options.MetadataAddress = $"{issuer}/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = "auth_server", //okta custom auth server name
                    ClockSkew = TimeSpan.FromMinutes(2),
                };
            });
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<HttpClientHeadersHandler>();
            services.AddHttpClient<IServiceClient, ServiceClient>(x =>
            {
                x.BaseAddress = new Uri(_configuration.GetValue<string>("Service:ServiceUrlBase"));
            });
            
            //register the HttpClientHeadersHandler for all typed http clients
            services.ConfigureAll<HttpClientFactoryOptions>(options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                {
                    builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<HttpClientHeadersHandler>());
                });
            });
            
            services.AddControllers();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}