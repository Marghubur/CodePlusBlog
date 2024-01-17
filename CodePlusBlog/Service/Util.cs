using CodePlusBlog.Model;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServiceLayer.Service
{
    public class Util
    {
        public static IConfiguration _configuration;

        public Util(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string EncryptPassword(string Password)
        {
            string encryptionKey = SettingsConfigHelper.AppSetting("Secret");
            byte[] clearBytes = Encoding.Unicode.GetBytes(Password);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x65, 0x4d, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms= new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    Password = Convert.ToBase64String(ms.ToArray());
                }
            }
            return Password;
        }

        public static string DeEncryptPassword(string Password)
        {
            string encryptionKey = SettingsConfigHelper.AppSetting("Secret");
            byte[] clearBytes = Convert.FromBase64String(Password);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x65, 0x4d, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    Password = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return Password;
        }

        public static string GenerateJWTToken(User user)
        {
            if (user.UsertypeId == 1)
                user.RoleName = "Admin";
            else
                user.RoleName = "User";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SettingsConfigHelper.AppSetting("Secret"));
            var tokenDescrpitor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleName),
                    new Claim("Id", user.UserId.ToString()),
                    new Claim("User", JsonConvert.SerializeObject(user)),
                }),
                Expires = user.RememberMe ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };
            var token = tokenHandler.CreateToken(tokenDescrpitor);
            return tokenHandler.WriteToken(token);
        }

        public static long? ValidateToken(string Token)
        {
            if (Token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SettingsConfigHelper.AppSetting("Secret"));
            try
            {
                tokenHandler.ValidateToken(Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwttoken = (JwtSecurityToken)validatedToken;
                var userId = long.Parse(jwttoken.Claims.First(x => x.Type == "Id").Value);
                return userId;
                    }
            catch (Exception)
            {

                return null;
            }
        }

        //public RefreshTokenModal CreateRefreshToken(User user)
        //{
        //    var tokenBytes = RandomNumberGenerator.GetBytes(64);
        //    var refreshToken = Convert.ToBase64String(tokenBytes);
        //    RefreshTokenModal refreshTokenModal = new RefreshTokenModal
        //    {
        //        RefreshToken = refreshToken,
        //        Email = user.Email,
        //        ExpiryTime = DateTime.UtcNow.AddDays(7),
        //        UserName = user.FullName
        //    };
        //    SaveRefreshToken(refreshTokenModal);
        //    return refreshTokenModal;
        //}

        //private string SaveRefreshToken(RefreshTokenModal refreshTokenModal)
        //{
        //    var result = string.Empty;
        //    if (refreshTokenModal != null)
        //    {
        //        result = _repository.Execute<RefreshTokenModal>("sp_refresh_token_insupd", refreshTokenModal, true);
        //        if (string.IsNullOrEmpty(result))
        //            throw new Exception("Fail to seve refresh token");
        //        return result;
        //    }
        //    return result;
        //}
    }
}
