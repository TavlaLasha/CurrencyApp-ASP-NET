using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DataViewModels.CurrencyManagement
{
    public class CurrencyDTO
    {
        public string code { get; set; }
        public int quantity { get; set; }
        public string rateFormated { get; set; }
        public string diffFormated { get; set; }
        public double rate { get; set; }
        public string name { get; set; }
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
