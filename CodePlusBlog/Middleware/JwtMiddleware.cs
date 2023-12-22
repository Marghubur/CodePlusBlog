using CodePlusBlog.Model;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServiceLayer.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CodePlusBlog.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SettingsConfigHelper.AppSetting("Secret"));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);
                var user = JsonConvert.DeserializeObject<User>(jwtToken.Claims.First(x => x.Type == "User").Value);


                // Attach user to context on successful validation
                context.Items["User"] = user;
            }
            catch
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }
        }
    }
}
