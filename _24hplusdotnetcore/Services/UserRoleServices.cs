using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace _24hplusdotnetcore.Services
{
    public class UserRoleServices
    {
        private readonly ILogger<UserLoginServices> _logger;
        private readonly IMongoCollection<UserRole> _userRole;
        public UserRoleServices(ILogger<UserLoginServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _userRole = database.GetCollection<UserRole>(Common.MongoCollection.UserRoles);
        }
        public List<UserRole> GetList()
        {
            var lstUserRole = new List<UserRole>();
            try
            {
                lstUserRole = _userRole.Find(u => true).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstUserRole;
        }
        public UserRole GetUserRoleByUserName(string userName)
        {
            var objUserRole = new UserRole();
            try
            {
                objUserRole = _userRole.Find(u => u.UserName == userName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objUserRole;
        }
        public List<UserRole> GetTeamMemberByTeamLead(string UserNameTeamLead)
        {
            var lstUserRole = new List<UserRole>();
            try
            {
                lstUserRole = _userRole.Find(u => u.TeamLead == UserNameTeamLead).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstUserRole;
        }
        public List<UserRole> GetTeamLeadByAdmin(string UserNameAdmin)
        {
            var lstUserRole = new List<UserRole>();
            try
            {
                lstUserRole = _userRole.Find(u => u.AdminCoordinator == UserNameAdmin).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstUserRole;
        }
        public UserRole CreateUserRole(UserRole userRole)
        {
            try
            {
                _userRole.InsertOne(userRole);
                return userRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}