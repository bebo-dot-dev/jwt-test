using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace jwt_test
{
    public class JwtBearerConfigureOptionsService : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public JwtBearerConfigureOptionsService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == JwtBearerDefaults.AuthenticationScheme)
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _httpContextAccessor.HttpContext.Request.Host.ToUriComponent(),
                    ValidAudience = JwtConstants.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtConstants.SuperSecretKey))
                };
            }
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(string.Empty, options);
        }
    }
}