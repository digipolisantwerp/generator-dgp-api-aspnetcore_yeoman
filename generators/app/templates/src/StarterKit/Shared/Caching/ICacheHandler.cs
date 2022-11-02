using System;
using System.Threading;
using System.Threading.Tasks;

namespace StarterKit.Shared.Caching
{
	public interface ICacheHandler
	{
		Task<(bool succeeded, T value)> GetOrCreateFromCacheAsync<T>(string key, Func<Task<T>> GetValueFunc,
			CancellationToken cancellationToken = default)
			where T : class;

		Task<(bool succeeded, T value)> GetFromCacheAsync<T>(string key, CancellationToken cancellationToken = default)
			where T : class;

		Task SaveToCacheAsync<T>(string key, T value, CancellationToken cancellationToken = default)
			where T : class;

		Task SaveToCacheAsync<T>(string key, T value, TimeSpan expirationTimeSpan, CancellationToken cancellationToken = default)
			where T : class;

		Task<(bool succeeded, T value)> RefreshCacheValue<T>(string key, Func<Task<T>> GetValueFunc)
			where T : class;

		bool RemoveCacheValue<T>(string key)
			where T : class;

		Task PurgeCacheAsync(string[] keys);
	}
}
