using ClassFileBackEnd.Models;

namespace ClassFileBackEnd.Mapper
{
    public class ClassDTO
    {
        public int Id { get; set; }

        public string? ClassName { get; set; }

        public string? ImageCover { get; set; }

        public AccountProfileDTO? TeacherAccount { get; set; }

        public DateTime? LastPost { get; set; }

        public string? ClassCode { get; set; }

    }
}
