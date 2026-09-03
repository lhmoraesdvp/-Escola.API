using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Escola.Dominio.Cache;

namespace Escola.Infraestrutura.Cache
{
    public class CacheEmMemoria : ICacheService
    {
        // ConcurrentDictionary garante thread-safety, ja que multiplas
        // requisicoes HTTP podem ler/escrever no cache simultaneamente
        private static readonly ConcurrentDictionary<string, (object Valor, DateTime Expira)> _armazenamento
            = new ConcurrentDictionary<string, (object, DateTime)>();

        public Task<T> ObterAsync<T>(string chave) where T : class
        {
            if (_armazenamento.TryGetValue(chave, out var item) && item.Expira > DateTime.UtcNow)
            {
                return Task.FromResult(item.Valor as T);
            }

            return Task.FromResult<T>(null);
        }

        public Task DefinirAsync<T>(string chave, T valor, TimeSpan expiracao) where T : class
        {
            _armazenamento[chave] = (valor, DateTime.UtcNow.Add(expiracao));
            return Task.CompletedTask;
        }

        public Task RemoverAsync(string chave)
        {
            _armazenamento.TryRemove(chave, out _);
            return Task.CompletedTask;
        }
    }
}