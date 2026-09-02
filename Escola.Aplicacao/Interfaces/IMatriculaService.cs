using System.Threading.Tasks;
using Escola.Aplicacao.DTOs;

namespace Escola.Aplicacao.Interfaces
{
    public interface IMatriculaService
    {
        Task MatricularAsync(MatriculaRequestDto request);
    }
}