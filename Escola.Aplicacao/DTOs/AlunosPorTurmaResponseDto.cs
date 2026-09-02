namespace Escola.Aplicacao.DTOs
{
    public class AlunosPorTurmaResponseDto
    {
        public string NomeTurma { get; set; }
        public int QuantidadeAlunos { get; set; }
        public int VagasRestantes { get; set; }
    }
}