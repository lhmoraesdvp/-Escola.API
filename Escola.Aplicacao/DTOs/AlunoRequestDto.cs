using System;

namespace Escola.Aplicacao.DTOs
{
    public class AlunoRequestDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}