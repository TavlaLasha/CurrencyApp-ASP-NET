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
        CurrecnyManagement currM = new CurrecnyManagement();

        [Route("api/Currency/FillDBWithLatest")]
        [HttpGet]
        public bool FillDBWithLatest() => currM.FillDBWithNew();

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
