using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Escola.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuração e serviços de API Web

            // Rotas de API Web
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new Escola.Api.Filtros.ExcecaoFiltroAttribute());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            // forca JSON como formato de resposta padrao
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
