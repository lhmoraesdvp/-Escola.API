using System;

namespace Escola.Aplicacao.Excecoes
{
    public class RegraNegocioException : Exception
    {
        public RegraNegocioException(string mensagem) : base(mensagem) { }
    }
}