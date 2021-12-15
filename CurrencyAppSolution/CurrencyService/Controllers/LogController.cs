using BLL.Services;
using Models.DataViewModels.CurrencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CurrencyService.Controllers
{
    public class LogController : ApiController
    {
        LogManagement logM = new LogManagement();

        [Route("api/Log/GetAll")]
        [HttpGet]
        public IEnumerable<LogDTO> GetAllCurrencies() => logM.GetAllLogs();
    }
}
