using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio.Dados;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Infraestrutura.Repositorios
{
    public class AlunoRepositorio : IAlunoRepositorio
    {
        private readonly IConexaoFactory _conexaoFactory;

        public AlunoRepositorio(IConexaoFactory conexaoFactory)
        {
            _conexaoFactory = conexaoFactory;
        }

        public async Task<Aluno> ObterPorIdAsync(int id)
        {
            const string sql = @"
                SELECT Id, Nome, Email, DataNascimento, Ativo, DataCadastro
                FROM dbo.Aluno
                WHERE Id = @Id;";

            using (var conexao = _conexaoFactory.CriarConexao())
            {
                return await conexao.QueryFirstOrDefaultAsync<Aluno>(sql, new { Id = id });
            }
        }

        public async Task<(IEnumerable<Aluno> Itens, int Total)> ListarAsync(
            string nomeFiltro, int pagina, int tamanhoPagina)
        {
            const string sql = @"
                SELECT Id, Nome, Email, DataNascimento, Ativo, DataCadastro
                FROM dbo.Aluno
                WHERE (@NomeFiltro IS NULL OR Nome LIKE '%' + @NomeFiltro + '%')
                ORDER BY Nome
                OFFSET @Skip ROWS FETCH NEXT @TamanhoPagina ROWS ONLY;

                SELECT COUNT(*)
                FROM dbo.Aluno
                WHERE (@NomeFiltro IS NULL OR Nome LIKE '%' + @NomeFiltro + '%');";

            using (var conexao = _conexaoFactory.CriarConexao())
            {
                var parametros = new
                {
                    NomeFiltro = nomeFiltro,
                    Skip = (pagina - 1) * tamanhoPagina,
                    TamanhoPagina = tamanhoPagina
                };

                using (var multi = await conexao.QueryMultipleAsync(sql, parametros))
                {
                    var itens = await multi.ReadAsync<Aluno>();
                    var total = await multi.ReadSingleAsync<int>();
                    return (itens, total);
                }
            }
        }

        public async Task<int> InserirAsync(Aluno aluno)
        {
            const string sql = @"
                INSERT INTO dbo.Aluno (Nome, Email, DataNascimento, Ativo, DataCadastro)
                OUTPUT INSERTED.Id
                VALUES (@Nome, @Email, @DataNascimento, @Ativo, GETDATE());";

            using (var conexao = _conexaoFactory.CriarConexao())
            {
                return await conexao.QuerySingleAsync<int>(sql, aluno);
            }
        }

        public async Task AtualizarAsync(Aluno aluno)
        {
            const string sql = @"
                UPDATE dbo.Aluno
                SET Nome = @Nome, Email = @Email, DataNascimento = @DataNascimento
                WHERE Id = @Id;";

            using (var conexao = _conexaoFactory.CriarConexao())
            {
                await conexao.ExecuteAsync(sql, aluno);
            }
        }

        public async Task InativarAsync(int id)
        {
            const string sql = "UPDATE dbo.Aluno SET Ativo = 0 WHERE Id = @Id;";

            using (var conexao = _conexaoFactory.CriarConexao())
            {
                await conexao.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}