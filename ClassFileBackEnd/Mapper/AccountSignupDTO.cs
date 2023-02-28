using System.ComponentModel.DataAnnotations;

namespace ClassFileBackEnd.Mapper
{
    public class AccountSignupDTO
    {
        private string? fullname;
        private string? username;
        private string? password;
        private string? password2;
        private string? accounttype;

        public string? FullName { get { return fullname; } set { fullname = value; } }
        public string? Username { get { return username; } set { username = value; } }
        public string? Password { get { return password; } set { password = value; } }
        public string? Password2 { get { return password2; } set { password2 = value; } }
        public string? AccountType { get { return accounttype; } set { accounttype = value; } }
    }
}
