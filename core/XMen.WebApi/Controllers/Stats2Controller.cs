using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XMen.Data;

namespace XMen.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Stats2Controller : ControllerBase
    {
        private readonly IPostgresqlManager _manager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        public Stats2Controller(IPostgresqlManager manager)
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
