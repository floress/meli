using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XMen.Data;

namespace XMen.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IBigTableManager _manager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        public StatsController(IBigTableManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _manager.Stats());
        }
    }
}
