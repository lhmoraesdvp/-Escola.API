using System.Data;
using System.Threading.Tasks;
using Escola.Dominio.Entidades;

namespace Escola.Dominio.Repositorios
{
    public interface IMatriculaRepositorio
    {
        Task<bool> AlunoJaMatriculadoAsync(int alunoId, int turmaId, IDbConnection conexao, IDbTransaction transacao);
        Task InserirAsync(Matricula matricula, IDbConnection conexao, IDbTransaction transacao);
    }
}