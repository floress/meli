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
    public class Mutant2Controller : ControllerBase
    {
        private readonly IMutantDetector _detector;
        private readonly IPostgresqlManager _postgresql;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detector"></param>
        /// <param name="postgresql"></param>
        public Mutant2Controller(IMutantDetector detector, 
            IPostgresqlManager postgresql)
        {
            _detector = detector;
            _postgresql = postgresql;
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
            var isMutant = await _postgresql.InsertAndGet(input.Dna, dna => _detector.IsMutant(dna));
            return isMutant ? Ok() : StatusCode(403);
        }
    }
}
