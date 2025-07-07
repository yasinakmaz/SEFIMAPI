namespace SEFIMAPI.Data.Models
{
    public record class DirectTransaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        [Column("Total", TypeName = "decimal(10, 2)")]
        public decimal Total { get; set; } = 0.00m;
        public string? UserName { get; set; }
        public string? CustomerName { get; set; }
    }
}
