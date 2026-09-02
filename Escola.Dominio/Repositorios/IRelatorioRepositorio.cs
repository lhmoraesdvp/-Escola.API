using System.Collections.Generic;
using System.Threading.Tasks;
using Escola.Dominio.Relatorios;

namespace Escola.Dominio.Repositorios
{
    public interface IRelatorioRepositorio
    {
        Task<IEnumerable<AlunosPorTurmaResultado>> ObterAlunosPorTurmaAsync();
    }
}