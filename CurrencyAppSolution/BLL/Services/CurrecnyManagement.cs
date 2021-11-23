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
using System.Data.Entity;

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
            List<Currency> curs = new List<Currency>();
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
                    curs.Add(new Currency
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
            if (curs.Any())
            {
                db.Currencies.AddRange(curs);
            }
            db.SaveChanges();
            return true;
        }

        public bool EditCurrency(string code, string user, CurrencyDTO dt)
        {
            if (user.Equals(""))
                throw new Exception($"Forbidden request!");

            if (!db.Currencies.Any(i => i.code.Equals(code)))
                throw new Exception($"Currency with code {dt.code} not found!");
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                var curr = db.Currencies.Where(i => i.code.Equals(code)).First();

                CurrencyChangeLog log = new CurrencyChangeLog();

                log.User = user;
                log.CurrencyName = code;
                log.Updated_At = DateTime.Now;

                StringBuilder dat = new StringBuilder();

                dat.Append((curr.quantity != dt.quantity)? $"quantity:{curr.quantity} -> {dt.quantity}" : "");
                dat.Append((curr.rateFormated != dt.rateFormated) ? $"rateFormated:{curr.rateFormated} -> {dt.rateFormated}" : "");
                dat.Append((curr.diffFormated != dt.diffFormated) ? $"diffFormated:{curr.diffFormated} -> {dt.diffFormated}" : "");
                dat.Append((curr.rate != dt.rate) ? $"rate:{curr.rate} -> {dt.rate}" : "");
                dat.Append((curr.name != dt.name) ? $"name:{curr.name} -> {dt.name}" : "");
                dat.Append((curr.diff != dt.diff) ? $"diff:{curr.diff} -> {dt.diff}" : "");
                dat.Append((curr.date != dt.date) ? $"date:{curr.date} -> {dt.date}" : "");
                dat.Append((curr.validFromDate != dt.validFromDate) ? $"validFromDate:{curr.validFromDate} -> {dt.validFromDate}" : "");

                log.Data = dat.ToString();

                db.CurrencyChangeLogs.Add(log);

                curr.quantity = dt.quantity;
                curr.rateFormated = dt.rateFormated;
                curr.diffFormated = dt.diffFormated;
                curr.rate = dt.rate;
                curr.name = dt.name;
                curr.diff = dt.diff;
                curr.date = dt.date;
                curr.validFromDate = dt.validFromDate;

                db.SaveChanges();
                transaction.Commit();
            }
            return true;
        }

        public IEnumerable<CurrencyDTO> GetAllCurrencies()
        {
            return db.Currencies.Select(i => new CurrencyDTO
            {
                code = i.code,
                quantity = i.quantity,
                rateFormated = i.rateFormated,
                diffFormated = i.diffFormated,
                rate = i.rate,
                name = i.name,
                diff = i.diff,
                date = i.date,
                validFromDate = i.validFromDate
            });
        }

        public CurrencyDTO GetCurrency(string code)
        {
            return db.Currencies.Where(i=>i.code.Equals(code)).Select(i => new CurrencyDTO
            {
                code = i.code,
                quantity = i.quantity,
                rateFormated = i.rateFormated,
                diffFormated = i.diffFormated,
                rate = i.rate,
                name = i.name,
                diff = i.diff,
                date = i.date,
                validFromDate = i.validFromDate
            }).FirstOrDefault();
        }
    }
}
