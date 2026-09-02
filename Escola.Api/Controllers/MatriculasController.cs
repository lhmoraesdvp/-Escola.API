using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.DTOs;
using Escola.Aplicacao.Interfaces;

namespace Escola.Api.Controllers
{
    [RoutePrefix("api/matriculas")]
    public class MatriculasController : ApiController
    {
        private readonly IMatriculaService _matriculaService;

        public MatriculasController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Matricular(MatriculaRequestDto request)
        {
            await _matriculaService.MatricularAsync(request);
            return StatusCode(HttpStatusCode.Created);
        }
    }
}