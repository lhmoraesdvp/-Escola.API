using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Escola.Dominio.Entidades;

namespace Escola.Dominio.Repositorios
{
    public interface ITurmaRepositorio
    {
        Task<IEnumerable<Turma>> ListarComVagasAsync();

        // versoes que participam de uma transacao (usadas no fluxo de matricula)
        Task<Turma> ObterPorIdParaAtualizacaoAsync(int id, IDbConnection conexao, IDbTransaction transacao);
        Task DecrementarVagaAsync(int turmaId, IDbConnection conexao, IDbTransaction transacao);
    }
}