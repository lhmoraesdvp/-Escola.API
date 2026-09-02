using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Escola.Dominio.Dados;

namespace Escola.Infraestrutura.Dados
{
    public class ConexaoFactory : IConexaoFactory
    {
        private readonly string _connectionString;

        public ConexaoFactory()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["TesteEscola"].ConnectionString;
        }

        public IDbConnection CriarConexao()
        {
            return new SqlConnection(_connectionString);
        }
    }
}