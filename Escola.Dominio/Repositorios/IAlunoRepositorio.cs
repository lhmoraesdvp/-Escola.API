using System.Collections.Generic;
using System.Threading.Tasks;
using Escola.Dominio.Entidades;

namespace Escola.Dominio.Repositorios
{
    public interface IAlunoRepositorio
    {
        Task<Aluno> ObterPorIdAsync(int id);
        Task<(IEnumerable<Aluno> Itens, int Total)> ListarAsync(string nomeFiltro, int pagina, int tamanhoPagina);
        Task<int> InserirAsync(Aluno aluno);
        Task AtualizarAsync(Aluno aluno);
        Task InativarAsync(int id);
    }
}