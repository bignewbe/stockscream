using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CommonCSharpLibary.Network;
using Newtonsoft.Json;
using PortableCSharpLib.TechnicalAnalysis;

namespace StockScream.Services
{
    public class TcpStockClient
    {
        int _numClients;
        Semaphore _sema;
        ConcurrentQueue<int> _queue;

        public bool IsConnected { get; set; }
        public ConcurrentDictionary<int, ITcpClientCommon2> Clients { get; set; }

        public TcpStockClient(int numClients)
        {
            _numClients = numClients;
            IsConnected = false;
        }

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            if (!IsConnected)
            {
                _sema = new Semaphore(_numClients, _numClients);
                Clients = new ConcurrentDictionary<int, ITcpClientCommon2>();
                _queue = new ConcurrentQueue<int>();

                for (int i = 0; i < _numClients; i++)
                {
                    var client = new CTcpClientCommon2(ip, port);
                    if (await client.ConnectAsync())
                    {
                        Clients.TryAdd(client.ClientId, client);
                        _queue.Enqueue(client.ClientId);
                    }
                    else throw new Exception("Failed to connect to server");
                }
                IsConnected = true;               //set flag such that threads can start to retrieve client from pool
            }

            return IsConnected;
        }

        public void Disconnect()
        {
            if (!IsConnected) return;

            ITcpClientCommon2 client;
            var count = Clients.Values.Count(c => c.IsConnected);
            while (count > 0)
            {
                if (TryGetAvailableClient(out client))  //this will dequeue and decrement the semaphore so that other thread will never obtain it again
                {
                    client.Disconnect();
                    --count;
                }
            }

            //we are sure all clients disconnected and _queue is empty
            //set flag to false such that no thread can obtain client anymore from GetAvailableClient
            //but thread cans till release those obtained client
            IsConnected = false;
        }

        //deque client queue and decrement semaphore counter
        //null => means either tcpserver is not connected, or all clients are currently occupied and timeout occur
        bool TryGetAvailableClient(out ITcpClientCommon2 client)
        {
            client = null;
            if (!IsConnected || !_sema.WaitOne(1000))  //not connected or timeout occurs
                return false;

            int clientId;
            if (!_queue.TryDequeue(out clientId))
                throw new Exception("Failed to get available client in GetAvailableClient");

            Debug.WriteLine("Client {0} is returned.", clientId);
            client = Clients[clientId];
            return true;
        }

        void ReleaseClient(ITcpClientCommon2 client)
        {
            if (client == null) return;
            _queue.Enqueue(client.ClientId);
            _sema.Release();
        }

        public async Task<long> RequestStystemTime()
        {
            ITcpClientCommon2 client = null;
            long result = -1;
            try
            {
                if (TryGetAvailableClient(out client))
                    result = await client.RequestServerTime();
            }
            finally
            {
                ReleaseClient(client);
            }
            return result;
        }

        public async Task<List<string>> SearchStocks(dynamic parameter)
        {
            ITcpClientCommon2 client = null;
            try {
                if (TryGetAvailableClient(out client)) {
                    var result = await client.RequestGeneralResult(parameter);
                    if (result == null) return null;

                    return result is string? JsonConvert.DeserializeObject<List<string>>(result) : result;
                }

                return null;
            }
            finally {
                ReleaseClient(client);
            }
        }
        
        public async Task<List<QuoteBasic>> RequestQuote(IList<string> symbols)
        {
            ITcpClientCommon2 client = null;
            try {
                if (TryGetAvailableClient(out client)) {                    
                    var result = await client.RequestGeneralResult(new List<string>(symbols));
                    if (result == null) return null;

                    return result is string ? JsonConvert.DeserializeObject<List<QuoteBasic>>(result) : result;
                }
                return null;
            }
            finally {
                ReleaseClient(client);
            }
        }

        public async Task<dynamic> RequestChart(string symbol)
        {
            ITcpClientCommon2 client = null;
            dynamic result = null;
            try
            {
                if (TryGetAvailableClient(out client))
                    result = await client.RequestGeneralResult(symbol);
            }
            finally
            {
                ReleaseClient(client);
            }
            return result;
        }
    }
}