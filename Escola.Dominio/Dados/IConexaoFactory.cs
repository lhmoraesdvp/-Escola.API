using System.Data;

namespace Escola.Dominio.Dados
{
    public interface IConexaoFactory
    {
        IDbConnection CriarConexao();
    }
}