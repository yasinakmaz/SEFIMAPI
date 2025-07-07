namespace SEFIMAPI.Data.Models
{
    public record class User
    {
        [Key]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

    }
}
