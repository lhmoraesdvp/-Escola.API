// Controllers/PaginasController.cs
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Escola.Api.Controllers
{
    [RoutePrefix("paginas")]
    public class PaginasController : ApiController
    {
        [HttpGet, Route("alunos")]
        public HttpResponseMessage Alunos()
        {
            var resposta = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(HtmlAlunos, Encoding.UTF8, "text/html")
            };
            return resposta;
        }

        private const string HtmlAlunos = @"
<!DOCTYPE html>
<html lang=""pt-br"">
<head>
    <meta charset=""UTF-8"">
    <title>Alunos - Sistema de Matriculas</title>
    <script src=""https://code.jquery.com/jquery-3.7.1.min.js""></script>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background: #f5f5f5; }
        h1 { color: #2c3e50; }
        #filtro { margin-bottom: 16px; }
        #filtro input { padding: 8px; width: 250px; }
        #filtro button { padding: 8px 16px; margin-left: 8px; cursor: pointer; }
        table { border-collapse: collapse; width: 100%; background: white; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        th, td { padding: 10px 14px; text-align: left; border-bottom: 1px solid #eee; }
        th { background: #2c3e50; color: white; }
        tr:hover { background: #f9f9f9; }
        .ativo { color: #27ae60; font-weight: bold; }
        .inativo { color: #c0392b; font-weight: bold; }
        #total { margin-top: 12px; color: #555; }
        #carregando { color: #888; }
    </style>
</head>
<body>
    <h1>Lista de Alunos</h1>

    <div id=""filtro"">
        <input type=""text"" id=""inputNome"" placeholder=""Filtrar por nome..."">
        <button id=""btnFiltrar"">Filtrar</button>
        <button id=""btnLimpar"">Limpar</button>
    </div>

    <p id=""carregando"">Carregando...</p>

    <table id=""tabelaAlunos"" style=""display:none;"">
        <thead>
            <tr>
                <th>Id</th>
                <th>Nome</th>
                <th>Email</th>
                <th>Data de Nascimento</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody id=""corpoTabela""></tbody>
    </table>

    <p id=""total""></p>

    <script>
        function carregarAlunos(nomeFiltro) {
            $('#carregando').text('Carregando...').show();
            $('#tabelaAlunos').hide();

            $.ajax({
                url: '/api/alunos',
                method: 'GET',
                data: { nome: nomeFiltro || null, pagina: 1, tamanhoPagina: 50 },
                dataType: 'json'
            }).done(function (resultado) {
                var corpo = $('#corpoTabela');
                corpo.empty();

                resultado.itens.forEach(function (aluno) {
                    var dataFormatada = new Date(aluno.dataNascimento).toLocaleDateString('pt-BR');
                    var statusClasse = aluno.ativo ? 'ativo' : 'inativo';
                    var statusTexto = aluno.ativo ? 'Ativo' : 'Inativo';

                    var linha = '<tr>' +
                        '<td>' + aluno.id + '</td>' +
                        '<td>' + aluno.nome + '</td>' +
                        '<td>' + aluno.email + '</td>' +
                        '<td>' + dataFormatada + '</td>' +
                        '<td class=""' + statusClasse + '"">' + statusTexto + '</td>' +
                        '</tr>';

                    corpo.append(linha);
                });

                $('#total').text('Total de alunos encontrados: ' + resultado.total);
                $('#carregando').hide();
                $('#tabelaAlunos').show();
            }).fail(function (erro) {
                $('#carregando').text('Erro ao carregar alunos: ' + erro.statusText);
            });
        }

        $(document).ready(function () {
            carregarAlunos();

            $('#btnFiltrar').on('click', function () {
                carregarAlunos($('#inputNome').val());
            });

            $('#btnLimpar').on('click', function () {
                $('#inputNome').val('');
                carregarAlunos();
            });
        });
    </script>
</body>
</html>
";
    }
}