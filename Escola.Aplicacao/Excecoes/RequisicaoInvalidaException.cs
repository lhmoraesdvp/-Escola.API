using System;

namespace Escola.Aplicacao.Excecoes
{
    public class RequisicaoInvalidaException : Exception
    {
        public RequisicaoInvalidaException(string mensagem) : base(mensagem) { }
    }
}