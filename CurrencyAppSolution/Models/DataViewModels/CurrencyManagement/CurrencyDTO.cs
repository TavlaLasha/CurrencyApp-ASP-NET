using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DataViewModels.CurrencyManagement
{
    public class CurrencyDTO
    {
        public string code { get; set; }
        [Required]
        public int quantity { get; set; }
        [Required]
        public string rateFormated { get; set; }
        [Required]
        public string diffFormated { get; set; }
        [Required]
        public double rate { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public double diff { get; set; }
        public DateTime date { get; set; }
        public DateTime validFromDate { get; set; }
    }
    public class CurrencyRoot
    {
        public DateTime date { get; set; }
        public List<CurrencyDTO> currencies { get; set; }
    }
}
