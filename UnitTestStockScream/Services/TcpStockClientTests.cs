using CommonCSharpLibary.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockScream.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StockScream.Services.Tests
{
    [TestClass()]
    public class TcpStockClientTests
    {
        static CTcpServerCommon2 server;
        static string ip;
        static int port;

        [ClassInitialize]
        public static void Prepare(TestContext context)
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName()).ToList();
            ip = addresses.Find(p => p.AddressFamily == AddressFamily.InterNetwork).ToString();
            port = 11000;
            server = new CTcpServerCommon2(ip, port);
            server.Start();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            server.Dispose();
        }


        [TestMethod()]
        public void ConnectAsyncTest()
        {
            var client = new TcpStockClient(10);

            Assert.IsTrue(client.ConnectAsync(ip, port).Result);
            Assert.IsTrue(client.IsConnected);
            Assert.IsTrue(client.Clients.Count == 10);

            client.Disconnect();
            Assert.IsFalse(client.IsConnected);
        }

        //[TestMethod()]
        //public void TcpStockClientTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void DisconnectTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void RequestStystemTimeTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void SearchStocksTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void RequestQuoteTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void RequestChartTest()
        //{
        //    Assert.Fail();
        //}
    }
}