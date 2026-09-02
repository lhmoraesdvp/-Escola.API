#nullable disable
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Escola.Aplicacao.DTOs;
using Escola.Aplicacao.Excecoes;
using Escola.Aplicacao.Servicos;
using Escola.Dominio.Dados;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Tests
{
    [TestClass]
    public class MatriculaServiceTests
    {
        private Mock<IConexaoFactory> _conexaoFactoryMock;
        private Mock<IAlunoRepositorio> _alunoRepositorioMock;
        private Mock<ITurmaRepositorio> _turmaRepositorioMock;
        private Mock<IMatriculaRepositorio> _matriculaRepositorioMock;
        private Mock<IDbConnection> _conexaoMock;
        private Mock<IDbTransaction> _transacaoMock;
        private MatriculaService _service;

        [TestInitialize]
        public void Setup()
        {
            _alunoRepositorioMock = new Mock<IAlunoRepositorio>();
            _turmaRepositorioMock = new Mock<ITurmaRepositorio>();
            _matriculaRepositorioMock = new Mock<IMatriculaRepositorio>();

            _transacaoMock = new Mock<IDbTransaction>();

            _conexaoMock = new Mock<IDbConnection>();
            _conexaoMock.Setup(c => c.BeginTransaction()).Returns(_transacaoMock.Object);

            _conexaoFactoryMock = new Mock<IConexaoFactory>();
            _conexaoFactoryMock.Setup(f => f.CriarConexao()).Returns(_conexaoMock.Object);

            _service = new MatriculaService(
                _conexaoFactoryMock.Object,
                _alunoRepositorioMock.Object,
                _turmaRepositorioMock.Object,
                _matriculaRepositorioMock.Object);
        }

        /// <summary>
        /// Helper que substitui Assert.ThrowsExceptionAsync (indisponivel nessa versao do MSTest).
        /// Executa a acao e falha o teste se a excecao esperada nao for lancada.
        /// </summary>
        private static async Task AssertLancaExcecaoAsync<TException>(Func<Task> acao)
            where TException : Exception
        {
            try
            {
                await acao();
            }
            catch (TException)
            {
                return; // excecao esperada foi lancada, teste passa
            }

            Assert.Fail($"Esperava uma excecao do tipo {typeof(TException).Name}, mas nenhuma foi lancada.");
        }

        [TestMethod]
        public async Task MatricularAsync_AlunoNaoEncontrado_DeveLancarEntidadeNaoEncontradaException()
        {
            _alunoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync((Aluno)null);

            var request = new MatriculaRequestDto { AlunoId = 1, TurmaId = 1 };

            await AssertLancaExcecaoAsync<EntidadeNaoEncontradaException>(
                () => _service.MatricularAsync(request));
        }

        [TestMethod]
        public async Task MatricularAsync_AlunoInativo_DeveLancarRegraNegocioException()
        {
            _alunoRepositorioMock.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(new Aluno { Id = 1, Ativo = false });

            var request = new MatriculaRequestDto { AlunoId = 1, TurmaId = 1 };

            await AssertLancaExcecaoAsync<RegraNegocioException>(
                () => _service.MatricularAsync(request));
        }

        [TestMethod]
        public async Task MatricularAsync_TurmaNaoEncontrada_DeveLancarEntidadeNaoEncontradaException()
        {
            _alunoRepositorioMock.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(new Aluno { Id = 1, Ativo = true });

            _turmaRepositorioMock
                .Setup(r => r.ObterPorIdParaAtualizacaoAsync(1, _conexaoMock.Object, _transacaoMock.Object))
                .ReturnsAsync((Turma)null);

            var request = new MatriculaRequestDto { AlunoId = 1, TurmaId = 1 };

            await AssertLancaExcecaoAsync<EntidadeNaoEncontradaException>(
                () => _service.MatricularAsync(request));

            _transacaoMock.Verify(t => t.Rollback(), Times.Once);
        }

        [TestMethod]
        public async Task MatricularAsync_TurmaSemVaga_DeveLancarRegraNegocioException()
        {
            _alunoRepositorioMock.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(new Aluno { Id = 1, Ativo = true });

            _turmaRepositorioMock
                .Setup(r => r.ObterPorIdParaAtualizacaoAsync(1, _conexaoMock.Object, _transacaoMock.Object))
                .ReturnsAsync(new Turma { Id = 1, VagasDisponiveis = 0 });

            var request = new MatriculaRequestDto { AlunoId = 1, TurmaId = 1 };

            await AssertLancaExcecaoAsync<RegraNegocioException>(
                () => _service.MatricularAsync(request));

            _transacaoMock.Verify(t => t.Rollback(), Times.Once);
            _transacaoMock.Verify(t => t.Commit(), Times.Never);
        }

        [TestMethod]
        public async Task MatricularAsync_AlunoJaMatriculado_DeveLancarRegraNegocioException()
        {
            _alunoRepositorioMock.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(new Aluno { Id = 1, Ativo = true });

            _turmaRepositorioMock
                .Setup(r => r.ObterPorIdParaAtualizacaoAsync(1, _conexaoMock.Object, _transacaoMock.Object))
                .ReturnsAsync(new Turma { Id = 1, VagasDisponiveis = 5 });

            _matriculaRepositorioMock
                .Setup(r => r.AlunoJaMatriculadoAsync(1, 1, _conexaoMock.Object, _transacaoMock.Object))
                .ReturnsAsync(true);

            var request = new MatriculaRequestDto { AlunoId = 1, TurmaId = 1 };

            await AssertLancaExcecaoAsync<RegraNegocioException>(
                () => _service.MatricularAsync(request));

            _transacaoMock.Verify(t => t.Rollback(), Times.Once);
        }

        [TestMethod]
        public async Task MatricularAsync_CenarioValido_DeveInserirMatriculaEDecrementarVagaEExecutarCommit()
        {
            _alunoRepositorioMock.Setup(r => r.ObterPorIdAsync(1))
                .ReturnsAsync(new Aluno { Id = 1, Ativo = true });

            _turmaRepositorioMock
                .Setup(r => r.ObterPorIdParaAtualizacaoAsync(1, _conexaoMock.Object, _transacaoMock.Object))
                .ReturnsAsync(new Turma { Id = 1, VagasDisponiveis = 5 });

            _matriculaRepositorioMock
                .Setup(r => r.AlunoJaMatriculadoAsync(1, 1, _conexaoMock.Object, _transacaoMock.Object))
                .ReturnsAsync(false);

            var request = new MatriculaRequestDto { AlunoId = 1, TurmaId = 1 };

            await _service.MatricularAsync(request);

            _matriculaRepositorioMock.Verify(
                r => r.InserirAsync(
                    It.Is<Matricula>(m => m.AlunoId == 1 && m.TurmaId == 1),
                    _conexaoMock.Object,
                    _transacaoMock.Object),
                Times.Once);

            _turmaRepositorioMock.Verify(
                r => r.DecrementarVagaAsync(1, _conexaoMock.Object, _transacaoMock.Object),
                Times.Once);

            _transacaoMock.Verify(t => t.Commit(), Times.Once);
            _transacaoMock.Verify(t => t.Rollback(), Times.Never);
        }
    }
}