using System.Threading.Tasks;
using Escola.Aplicacao.DTOs;

namespace Escola.Aplicacao.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoResponseDto> ObterPorIdAsync(int id);
        Task<AlunoListaResponseDto> ListarAsync(string nomeFiltro, int pagina, int tamanhoPagina);
        Task<int> CriarAsync(AlunoRequestDto request);
        Task AtualizarAsync(int id, AlunoRequestDto request);
        Task InativarAsync(int id);
    }
}