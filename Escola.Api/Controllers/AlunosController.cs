using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.DTOs;
using Escola.Aplicacao.Interfaces;

namespace Escola.Api.Controllers
{
    [RoutePrefix("api/alunos")]
    public class AlunosController : ApiController
    {
        private readonly IAlunoService _alunoService;

        public AlunosController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Listar(string nome = null, int pagina = 1, int tamanhoPagina = 10)
        {
            var resultado = await _alunoService.ListarAsync(nome, pagina, tamanhoPagina);
            return Ok(resultado);
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> ObterPorId(int id)
        {
            var aluno = await _alunoService.ObterPorIdAsync(id);
            return Ok(aluno);
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Criar(AlunoRequestDto request)
        {
            var id = await _alunoService.CriarAsync(request);
            return Created($"api/alunos/{id}", new { id });
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Atualizar(int id, AlunoRequestDto request)
        {
            await _alunoService.AtualizarAsync(id, request);
            return Ok();
        }

        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Inativar(int id)
        {
            await _alunoService.InativarAsync(id);
            return Ok();
        }
    }
}