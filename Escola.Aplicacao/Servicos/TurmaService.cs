using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Escola.Aplicacao.DTOs;
using Escola.Aplicacao.Interfaces;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Servicos
{
    public class TurmaService : ITurmaService
    {
        private readonly ITurmaRepositorio _turmaRepositorio;

        public TurmaService(ITurmaRepositorio turmaRepositorio)
        {
            _turmaRepositorio = turmaRepositorio;
        }

        public async Task<IEnumerable<TurmaResponseDto>> ListarComVagasAsync()
        {
            var turmas = await _turmaRepositorio.ListarComVagasAsync();

            return turmas.Select(t => new TurmaResponseDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Periodo = t.Periodo,
                VagasTotal = t.VagasTotal,
                VagasDisponiveis = t.VagasDisponiveis
            });
        }
    }
}