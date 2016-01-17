using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using CommonCSharpLibary.CustomExtensions;
using CommonCSharpLibary.Stock;
using NLog;
using NLog.Targets;
using StockScream.Models;
using CommonCSharpLibary.Network;
using System.Windows.Forms;
using StockScream.Identity;
using StockScream.DataModels;
using PortableCSharpLib;

namespace StockScream.Services
{
    public class Globals
    {
        static HashSet<string> _symbols;
        public static ITcpClientCommon2 QuoteClient { get; set; }
        public static TcpStockClient StockClient { get; set; }
        public static Logger logger;
        public static ConcurrentDictionary<string, Token> ConfirmationTokens { get; set; }
        public static StockMapping MapStock { get; set; }
        public static WParamMapping MapWParam { get; set; }
        //public static CBasicStockInfo BasicStockInfo { get; set; }
        //public static ConcurrentDictionary<string, UserProfile> Users;

        public static void InitializeGlobals(string stockMapPath, string logPath)
        {
            ConfirmationTokens = new ConcurrentDictionary<string, Token>();

            //initialize email service
            var host = ConfigurationManager.AppSettings["smtpServer"];
            var port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            var enableSsl = bool.Parse(ConfigurationManager.AppSettings["enableSsl"]);
            var userName = ConfigurationManager.AppSettings["mailAccount"];
            var password = ConfigurationManager.AppSettings["mailPassword"];
            var fromEmail = ConfigurationManager.AppSettings["adminEmail"];
            EmailService.Initialize(host, port, userName, password, enableSsl, fromEmail);

            //initialize stock server
            var ip_stockServer = ConfigurationManager.AppSettings["ip_stockServer"];
            var port_stockServer = int.Parse(ConfigurationManager.AppSettings["port_stockServer"]);
            StockClient = new TcpStockClient(10);
            var r = StockClient.ConnectAsync(ip_stockServer, port_stockServer).Result;

            //initialize real time quote server
            var ip_quoteServer = ConfigurationManager.AppSettings["ip_quoteServer"];
            var port_quoteServer = int.Parse(ConfigurationManager.AppSettings["port_quoteServer"]);
            QuoteClient = new CTcpClientCommon2(ip_quoteServer, port_quoteServer);
            r = QuoteClient.ConnectAsync().Result;
            
            //create WParam mapping
            MapWParam = WParam.GetWParamMap();

            //load stock mapping
            var mapStock = new StockMapping();
            CommonCSharpLibary.Serialize.ObjectSerializer.DeSerializeObject(stockMapPath, ref mapStock);
            MapStock = mapStock;
            
            //load all stock symbols from database for fast server side validation
            _symbols = new HashSet<string>(LoadSymbolsFromDataBase());

            //logger = LogManager.GetCurrentClassLogger();
            logger = ConfigureNLLogger(logPath);
        }

        public static bool IsValidSymbol(string symbol)
        {
            return _symbols.Contains(symbol);
        }

        /// <summary>
        /// configure NL logger path
        /// </summary>
        /// <param name="logPath"></param>
        static Logger ConfigureNLLogger(string logPath)
        {
            if (logPath == null) return null;
            if (!Directory.Exists(logPath))
            {
                if (MessageBox.Show(string.Format("{0} does not exist. do you want to create it?", logPath), "Log Path Not Exist", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Directory.CreateDirectory(logPath);
                else return null;
            }

            var year = DateTime.Now.Year - 2000;
            var weekNo = DateTime.UtcNow.GetIso8601WeekOfYear();

            foreach (var t in new List<string> { "Info", "Error", "Trace" })
            {
                var target = (FileTarget)LogManager.Configuration.FindTargetByName("file" + t);
                target.FileName = logPath.Replace("\\", "/") + string.Format("/{0}{1,2}_{2}.log", year, weekNo, t.ToLower());
            }

            LogManager.ReconfigExistingLoggers();
            return LogManager.GetCurrentClassLogger();
        }

        //load all stock symbols from database for fast server side validation
        static IEnumerable<string> LoadSymbolsFromDataBase()
        {
            var db = new StockDbContext();
            var symbols1 = (from s in db.StockFinancials select s.Symbol).ToList();
            var symbols2 = (from s in db.PPStocks select s.Symbol).ToList();
            return symbols1.Intersect(symbols2);
        }
    }
}