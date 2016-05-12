using Microsoft.AspNet.SignalR;
using StockScream.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockScream.Hubs
{
    [AuthorizeClaims]
    //[System.Web.Mvc.Authorize]                         
    [System.Web.Mvc.ValidateAntiForgeryToken]
    public class ForexHub : Hub
    {
        //because hub is created per request we have to use static variable to store information
        //static HashSet<string> SymbolSet = new HashSet<string> { "GOOG_900", "EUR.USD_5", "EUR.GBP_5" };
        static bool IsRealTimeQuoteRequested = false;
        static Dictionary<string, int> _requested;       //keep track of requested symbols to prevent repetive request

        public ForexHub()
        {
            if (!IsRealTimeQuoteRequested)
            {
                _requested = new Dictionary<string, int>();
                Global.Instance.QuoteClient.ReceiveRealTimeForexQuote += Client_ReceiveRealTimeForexQuote;
                Global.Instance.QuoteClient.ReceiveRealTimeStockQuote += Client_ReceiveRealTimeStockQuote; ;
                Global.Instance.QuoteClient.ReceiveMsg += Client_ReceiveMsg;
                //SymbolSet = new HashSet<string>(Global.me.QuoteClient.RequestGeneralResult(new RP_AvailableSymbols()).Result as List<string>);
                IsRealTimeQuoteRequested = true;
            }
        }

        private void Client_ReceiveMsg(object sender, int code, string msg)
        {
            throw new NotImplementedException();
        }

        private void Client_ReceiveRealTimeStockQuote(object sender, string symbol, long t, double o, double c, double h, double l, double v)
        {
            //this.Clients.Group(symbol).onNewStockQuote(symbol, t, o, c, h, l, v);  //note that we dont tranmit interval here because we can handle only 7 parameters maximally
            var value = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", t, o, c, h, l, v);
            this.Clients.Group(symbol).onNewStockQuote(symbol, value);  //note that we dont tranmit interval here because we can handle only 7 parameters maximally
        }

        private void Client_ReceiveRealTimeForexQuote(object sender, string symbol, long t, double buyPrice, double sellPrice)
        {
            this.Clients.Group(symbol).onNewForexQuote(symbol, t, buyPrice, sellPrice);  
        }

        /// <summary>
        /// For clients to subscribe what symbols they want to stream real time quote. 
        /// Immediately called after hub is started. 
        /// </summary>
        /// <param name="symbolStr"></param>
        public void Subscribe(string symbolStr)
        {
            if (string.IsNullOrEmpty(symbolStr)) return;
            var symbols = Regex.Split(symbolStr, @"[;,\s+]").Select(s => s.Trim()).Where(s=>!string.IsNullOrEmpty(s)).Select(s=>s.ToUpper()).ToList();
            if (symbols == null) return;
            
            lock (_requested)
            {
                var subscribedSymbols = string.Empty;            
                foreach (var symbol in symbols)
                {
                    if (!Global.Instance.AvailableSymbolsForStreaming.Contains(symbol)) continue;                //we cannot subscribe symbols that are not in the set

                    subscribedSymbols += symbol + ";";

                    Groups.Add(Context.ConnectionId, symbol);
                    if (!_requested.ContainsKey(symbol))                      //if not yet requested to the quote server, make the request 
                    {
                        _requested.Add(symbol, 1);
                        Global.Instance.QuoteClient.RequestRealTimeQuote(symbol);
                    }
                }
                Clients.Client(Context.ConnectionId).onSubscribe(subscribedSymbols);   //indicate to client what symbols are requested
            }
        }

        public void UnSubscribe(string symbolStr)
        {
            if (string.IsNullOrEmpty(symbolStr)) return;
            var symbols = Regex.Split(symbolStr, @"[;,\s+]").Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).Select(s => s.ToUpper()).ToList();
            if (symbols == null) return;

            lock (_requested)
            {
                var unSubscribedSymbols = string.Empty;
                foreach (var symbol in symbols)
                {
                    if (!Global.Instance.AvailableSymbolsForStreaming.Contains(symbol)) continue;                //we cannot unsubscribe symbols that are not in the set

                    unSubscribedSymbols += symbol + ";";
                    Groups.Remove(Context.ConnectionId, symbol);
                }
                Clients.Client(Context.ConnectionId).onUnSubscribe(unSubscribedSymbols);                   //indicate to client what symbols are requested
            }
        }

        public override Task OnConnected()
        {
            //string name = Context.User.Identity.Name;
            //Groups.Add(Context.ConnectionId, name);
            //var user = Context.User;
            //Groups.Add(Context.ConnectionId, "a");
            //Groups.Add(Context.ConnectionId, "b");
            return base.OnConnected();                        
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }

    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();
        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}
