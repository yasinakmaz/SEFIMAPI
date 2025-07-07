namespace SEFIMAPI.Data.Models
{
    public record class Product
    {
        [Key]
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? ProductGroup { get; set; }
        public string? ProductCode { get; set; }
        public string? Order { get; set; }
        [Column("VatRate", TypeName = "decimal(10, 2)")]
        public decimal VatRate { get; set; }
        [Column("Price", TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; } = 0.00m;
        public string? ProductType { get; set; }
    }
}
