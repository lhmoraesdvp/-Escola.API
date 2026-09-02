using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Escola.Aplicacao.DTOs;
using Escola.Aplicacao.Interfaces;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Servicos
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IRelatorioRepositorio _relatorioRepositorio;

        public RelatorioService(IRelatorioRepositorio relatorioRepositorio)
        {
            _relatorioRepositorio = relatorioRepositorio;
        }

        public async Task<IEnumerable<AlunosPorTurmaResponseDto>> ObterAlunosPorTurmaAsync()
        {
            var resultado = await _relatorioRepositorio.ObterAlunosPorTurmaAsync();

            return resultado.Select(r => new AlunosPorTurmaResponseDto
            {
                NomeTurma = r.NomeTurma,
                QuantidadeAlunos = r.QuantidadeAlunos,
                VagasRestantes = r.VagasRestantes
            });
        }
    }
}