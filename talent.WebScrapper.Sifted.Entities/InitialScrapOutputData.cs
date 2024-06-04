using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace talentX.WebScrapper.Sifted.Entities
{
    public class InitialScrapOutputData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Sectors { get; set; }
        public string? SectorUrl { get; set; }
    }
}
