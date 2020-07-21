using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XMen.Data;
using XMen.Tools;
using XMen.WebApi.Models;

namespace XMen.WebApi.Controllers
{
    /// <summary>
    /// Sabrás si un humano es mutante, si encuentras ​más de una secuencia de cuatro letras iguales​, de forma oblicua, horizontal o vertical.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class MutantController : ControllerBase
    {
        private readonly IMutantDetector _detector;
        private readonly IBigTableManager _bigTable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detector"></param>
        /// <param name="bigTable"></param>
        public MutantController(IMutantDetector detector, 
            IBigTableManager bigTable)
        {
            _detector = detector;
            _bigTable = bigTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("It works!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(MutantInput input)
        {
            var isMutant = await _bigTable.InsertAndGet(input.Dna, dna => _detector.IsMutant(dna));
            return isMutant ? Ok() : StatusCode(403);
        }
    }
}
