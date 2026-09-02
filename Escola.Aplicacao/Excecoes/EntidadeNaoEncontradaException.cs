using System;

namespace Escola.Aplicacao.Excecoes
{
    public class EntidadeNaoEncontradaException : Exception
    {
        public EntidadeNaoEncontradaException(string mensagem) : base(mensagem) { }
    }
}