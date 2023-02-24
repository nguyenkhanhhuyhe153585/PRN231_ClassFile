namespace ClassFileBackEnd.Mapper
{
    public class AccountDTO
    {
        private string? username;
        private string? password;

        public string? Username { get { return username; } set { username = value; } }
        public string? Password { get { return password; } set { password = value; } }
    }
}
