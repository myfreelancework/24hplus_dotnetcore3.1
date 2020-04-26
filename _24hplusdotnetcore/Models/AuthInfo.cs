namespace _24hplusdotnetcore.Models
{
    public class AuthInfo
    {
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserMiddleName { get; set; }
        public string UserLastName { get; set; }
        public string RoleId { get; set; }
        public string token { get; set; }
        public string RefreshToken { get; set; }
    }
}