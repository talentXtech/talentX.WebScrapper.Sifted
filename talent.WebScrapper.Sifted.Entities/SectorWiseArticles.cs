using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talentX.WebScrapper.Sifted.Entities
{
    public class SectorWiseArticles
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Sectors { get; set; }

        public string? SectorUrl { get; set; }
        public string? ArticleUrl { get; set; }
    }
}
