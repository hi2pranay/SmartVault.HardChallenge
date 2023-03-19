namespace SmartVault.DataGeneration.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public long Length { get; set; }
        public int AccountId { get; set; }
    }
}
