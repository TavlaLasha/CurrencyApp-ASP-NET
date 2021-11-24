using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL.Services;
using Models.DataViewModels.CurrencyManagement;

namespace CurrencyService.Controllers
{
    public class CurrencyController : ApiController
    {
        CurrencyManagement currM = new CurrencyManagement();

        [Route("api/Currency/FillDBWithLatest/{user}")]
        [HttpGet]
        public List<string> FillDBWithLatest(string user) => currM.FillDBWithNew(user);

        [Route("api/Currency/EditCurrency/{code}/{user}")]
        [HttpPost]
        public bool EditCurrency(string code, string user, [FromBody] CurrencyDTO u) => currM.EditCurrency(code, user, u);

        [Route("api/Currency/GetAll")]
        [HttpGet]
        public IEnumerable<CurrencyDTO> GetAllCurrencies() => currM.GetAllCurrencies();

        [Route("api/Currency/GetCurrency/{code}")]
        [HttpGet]
        public CurrencyDTO GetAllCurrencies(string code) => currM.GetCurrency(code);
    }
}
