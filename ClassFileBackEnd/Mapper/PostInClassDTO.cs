using ClassFileBackEnd.Models;

namespace ClassFileBackEnd.Mapper
{
    public class PostInClassDTO
    {
        public int Id { get; set; }

        public int? ClassId { get; set; }

        public string? Title { get; set; }

        public int? PostedAccountId { get; set; }

        public AccountProfileDTO? PostedAccount { get; set; }

        public DateTime? DateCreated { get; set; }

        public virtual List<FileDTO> Files { get; } = new List<FileDTO>();

        public ClassInPostDTO? Class { get; set; }
    }
}
