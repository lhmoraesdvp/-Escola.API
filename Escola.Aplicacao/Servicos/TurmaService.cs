// Servicos/TurmaService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Escola.Aplicacao.DTOs;
using Escola.Aplicacao.Interfaces;
using Escola.Dominio.Cache;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Servicos
{
    public class TurmaService : ITurmaService
    {
        private const string ChaveCacheTurmas = "turmas:listagem";

        private readonly ITurmaRepositorio _turmaRepositorio;
        private readonly ICacheService _cacheService;

        public TurmaService(ITurmaRepositorio turmaRepositorio, ICacheService cacheService)
        {
            _turmaRepositorio = turmaRepositorio;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<TurmaResponseDto>> ListarComVagasAsync()
        {
            var cacheado = await _cacheService.ObterAsync<List<TurmaResponseDto>>(ChaveCacheTurmas);
            if (cacheado != null)
            {
                return cacheado;
            }

            var turmas = await _turmaRepositorio.ListarComVagasAsync();

            var resultado = turmas.Select(t => new TurmaResponseDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Periodo = t.Periodo,
                VagasTotal = t.VagasTotal,
                VagasDisponiveis = t.VagasDisponiveis
            }).ToList();

            await _cacheService.DefinirAsync(ChaveCacheTurmas, resultado, TimeSpan.FromMinutes(5));

            return resultado;
        }
    }
}