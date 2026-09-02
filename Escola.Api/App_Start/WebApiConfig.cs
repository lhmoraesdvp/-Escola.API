using System.Web.Http;

namespace Escola.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Filters.Add(new Escola.Api.Filtros.ExcecaoFiltroAttribute());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // forca JSON como formato de resposta padrao
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // usa camelCase no JSON (nome -> "nome", nao "Nome"), consistente com o alunos.html
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        }
    }
}