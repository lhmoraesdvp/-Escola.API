using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Interfaces;

namespace Escola.Api.Controllers
{
    [RoutePrefix("api/relatorios")]
    public class RelatoriosController : ApiController
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpGet, Route("alunos-por-turma")]
        public async Task<IHttpActionResult> AlunosPorTurma()
        {
            var resultado = await _relatorioService.ObterAlunosPorTurmaAsync();
            return Ok(resultado);
        }
    }
}