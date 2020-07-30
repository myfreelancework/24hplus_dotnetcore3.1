using System.Collections.Generic;
using System.Linq;
using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace _24hplusdotnetcore.Services
{
    public class UserServices
    {
        private readonly ILogger<UserServices> _logger;
        private readonly IMongoCollection<User> _user;
        //private readonly IDataProtectionProvider _dataProtectionProvider;
        public UserServices(IMongoDbConnection connection, ILogger<UserServices> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _user = database.GetCollection<User>(MongoCollection.UsersCollection);
            _logger = logger;
           // _dataProtectionProvider = dataProtectionProvider;
        }

        public User Create(User user)
        {
            var userModel = new User();
            try
            {                
                userModel.UserName = user.UserName;
                userModel.UserFirstName = user.UserFirstName;
                userModel.UserMiddleName = user.UserMiddleName;
                userModel.UserLastName = user.UserLastName;
                userModel.UserEmail = user.UserEmail;
                userModel.UserPassword = user.UserPassword;//cipher.Encrypt(user.UserPassword);
                userModel.RoleName = user.RoleName;
                userModel.Phone = user.Phone;
                _user.InsertOne(userModel);
                _logger.LogInformation("Create user successfully: "+ JsonConvert.SerializeObject(userModel) +"");
                return userModel;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return userModel;
            }
            //CipherServices cipher = new CipherServices(_dataProtectionProvider);
           
        }

        public List<User> Get()
        {
           return _user.Find(user => true).ToList();
        }

        public User Get(string userName)
        {
            return _user.Find(user => user.UserName == userName).FirstOrDefault();
        }
    }
}
