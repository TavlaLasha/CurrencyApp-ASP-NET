using Models.DataViewModels.CurrencyManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CurrencyApp.Controllers
{
    public class HomeController : Controller
    {
        static HttpClient client = new HttpClient();
        string BaseURL = ConfigurationManager.AppSettings["CurrencyService"];

        public ActionResult Index()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync($"{BaseURL}/GetAll").Result;
                List<CurrencyDTO> ct = new List<CurrencyDTO>();
                if (response.IsSuccessStatusCode)
                {
                    ct = JsonConvert.DeserializeObject<List<CurrencyDTO>>(response.Content.ReadAsStringAsync().Result);
                }
                return View(ct);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult FillDBWithNew(string user)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync($"{BaseURL}/FillDBWithLatest/{user}").Result;
                List<string> updated = new List<string>();
                if (response.IsSuccessStatusCode)
                {
                    updated = JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);
                }
                TempData["codes"] = updated;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string code)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync($"{BaseURL}/GetCurrency/{code}").Result;
                CurrencyDTO ct = new CurrencyDTO();
                if (response.IsSuccessStatusCode)
                {
                    ct = JsonConvert.DeserializeObject<CurrencyDTO>(response.Content.ReadAsStringAsync().Result);
                }
                return View(ct);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult();
            }
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(string code, HttpPostedFileBase file, [Bind(Include = "quantity, rateFormated, diffFormated, rate, name, diff, date, validFromDate")] CurrencyDTO currency)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("მონაცემები არავალიდურია!");

                string output = JsonConvert.SerializeObject(currency);
                var stringContent = new StringContent(output, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync($"{BaseURL}/EditCurrency/{code}/{User.Identity.Name.Substring(0, User.Identity.Name.IndexOf("@"))}", stringContent).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }

                TempData["codes"] = new List<string> { code };

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}