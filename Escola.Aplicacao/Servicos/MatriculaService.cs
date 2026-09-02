using System.Threading.Tasks;
using Escola.Aplicacao.DTOs;
using Escola.Aplicacao.Excecoes;
using Escola.Aplicacao.Interfaces;
using Escola.Dominio.Dados;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Servicos
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IConexaoFactory _conexaoFactory;
        private readonly IAlunoRepositorio _alunoRepositorio;
        private readonly ITurmaRepositorio _turmaRepositorio;
        private readonly IMatriculaRepositorio _matriculaRepositorio;

        public MatriculaService(
            IConexaoFactory conexaoFactory,
            IAlunoRepositorio alunoRepositorio,
            ITurmaRepositorio turmaRepositorio,
            IMatriculaRepositorio matriculaRepositorio)
        {
            _conexaoFactory = conexaoFactory;
            _alunoRepositorio = alunoRepositorio;
            _turmaRepositorio = turmaRepositorio;
            _matriculaRepositorio = matriculaRepositorio;
        }

        public async Task MatricularAsync(MatriculaRequestDto request)
        {
            // validacao que nao precisa estar dentro da transacao,
            // pois nao envolve nenhuma escrita
            var aluno = await _alunoRepositorio.ObterPorIdAsync(request.AlunoId);
            if (aluno == null)
                throw new EntidadeNaoEncontradaException($"Aluno {request.AlunoId} nao encontrado.");

            if (!aluno.Ativo)
                throw new RegraNegocioException("Aluno inativo nao pode ser matriculado.");

            using (var conexao = _conexaoFactory.CriarConexao())
            {
                conexao.Open();
                using (var transacao = conexao.BeginTransaction())
                {
                    try
                    {
                        var turma = await _turmaRepositorio.ObterPorIdParaAtualizacaoAsync(
                            request.TurmaId, conexao, transacao);

                        if (turma == null)
                            throw new EntidadeNaoEncontradaException($"Turma {request.TurmaId} nao encontrada.");

                        if (turma.VagasDisponiveis <= 0)
                            throw new RegraNegocioException("Turma sem vagas disponiveis.");

                        var jaMatriculado = await _matriculaRepositorio.AlunoJaMatriculadoAsync(
                            request.AlunoId, request.TurmaId, conexao, transacao);

                        if (jaMatriculado)
                            throw new RegraNegocioException("Aluno ja matriculado nessa turma.");

                        await _matriculaRepositorio.InserirAsync(
                            new Matricula { AlunoId = request.AlunoId, TurmaId = request.TurmaId },
                            conexao, transacao);

                        await _turmaRepositorio.DecrementarVagaAsync(request.TurmaId, conexao, transacao);

                        transacao.Commit();
                    }
                    catch
                    {
                        transacao.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}