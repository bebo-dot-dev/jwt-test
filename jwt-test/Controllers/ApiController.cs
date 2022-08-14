using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using jwt_test.Contract;
using jwt_test.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;

namespace jwt_test.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IServiceClient _serviceClient;
        public ApiController(IServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }
        
        [HttpGet("token/")]
        public JsonResult GetToken()
        {
            var strToken = GenerateToken(123, "Joe Bloggs", "joe.bloggs@jwtware.com");
            var validToken = ValidateToken(strToken);

            return new JsonResult(new
            {
                token = strToken,
                tokenValid = validToken.Key,
                tokenJson = System.Text.Json.JsonSerializer.Serialize(validToken.Value)
            });
        }

        [Authorize]
        [HttpGet("secured/value1/{id:int}/value2/{id2:int}")]
        public async Task<JsonResult> CallTokenSecuredEndpoint([FromRoute] int id, int id2, [FromQuery] RequestModel request)
        {
            var response = await _serviceClient.DoGet(request);
            response = await _serviceClient.DoPost(request);
            
            var claimsList = User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList();
            return new JsonResult(new
            {
                HappyMessage = "Hurray you are authorized to call this endpoint",
                ParamsPassedMessage = "You passed in:",
                Id = id,
                Id2 = id2,
                Request = request,
                ClaimsMessage = "These are your JWT claims:",
                Claims = claimsList
            });
        }
        
        private string GenerateToken(int userId, string userName, string emailAddress)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtConstants.SuperSecretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", userId.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userName),
                    new Claim(ClaimTypes.Email, emailAddress),
                    new Claim(ClaimTypes.Role, "AD Group 1"),
                    new Claim(ClaimTypes.Role, "AD Group 2")
                    //add more claims as required for user context
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(8), //limit the validity of the token to 8 hrs
                Issuer = HttpContext.Request.Host.ToUriComponent(), //make the calling application the issuer of the token
                Audience = JwtConstants.Audience, //limit the audience of this token to the calling application (applicationId, similar to okta clientId)
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private KeyValuePair<bool, SecurityToken?> ValidateToken(string token)
        {
            SecurityToken outToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = HttpContext.Request.Host.ToUriComponent(),
                    ValidAudience = JwtConstants.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtConstants.SuperSecretKey))
                }, out outToken);
            }
            catch
            {
                return new KeyValuePair<bool, SecurityToken?>(false, null);
            }

            return new KeyValuePair<bool, SecurityToken?>(true, outToken);
        }
    }
}