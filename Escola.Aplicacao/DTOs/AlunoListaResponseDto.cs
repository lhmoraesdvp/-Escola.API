using System.Collections.Generic;

namespace Escola.Aplicacao.DTOs
{
    public class AlunoListaResponseDto
    {
        public IEnumerable<AlunoResponseDto> Itens { get; set; }
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
    }
}