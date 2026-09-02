using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Escola.Aplicacao.Excecoes;

namespace Escola.Api.Filtros
{
    public class ExcecaoFiltroAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var excecao = context.Exception;
            HttpStatusCode statusCode;
            string mensagem = excecao.Message;

            if (excecao is RequisicaoInvalidaException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (excecao is EntidadeNaoEncontradaException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else if (excecao is RegraNegocioException)
            {
                statusCode = HttpStatusCode.Conflict;
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
                mensagem = "Ocorreu um erro interno.";
            }

            context.Response = context.Request.CreateErrorResponse(statusCode, mensagem);
        }
    }
}