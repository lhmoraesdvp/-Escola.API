using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio.Dados;
using Escola.Dominio.Relatorios;
using Escola.Dominio.Repositorios;

namespace Escola.Infraestrutura.Repositorios
{
    public class RelatorioRepositorio : IRelatorioRepositorio
    {
        private readonly IConexaoFactory _conexaoFactory;

        public RelatorioRepositorio(IConexaoFactory conexaoFactory)
        {
            _conexaoFactory = conexaoFactory;
        }

        public async Task<IEnumerable<AlunosPorTurmaResultado>> ObterAlunosPorTurmaAsync()
        {
            const string sql = @"
                SELECT
                    t.Nome AS NomeTurma,
                    COUNT(m.Id) AS QuantidadeAlunos,
                    t.VagasDisponiveis AS VagasRestantes
                FROM dbo.Turma t
                LEFT JOIN dbo.Matricula m ON m.TurmaId = t.Id
                GROUP BY t.Id, t.Nome, t.VagasDisponiveis
                ORDER BY t.Nome;";

            using (var conexao = _conexaoFactory.CriarConexao())
            {
                return await conexao.QueryAsync<AlunosPorTurmaResultado>(sql);
            }
        }
    }
}