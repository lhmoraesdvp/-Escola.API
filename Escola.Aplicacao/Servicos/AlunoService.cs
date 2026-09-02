using System.Linq;
using System.Threading.Tasks;
using Escola.Aplicacao.DTOs;
using Escola.Aplicacao.Excecoes;
using Escola.Aplicacao.Interfaces;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Servicos
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepositorio _alunoRepositorio;

        public AlunoService(IAlunoRepositorio alunoRepositorio)
        {
            _alunoRepositorio = alunoRepositorio;
        }

        public async Task<AlunoResponseDto> ObterPorIdAsync(int id)
        {
            var aluno = await _alunoRepositorio.ObterPorIdAsync(id);
            if (aluno == null)
                throw new EntidadeNaoEncontradaException($"Aluno {id} nao encontrado.");

            return MapearParaDto(aluno);
        }

        public async Task<AlunoListaResponseDto> ListarAsync(string nomeFiltro, int pagina, int tamanhoPagina)
        {
            if (pagina < 1) pagina = 1;
            if (tamanhoPagina < 1 || tamanhoPagina > 100) tamanhoPagina = 10;

            var (itens, total) = await _alunoRepositorio.ListarAsync(nomeFiltro, pagina, tamanhoPagina);

            return new AlunoListaResponseDto
            {
                Itens = itens.Select(MapearParaDto),
                Total = total,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina
            };
        }

        public async Task<int> CriarAsync(AlunoRequestDto request)
        {
            ValidarRequest(request);

            var aluno = new Aluno
            {
                Nome = request.Nome,
                Email = request.Email,
                DataNascimento = request.DataNascimento,
                Ativo = true
            };

            return await _alunoRepositorio.InserirAsync(aluno);
        }

        public async Task AtualizarAsync(int id, AlunoRequestDto request)
        {
            ValidarRequest(request);

            var alunoExistente = await _alunoRepositorio.ObterPorIdAsync(id);
            if (alunoExistente == null)
                throw new EntidadeNaoEncontradaException($"Aluno {id} nao encontrado.");

            alunoExistente.Nome = request.Nome;
            alunoExistente.Email = request.Email;
            alunoExistente.DataNascimento = request.DataNascimento;

            await _alunoRepositorio.AtualizarAsync(alunoExistente);
        }

        public async Task InativarAsync(int id)
        {
            var aluno = await _alunoRepositorio.ObterPorIdAsync(id);
            if (aluno == null)
                throw new EntidadeNaoEncontradaException($"Aluno {id} nao encontrado.");

            await _alunoRepositorio.InativarAsync(id);
        }

        private static void ValidarRequest(AlunoRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new RequisicaoInvalidaException("Nome e obrigatorio.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RequisicaoInvalidaException("Email e obrigatorio.");
        }

        private static AlunoResponseDto MapearParaDto(Aluno aluno)
        {
            return new AlunoResponseDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Email = aluno.Email,
                DataNascimento = aluno.DataNascimento,
                Ativo = aluno.Ativo,
                DataCadastro = aluno.DataCadastro
            };
        }
    }
}