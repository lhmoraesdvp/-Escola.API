using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Interfaces;

namespace Escola.Api.Controllers
{
    [RoutePrefix("api/turmas")]
    public class TurmasController : ApiController
    {
        private readonly ITurmaService _turmaService;

        public TurmasController(ITurmaService turmaService)
        {
            _turmaService = turmaService;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Listar()
        {
            var turmas = await _turmaService.ListarComVagasAsync();
            return Ok(turmas);
        }
    }
}