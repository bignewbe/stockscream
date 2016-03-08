using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using CommonCSharpLibary.CustomExtensions;
using NLog;
using NLog.Targets;
using StockScream.Models;
using CommonCSharpLibary.Network;
using StockScream.Identity;
using PortableCSharpLib;
using CommonCSharpLibary.StockAnalysis;
using StockScream.DataModels;
using PortableCSharpLib.NetworkRequestParameters;
using CommonCSharpLibary.Serialize;
using Newtonsoft.Json;

namespace StockScream.Services
{
    public class Global
    {
        HashSet<string> _symbols;

        public HashSet<string> AvailableSymbolsForStreaming { get; set; }
        public ITcpClientCommon2 QuoteClient { get; set; }
        public TcpStockClient StockClient { get; set; }
        public ConcurrentDictionary<string, Token> ConfirmationTokens { get; set; }
        public StockMapping MapStock { get; set; }
        public WParamMapping MapWParam { get; set; }
        public Logger logger;

        public static Global me;
        public static void Initialize(string stockMapPath, string logPath)
        {
            me = new Global(stockMapPath, logPath);
        }

        public Global(string stockMapPath, string logPath)
        {
            //logger = LogManager.GetCurrentClassLogger();
            logger = ConfigureNLLogger(logPath);
            logger.Info(string.Format("NL logger configured with path = {0}.", logPath));

            ConfirmationTokens = new ConcurrentDictionary<string, Token>();

            //initialize email service
            var host = ConfigurationManager.AppSettings["smtpServer"];
            var port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            var enableSsl = bool.Parse(ConfigurationManager.AppSettings["enableSsl"]);
            var userName = ConfigurationManager.AppSettings["mailAccount"];
            var password = ConfigurationManager.AppSettings["mailPassword"];
            var fromEmail = ConfigurationManager.AppSettings["adminEmail"];
            EmailService.Initialize(host, port, userName, password, enableSsl, fromEmail);
            logger.Info("Email service initialized");

            //initialize stock server
            var ip_stockServer = ConfigurationManager.AppSettings["ip_stockServer"];
            var port_stockServer = int.Parse(ConfigurationManager.AppSettings["port_stockServer"]);
            StockClient = new TcpStockClient(10);
            var r = StockClient.ConnectAsync(ip_stockServer, port_stockServer).Result;
            logger.Info(string.Format("Stock server connected {0}:{1}", ip_stockServer, port_stockServer));

            //initialize real time quote server
            var ip_quoteServer = ConfigurationManager.AppSettings["ip_quoteServer"];
            var port_quoteServer = int.Parse(ConfigurationManager.AppSettings["port_quoteServer"]);
            QuoteClient = new CTcpClientCommon2(ip_quoteServer, port_quoteServer);
            r = QuoteClient.ConnectAsync().Result;
            logger.Info(string.Format("Quote server connected {0}:{1}", ip_quoteServer, port_quoteServer));

            var s = QuoteClient.RequestGeneralResult(new RP_AvailableSymbols()).Result;
            var symbols = s is string ? JsonConvert.DeserializeObject<List<string>>(s) : s;
            AvailableSymbolsForStreaming = new HashSet<string>(symbols);

            //create wparam mapping
            MapWParam = WParam.GetWParamMap();

            //load stock mapping
            var mapStock = new StockMapping();
            ObjectSerializer.DeSerializeObject(stockMapPath, ref mapStock);
            MapStock = mapStock;
            logger.Info(string.Format("Stockmapping loaded from {0}", stockMapPath));

            //load all stock symbols from database for fast server side validation
            _symbols = new HashSet<string>(LoadSymbolsFromDataBase(new StockDbContext()));
            logger.Info("Stock symbols retrived from data base");
        }

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

        //load all stock symbols from database for fast server side validation
        static IEnumerable<string> LoadSymbolsFromDataBase(StockDbContext db)
        {
            var symbols1 = (from s in db.StockFinancials select s.Symbol).ToList();
            var symbols2 = (from s in db.PPStocks select s.Symbol).ToList();
            return symbols1.Intersect(symbols2);
        }
    }

    //public class Globals
    //{
    //    static HashSet<string> _symbols;
    //    public static ITcpClientCommon2 QuoteClient { get; set; }
    //    public static TcpStockClient StockClient { get; set; }
    //    public static ConcurrentDictionary<string, Token> ConfirmationTokens { get; set; }
    //    public static StockMapping MapStock { get; set; }
    //    public static WParamMapping MapWParam { get; set; }
    //    public static Logger logger;

    //    public static void InitializeGlobals(string stockMapPath, string logPath)
    //    {
    //        //logger = LogManager.GetCurrentClassLogger();
    //        logger = ConfigureNLLogger(logPath);
    //        logger.Info(string.Format("NL logger configured with path = {0}.", logPath));

    //        ConfirmationTokens = new ConcurrentDictionary<string, Token>();

    //        //initialize email service
    //        var host = ConfigurationManager.AppSettings["smtpServer"];
    //        var port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
    //        var enableSsl = bool.Parse(ConfigurationManager.AppSettings["enableSsl"]);
    //        var userName = ConfigurationManager.AppSettings["mailAccount"];
    //        var password = ConfigurationManager.AppSettings["mailPassword"];
    //        var fromEmail = ConfigurationManager.AppSettings["adminEmail"];
    //        EmailService.Initialize(host, port, userName, password, enableSsl, fromEmail);
    //        logger.Info("Email service initialized");

    //        //initialize stock server
    //        var ip_stockServer = ConfigurationManager.AppSettings["ip_stockServer"];
    //        var port_stockServer = int.Parse(ConfigurationManager.AppSettings["port_stockServer"]);
    //        StockClient = new TcpStockClient(10);
    //        var r = StockClient.ConnectAsync(ip_stockServer, port_stockServer).Result;
    //        logger.Info(string.Format("Stock server connected {0}:{1}", ip_stockServer, port_stockServer));

    //        //initialize real time quote server
    //        var ip_quoteServer = ConfigurationManager.AppSettings["ip_quoteServer"];
    //        var port_quoteServer = int.Parse(ConfigurationManager.AppSettings["port_quoteServer"]);
    //        QuoteClient = new CTcpClientCommon2(ip_quoteServer, port_quoteServer);
    //        r = QuoteClient.ConnectAsync().Result;
    //        logger.Info(string.Format("Quote server connected {0}:{1}", ip_quoteServer, port_quoteServer));

    //        //create wparam mapping
    //        MapWParam = WParam.GetWParamMap();

    //        //load stock mapping
    //        var mapStock = new StockMapping();
    //        CommonCSharpLibary.Serialize.ObjectSerializer.DeSerializeObject(stockMapPath, ref mapStock);
    //        MapStock = mapStock;
    //        logger.Info(string.Format("Stockmapping loaded from {0}", stockMapPath));

    //        //load all stock symbols from database for fast server side validation
    //        _symbols = new HashSet<string>(LoadSymbolsFromDataBase());
    //        logger.Info("Stock symbols retrived from data base");
    //    }
    //    public static bool IsValidSymbol(string symbol)
    //    {
    //        return _symbols.Contains(symbol);
    //    }

    //    /// <summary>
    //    /// configure NL logger path
    //    /// </summary>
    //    /// <param name="logPath"></param>
    //    static Logger ConfigureNLLogger(string logPath)
    //    {
    //        if (logPath == null) return null;
    //        //if (!Directory.Exists(logPath))
    //        //{
    //        //    if (MessageBox.Show(string.Format("{0} does not exist. do you want to create it?", logPath), "Log Path Not Exist", MessageBoxButtons.YesNo) == DialogResult.Yes)
    //        //        Directory.CreateDirectory(logPath);
    //        //    else return null;
    //        //}

    //        if (!Directory.Exists(logPath))
    //            Directory.CreateDirectory(logPath);

    //        var year = DateTime.Now.Year;
    //        var weekNo = DateTime.UtcNow.GetIso8601WeekOfYear();
    //        foreach (var t in new List<string> { "Info", "Error", "Trace" })
    //        {
    //            var target = (FileTarget)LogManager.Configuration.FindTargetByName("file" + t);
    //            if (target != null)
    //                target.FileName = logPath.Replace("\\", "/") + string.Format("/{0}{1}_{2}.log", year, weekNo.ToString("D2"), t.ToLower());
    //        }

    //        LogManager.ReconfigExistingLoggers();
    //        return LogManager.GetCurrentClassLogger();
    //    }

    //    //load all stock symbols from database for fast server side validation
    //    static IEnumerable<string> LoadSymbolsFromDataBase()
    //    {
    //        var db = new StockDbContext();
    //        var symbols1 = (from s in db.StockFinancials select s.Symbol).ToList();
    //        var symbols2 = (from s in db.PPStocks select s.Symbol).ToList();
    //        return symbols1.Intersect(symbols2);
    //    }
    //}
}