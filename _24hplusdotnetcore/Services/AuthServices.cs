using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace _24hplusdotnetcore.Services
{
    public class AuthServices
    {
        private readonly ILogger<AuthServices> _logger;
        private readonly IMongoCollection<User> _user;
        // private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IConfiguration _config;
        public AuthServices(IMongoDbConnection connection, IConfiguration config, ILogger<AuthServices> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _user = database.GetCollection<User>(MongoCollection.UsersCollection);
            //_dataProtectionProvider = dataProtectionProvider;
            _config = config;
            _logger = logger;
        }

        public AuthInfo Login(User user)
        {
            var authInfo = new AuthInfo();
            var loggedUser = AuthenticateUser(user);
            string token = string.Empty;
            if (loggedUser != null)
            {
                token = GenerateJSONWebToken(loggedUser);
                if (!string.IsNullOrEmpty(token))
                {
                    authInfo.UserFirstName = loggedUser.UserFirstName;
                    authInfo.UserMiddleName = loggedUser.UserMiddleName;
                    authInfo.UserLastName = loggedUser.UserLastName;
                    authInfo.UserName = loggedUser.UserName;
                    authInfo.RoleId = loggedUser.RoleName;
                    authInfo.token = token;
                    authInfo.RefreshToken = RandomString(50);
                }
            }
            return authInfo;

        }
        public ResponseLoginInfo LoginWithoutRefeshToken(RequestLoginInfo requestLoginInfo)
        {
            var resLogin = new ResponseLoginInfo();
            string token = string.Empty;
            try
            {
                var loginUser = _user.Find(u => u.UserName == requestLoginInfo.UserName && u.UserPassword == requestLoginInfo.Password).FirstOrDefault();
                if (loginUser != null)
                {
                    token = GenerateJSONWebTokenWithoutExpired(requestLoginInfo);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        resLogin.UserName = loginUser.UserName;
                        resLogin.UserFullName = loginUser.UserLastName + " " + loginUser.UserMiddleName + " " + loginUser.UserFirstName;
                        resLogin.Role = loginUser.RoleName;
                        resLogin.token = token;
                        resLogin.registration_token = requestLoginInfo.registration_token;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return resLogin;
        }
        private User AuthenticateUser(User user)
        {
            var loggedUser = new User();
            //CipherServices cipher = new CipherServices(_dataProtectionProvider);
            loggedUser = null;
            try
            {
                var loggedProcessUser = _user.Find(u => u.UserName == user.UserName).FirstOrDefault();
                if (loggedProcessUser != null)
                {
                    string descriptPassword = loggedProcessUser.UserPassword;//cipher.Decrypt(loggedProcessUser.UserPassword);
                    if (user.UserPassword == descriptPassword)
                    {
                        loggedUser = loggedProcessUser;
                    }

                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;

            }
            return loggedUser;
        }
        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.UserEmail),
                new Claim(JwtRegisteredClaimNames.GivenName, userInfo.UserFirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, userInfo.UserLastName + " " + userInfo.UserMiddleName),
                new Claim("Role", userInfo.RoleName.ToString()),
                new Claim(ClaimTypes.Role, userInfo.RoleName.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private string GenerateJSONWebTokenWithoutExpired(RequestLoginInfo requestLoginInfo)
        {
            string token = "";
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                        new Claim("UserName", requestLoginInfo.UserName),
                        new Claim("uuid", requestLoginInfo.uuid),
                        new Claim("ostype", requestLoginInfo.ostype)
                    };

                var tokenGenerated = new JwtSecurityToken(_config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    claims,
                    signingCredentials: credentials);
                token = new JwtSecurityTokenHandler().WriteToken(tokenGenerated);

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return token;
        }
    }
}