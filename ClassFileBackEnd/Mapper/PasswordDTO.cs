using Microsoft.AspNetCore.Mvc;

namespace ClassFileBackEnd.Mapper
{
    public class PasswordDTO
    {

        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword
        {
            get; set;
        }
    }
}
