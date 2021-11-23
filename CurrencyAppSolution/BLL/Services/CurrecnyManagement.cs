using Newtonsoft.Json;
using BLL.Contracts;
using Models.DataViewModels.CurrencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DAL.EF;

namespace BLL.Services
{
    public class CurrecnyManagement : ICurrecnyManagement
    {
        static HttpClient client = new HttpClient();
        string CurrencyURL = "https://nbg.gov.ge/gw/api/ct/monetarypolicy/currencies/ka/json";
        CurrencyDBContext db = new CurrencyDBContext();
        public bool FillDBWithNew()
        {
            HttpResponseMessage response = client.GetAsync(CurrencyURL).Result;
            List<CurrencyRoot> ct = new List<CurrencyRoot>();
            if (response.IsSuccessStatusCode)
            {
                ct = JsonConvert.DeserializeObject<List<CurrencyRoot>>(response.Content.ReadAsStringAsync().Result);
            }
            List<CurrencyDTO> c = ct[0].currencies;

            foreach (CurrencyDTO cdt in c)
            {
                if(db.Currencies.Any(i => i.code == cdt.code))
                {
                    var curr = db.Currencies.Where(i => i.code == cdt.code).First();

                    curr.quantity = cdt.quantity;
                    curr.rateFormated = cdt.rateFormated;
                    curr.diffFormated = cdt.diffFormated;
                    curr.rate = cdt.rate;
                    curr.name = cdt.name;
                    curr.diff = cdt.diff;
                    curr.date = cdt.date;
                    curr.validFromDate = cdt.validFromDate;
                }
                else
                {
                    db.Currencies.Add(new Currency
                    {
                        code = cdt.code,
                        quantity = cdt.quantity,
                        rateFormated = cdt.rateFormated,
                        diffFormated = cdt.diffFormated,
                        rate = cdt.rate,
                        name = cdt.name,
                        diff = cdt.diff,
                        date = cdt.date,
                        validFromDate = cdt.validFromDate
                    });
                }
            }
            db.SaveChanges();
            return true;
        }
    }
}
