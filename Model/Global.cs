using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using NLog;
using NLog.Targets;
using CommonCSharpLibary.Network;
using PortableCSharpLib;
using CommonCSharpLibary.StockAnalysis;
using StockScream.DataModels;
using PortableCSharpLib.NetworkRequestParameters;
using Newtonsoft.Json;
using CommonCSharpLibary.Serialize;

namespace StockScream.Services
{
    public class Global
    {
        public static Global Instance;
        static Global()
        {
            Instance = new Global();
        }

        public HashSet<string> AvailableSymbolsForTechAnalysis { get; set; }
        public HashSet<string> AvailableSymbolsForStreaming { get; private set; }
        public ITcpClientCommon2 QuoteClient { get; private set; }
        public TcpStockClient StockClient { get; private set; }
        public StockMapping MapStock { get; private set; }
        public WParamMapping MapWParam { get; private set; }
        public Logger NLLogger { get; private set; }
        public ConcurrentDictionary<string, Token> ConfirmationTokens { get; private set; } = new ConcurrentDictionary<string, Token>();

        public void ConnectQuoteServer(string ip, int port)
        {
            QuoteClient = new CTcpClientCommon2(ip, port);
            var r = QuoteClient.ConnectAsync().Result;
            NLLogger.Info(string.Format("{0} to Quote Server at {1}:{2}", (r? "Connected" : "Failed to connect"), ip, port));            
            if (r) {
                var s = QuoteClient.RequestGeneralResult(new RP_AvailableSymbols()).Result;
                var symbols = s is string ? JsonConvert.DeserializeObject<List<string>>(s) : s;
                AvailableSymbolsForStreaming = new HashSet<string>(symbols);
            }
        }

        public void ConnectStockServer(string ip, int port, int numConnections=10)
        {
            StockClient = new TcpStockClient(numConnections);
            var r = StockClient.ConnectAsync(ip, port).Result;
            NLLogger.Info(string.Format("{0} to Stock Server at {1}:{2}", (r? "Connected" : "Failed to connect"), ip, port));
        }

        //public virtual void GetSymbolsAvailableForTechAnalysis()
        //{
        //    ////load all stock symbols from database for fast server side validation
        //    //Symbols = new HashSet<string>(LoadSymbolsFromDataBase(new StockDbContext()));
        //    //logger.Info("Stock symbols retrived from data base");
        //}

        public void InitializeNLLogger(string logPath)
        {
            NLLogger = ConfigureNLLogger(logPath);
            NLLogger.Info(string.Format("NL logger configured with path = {0}.", logPath));
        }
        public void LoadStockMeta(string fileStockMeta)
        {
            //create wparam mapping
            MapWParam = WParam.GetWParamMap();

            //load stock mapping
            var mapStock = new StockMapping();
            ObjectSerializer.DeSerializeObject(fileStockMeta, ref mapStock);
            MapStock = mapStock;

            NLLogger.Info(string.Format("Stockmapping loaded from {0}", fileStockMeta));
        }

        //public Global(string stockMapPath, string logPath)
        //{
        //    //logger = LogManager.GetCurrentClassLogger();
        //    logger = ConfigureNLLogger(logPath);
        //    logger.Info(string.Format("NL logger configured with path = {0}.", logPath));

        //    ConfirmationTokens = new ConcurrentDictionary<string, Token>();

        //    //initialize email service
        //    var host = ConfigurationManager.AppSettings["smtpServer"];
        //    var port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
        //    var enableSsl = bool.Parse(ConfigurationManager.AppSettings["enableSsl"]);
        //    var userName = ConfigurationManager.AppSettings["mailAccount"];
        //    var password = ConfigurationManager.AppSettings["mailPassword"];
        //    var fromEmail = ConfigurationManager.AppSettings["adminEmail"];
        //    EmailService.Initialize(host, port, userName, password, enableSsl, fromEmail);
        //    logger.Info("Email service initialized");

        //    //initialize stock server
        //    var ip_stockServer = ConfigurationManager.AppSettings["ip_stockServer"];
        //    var port_stockServer = int.Parse(ConfigurationManager.AppSettings["port_stockServer"]);
        //    StockClient = new TcpStockClient(10);
        //    var r = StockClient.ConnectAsync(ip_stockServer, port_stockServer).Result;
        //    logger.Info(string.Format("Stock server connected {0}:{1}", ip_stockServer, port_stockServer));

        //    //initialize real time quote server
        //    var ip_quoteServer = ConfigurationManager.AppSettings["ip_quoteServer"];
        //    var port_quoteServer = int.Parse(ConfigurationManager.AppSettings["port_quoteServer"]);
        //    QuoteClient = new CTcpClientCommon2(ip_quoteServer, port_quoteServer);
        //    r = QuoteClient.ConnectAsync().Result;
        //    logger.Info(string.Format("Quote server connected {0}:{1}", ip_quoteServer, port_quoteServer));

        //    var s = QuoteClient.RequestGeneralResult(new RP_AvailableSymbols()).Result;
        //    var symbols = s is string ? JsonConvert.DeserializeObject<List<string>>(s) : s;
        //    AvailableSymbolsForStreaming = new HashSet<string>(symbols);

        //    //create wparam mapping
        //    MapWParam = WParam.GetWParamMap();

        //    //load stock mapping
        //    var mapStock = new StockMapping();
        //    ObjectSerializer.DeSerializeObject(stockMapPath, ref mapStock);
        //    MapStock = mapStock;
        //    logger.Info(string.Format("Stockmapping loaded from {0}", stockMapPath));

        //    //load all stock symbols from database for fast server side validation
        //    _symbols = new HashSet<string>(LoadSymbolsFromDataBase(new StockDbContext()));
        //    logger.Info("Stock symbols retrived from data base");
        //}

        /// <summary>
        /// configure NL logger path
        /// </summary>
        /// <param name="logPath"></param>
        static Logger ConfigureNLLogger(string logPath)
        {
            if (logPath == null) return null;

            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            var year = DateTime.Now.Year;
            var weekNo = DateTime.UtcNow.GetIso8601WeekOfYear();
            foreach (var t in new List<string> { "Info", "Error", "Trace" }) {
                var target = (FileTarget)LogManager.Configuration.FindTargetByName("file" + t);
                if (target != null)
                    target.FileName = logPath.Replace("\\", "/") + string.Format("/{0}{1}_{2}.log", year, weekNo.ToString("D2"), t.ToLower());
            }

            LogManager.ReconfigExistingLoggers();
            return LogManager.GetCurrentClassLogger();
        }

        ////load all stock symbols from database for fast server side validation
        //static IEnumerable<string> LoadSymbolsFromDataBase(StockDbContext db)
        //{
        //    var symbols1 = (from s in db.StockFinancials select s.Symbol).ToList();
        //    var symbols2 = (from s in db.PPStocks select s.Symbol).ToList();
        //    return symbols1.Intersect(symbols2);
        //}
    }
}