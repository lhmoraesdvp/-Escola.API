// App_Start/UnityConfig.cs
using System;
using Unity;

using Escola.Dominio.Dados;
using Escola.Dominio.Repositorios;
using Escola.Infraestrutura.Dados;
using Escola.Infraestrutura.Repositorios;
using Escola.Aplicacao.Interfaces;
using Escola.Aplicacao.Servicos;

namespace Escola.Api
{
    public static class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> container =
            new Lazy<IUnityContainer>(() =>
            {
                var c = new UnityContainer();
                RegisterTypes(c);
                return c;
            });

        public static IUnityContainer Container => container.Value;

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IConexaoFactory, ConexaoFactory>();
            container.RegisterType<IAlunoRepositorio, AlunoRepositorio>();
            container.RegisterType<ITurmaRepositorio, TurmaRepositorio>();
            container.RegisterType<IMatriculaRepositorio, MatriculaRepositorio>();

            container.RegisterType<IAlunoService, AlunoService>();
            container.RegisterType<ITurmaService, TurmaService>();
            container.RegisterType<IMatriculaService, MatriculaService>();
            container.RegisterType<IRelatorioRepositorio, RelatorioRepositorio>();
            container.RegisterType<IRelatorioService, RelatorioService>();
        }
    }
}