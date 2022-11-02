using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StarterKit.Shared.Options;

namespace StarterKit.Shared.Caching
{
	public class LocalCacheHandler : ICacheHandler
	{
		protected readonly AppSettings _appSettings;

		private readonly IMemoryCache _cache;

		public LocalCacheHandler(IMemoryCache cache,
			IOptions<AppSettings> appSettings)
		{
			_cache = cache ??
			         throw new ArgumentNullException(
				         $"{GetType().Name}.Ctr - Argument {nameof(cache)} cannot be null.");
			_appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
		}

		/// <summary>
		/// get cached value or retrieve the necessary value with the supplied function and cache the retrieved value
		/// </summary>
		public async Task<(bool succeeded, T value)> GetOrCreateFromCacheAsync<T>(string key,
			Func<Task<T>> GetValueFunc, CancellationToken cancellationToken = default)
			where T : class
		{
			var cachedResult = await _cache.GetOrCreateAsync(key, async entry =>
			{
				// keep in cache for fixed period; no sliding expiration, otherwise changes may never be picked up
				entry.AbsoluteExpiration = DateTime.Now.AddMinutes(_appSettings.CacheExpiration);

				return await GetValueFunc();
			});

			return (succeeded: true, value: cachedResult);
		}

		public Task<(bool succeeded, T value)> GetFromCacheAsync<T>(string key,
			CancellationToken cancellationToken = default)
			where T : class
		{
			var succeeded = _cache.TryGetValue(key, out T result);

			if (succeeded)
				return Task.FromResult((succeeded, value: result));
			return Task.FromResult((succeeded, value: default(T)));
		}

		public Task SaveToCacheAsync<T>(string key, T value, CancellationToken cancellationToken = default)
			where T : class
		{
			var relativeTime = new TimeSpan(hours: 0, minutes: _appSettings.CacheExpiration, seconds: 0);
			_cache.Set(key, value, relativeTime);

			return Task.CompletedTask;
		}

		public Task SaveToCacheAsync<T>(string key, T value, TimeSpan expirationTimeSpan, CancellationToken cancellationToken = default)
			where T : class
		{
			_cache.Set(key, value, expirationTimeSpan);

			return Task.CompletedTask;
		}

		public async Task<(bool succeeded, T value)> RefreshCacheValue<T>(string key, Func<Task<T>> GetValueFunc)
			where T : class
		{
			var result = await GetValueFunc();

			_cache.Set(key, result, DateTime.Now.AddMinutes(_appSettings.CacheExpiration));

			return (succeeded: true, value: result);
		}

		/// <summary>
		/// retrieve value from cache
		/// </summary>
		public bool RemoveCacheValue<T>(string cacheKey)
			where T : class
		{
			if (_cache.TryGetValue<T>(cacheKey, out _))
			{
				_cache.Remove(cacheKey);
				return true;
			}

			return false;
		}

		public Task PurgeCacheAsync(string[] keys)
		{
			if (keys == null || keys.Length < 1) return Task.CompletedTask;

			var numberOfKeys = keys.Length;
			for (var i = 0; i < numberOfKeys; i++)
			{
				_cache.Remove(keys[i]);
			}

			return Task.CompletedTask;
		}
	}
}
