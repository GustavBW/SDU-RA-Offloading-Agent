using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonitoringDaemon;

public class HttpClientPool
{
    private readonly Queue<HttpClient> _clients;
    private readonly SemaphoreSlim _poolSemaphore;
    private readonly int _poolSize;

    public HttpClientPool(int poolSize)
    {
        _poolSize = poolSize;
        _clients = new Queue<HttpClient>();
        _poolSemaphore = new SemaphoreSlim(poolSize, poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            _clients.Enqueue(new HttpClient());
        }
    }

    /**
     * Retrieves a Client from the pool concurrently, then handles the request in parallel using the callback. <br/>
     * Exceptions and non-200 status codes are ignored unless handlers are provided.
     */
    public async Task SendRequestAsync<T>(string requestUri, Action<T> onResponse, Action<Exception> onException = default, Action onNot200 = default)
    {
        onException ??= e => {}; // Default to empty lambda if null
        onNot200 ??= () => {}; // Default to empty lambda if null
        
        await _poolSemaphore.WaitAsync();

        HttpClient client;
        lock (_clients)
        {
            client = _clients.Dequeue();
        }

        try
        {
            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<T>(jsonString);
                onResponse(responseData);
            }
            else
            {
                onNot200();
            }
        }
        catch (Exception ex)
        {
            onException(ex);
        }
        finally
        {
            lock (_clients)
            {
                _clients.Enqueue(client);
            }
            _poolSemaphore.Release();
        }
    }

    public void Dispose()
    {
        lock (_clients)
        {
            while (_clients.Count > 0)
            {
                var client = _clients.Dequeue();
                client.Dispose();
            }
        }
    }
    
}