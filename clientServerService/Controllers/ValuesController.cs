using DistributeService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace clientServerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IDistributeServices _DistributeServices;
        public ValuesController(ILogger<ValuesController> logger, IDistributeServices DistributeServices)
        {
            _logger = logger;
            _DistributeServices = DistributeServices;
        }


        [HttpPost]
        [Route("InsertString")]
        public IActionResult InsertString(string value)
        {
            _DistributeServices.ProcessStoreString(value);
            return Ok();
        }

        [HttpGet]
        [Route("getString")]
        public IActionResult getString(string value)
        {
            
            return Ok();
        }
    }
}
