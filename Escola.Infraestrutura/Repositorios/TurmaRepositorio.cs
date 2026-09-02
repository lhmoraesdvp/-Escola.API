using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio.Dados;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Infraestrutura.Repositorios
{
    public class TurmaRepositorio : ITurmaRepositorio
    {
        private readonly IConexaoFactory _conexaoFactory;

        public TurmaRepositorio(IConexaoFactory conexaoFactory)
        {
            _conexaoFactory = conexaoFactory;
        }

        public async Task<IEnumerable<Turma>> ListarComVagasAsync()
        {
            const string sql = @"
                SELECT Id, Nome, Periodo, VagasTotal, VagasDisponiveis
                FROM dbo.Turma
                ORDER BY Nome;";

            using (var conexao = _conexaoFactory.CriarConexao())
            {
                return await conexao.QueryAsync<Turma>(sql);
            }
        }

        public async Task<Turma> ObterPorIdParaAtualizacaoAsync(
            int id, IDbConnection conexao, IDbTransaction transacao)
        {
            const string sql = @"
                SELECT Id, Nome, Periodo, VagasTotal, VagasDisponiveis
                FROM dbo.Turma WITH (UPDLOCK, ROWLOCK)
                WHERE Id = @Id;";

            return await conexao.QueryFirstOrDefaultAsync<Turma>(sql, new { Id = id }, transacao);
        }

        public async Task DecrementarVagaAsync(
            int turmaId, IDbConnection conexao, IDbTransaction transacao)
        {
            const string sql = @"
                UPDATE dbo.Turma
                SET VagasDisponiveis = VagasDisponiveis - 1
                WHERE Id = @TurmaId;";

            await conexao.ExecuteAsync(sql, new { TurmaId = turmaId }, transacao);
        }
    }
}