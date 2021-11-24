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
    public class CurrencyManagement : ICurrencyManagement
    {
        static HttpClient client = new HttpClient();
        string CurrencyURL = "https://nbg.gov.ge/gw/api/ct/monetarypolicy/currencies/ka/json";
        CurrencyDBContext db = new CurrencyDBContext();

        public List<string> FillDBWithNew(string user)
        {
            HttpResponseMessage response = client.GetAsync(CurrencyURL).Result;
            List<CurrencyRoot> ct = new List<CurrencyRoot>();
            if (response.IsSuccessStatusCode)
            {
                ct = JsonConvert.DeserializeObject<List<CurrencyRoot>>(response.Content.ReadAsStringAsync().Result);
            }
            List<CurrencyDTO> c = ct[0].currencies;
            List<Currency> curs = new List<Currency>();
            List<CurrencyChangeLog> logs = new List<CurrencyChangeLog>();
            List<string> updated = new List<string>();
            foreach (CurrencyDTO cdt in c)
            {
                if(db.Currencies.Any(i => i.code == cdt.code))
                {
                    var curr = db.Currencies.Where(i => i.code == cdt.code).First();

                    string diff = CheckDifferences(curr, cdt);
                    if (!diff.Equals("No Changes"))
                    {
                        logs.Add(new CurrencyChangeLog
                        {
                            User = user,
                            CurrencyName = cdt.code,
                            Updated_At = DateTime.Now,
                            Data = diff
                        });

                        curr.quantity = cdt.quantity;
                        curr.rateFormated = cdt.rateFormated;
                        curr.diffFormated = cdt.diffFormated;
                        curr.rate = cdt.rate;
                        curr.name = cdt.name;
                        curr.diff = cdt.diff;
                        curr.date = cdt.date;
                        curr.validFromDate = cdt.validFromDate;

                        updated.Add(cdt.code);
                    }
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
                db.Currencies.AddRange(curs);

            if (logs.Any())
                db.CurrencyChangeLogs.AddRange(logs);

            db.SaveChanges();
            return updated;
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
                
                string diff = CheckDifferences(curr, dt);
                if (!diff.Equals("No Changes"))
                {
                    CurrencyChangeLog log = new CurrencyChangeLog();
                    log.User = user;
                    log.CurrencyName = code;
                    log.Updated_At = DateTime.Now;
                    log.Data = diff;
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
                else
                {
                    return false;
                }
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

        public string CheckDifferences(Currency old, CurrencyDTO changed)
        {
            try
            {
                StringBuilder dat = new StringBuilder();

                dat.Append((old.quantity != changed.quantity) ? $"quantity:{old.quantity} -> {changed.quantity} " : "");
                dat.Append((old.rateFormated != changed.rateFormated) ? $"rateFormated:{old.rateFormated} -> {changed.rateFormated} " : "");
                dat.Append((old.diffFormated != changed.diffFormated) ? $"diffFormated:{old.diffFormated} -> {changed.diffFormated} " : "");
                dat.Append((old.rate != changed.rate) ? $"rate:{old.rate} -> {changed.rate} " : "");
                dat.Append((old.name != changed.name) ? $"name:{old.name} -> {changed.name} " : "");
                dat.Append((old.diff != changed.diff) ? $"diff:{old.diff} -> {changed.diff} " : "");
                dat.Append((old.date.Date != changed.date.Date) ? $"date:{old.date} -> {changed.date} " : "");
                dat.Append((old.validFromDate.Date != changed.validFromDate.Date) ? $"validFromDate:{old.validFromDate} -> {changed.validFromDate} " : "");

                if(dat.Length == 0)
                {
                    dat.Append("No Changes");
                }

                return dat.ToString();
            }
            catch
            {
                return "Error When Differentiating";
            }
            
        }
    }
}
