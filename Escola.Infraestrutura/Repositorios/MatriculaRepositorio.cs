using System.Data;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Infraestrutura.Repositorios
{
    public class MatriculaRepositorio : IMatriculaRepositorio
    {
        public async Task<bool> AlunoJaMatriculadoAsync(
            int alunoId, int turmaId, IDbConnection conexao, IDbTransaction transacao)
        {
            const string sql = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM dbo.Matricula
                    WHERE AlunoId = @AlunoId AND TurmaId = @TurmaId
                ) THEN 1 ELSE 0 END;";

            return await conexao.QuerySingleAsync<bool>(
                sql, new { AlunoId = alunoId, TurmaId = turmaId }, transacao);
        }

        public async Task InserirAsync(
            Matricula matricula, IDbConnection conexao, IDbTransaction transacao)
        {
            const string sql = @"
                INSERT INTO dbo.Matricula (AlunoId, TurmaId, DataMatricula)
                VALUES (@AlunoId, @TurmaId, GETDATE());";

            await conexao.ExecuteAsync(sql, matricula, transacao);
        }
    }
}