using System.Collections.Generic;
using System.Threading.Tasks;
using Escola.Aplicacao.DTOs;

namespace Escola.Aplicacao.Interfaces
{
    public interface ITurmaService
    {
        Task<IEnumerable<TurmaResponseDto>> ListarComVagasAsync();
    }
}