using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Escola.Api.Controllers
{
    [RoutePrefix("")]
    public class InicioController : ApiController
    {
        [HttpGet, Route("")]
        public HttpResponseMessage Index()
        {
            var resposta = new HttpResponseMessage(HttpStatusCode.Redirect);
            resposta.Headers.Location = new Uri("/paginas/alunos", UriKind.Relative);
            return resposta;
        }
    }
}