using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using zlib;
using Game.Base;
using System.Threading;

namespace Road.Test
{
    /// <summary>
    /// Summary description for TestZip
    /// </summary>
    [TestClass]
    public class TestZip
    {
        public TestZip()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Compress()
        {
            //压缩
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes("ul{\"state\":1,\"nick\":\"小龙\",\"sex\":true,\"email\":\"tionase@hotmail.com\",\"exp\":109040,\"isguest\":false,\"hotpoint\":26,\"money\":7099,\"styletype\":\"stand\",\"style\":\"2/2/2/1\",\"id\":\"2ef38760-9000-4f10-b96b-af4da8c656ae\",\"vertify\":1,\"level\":12,\"sign\":\"小龙\",\"score\":6323}");
            MemoryStream ms = new MemoryStream();
            Stream s = new ZOutputStream(ms, 9);

            s.Write(byteData, 0, byteData.Length);
            s.Close();
            byte[] compressData = (byte[])ms.ToArray();
            ms.Flush();
            ms.Close();
            Console.WriteLine(compressData.Length);

            //解压
            MemoryStream md = new MemoryStream();
            Stream d = new ZOutputStream(md);
            d.Write(compressData, 0, compressData.Length);
            d.Close();
            byte[] dompressData = (byte[])md.ToArray();
            md.Flush();
            md.Close();

            Console.WriteLine(dompressData.Length);
        }

        [TestMethod]
        public void TestCompress()
        {
            byte[] byteDate = Encoding.UTF8.GetBytes("sdjflsdfkiijksjfdfs");
            Console.Write(Marshal.ToHexDump("Compress",Marshal.Uncompress(Marshal.Compress(byteDate))));
 
        }

        [TestMethod]
        public void TestRamdom()
        {
            Random rd = new Random();
            for (int i = 0; i < 100; i++)
            {
                Console.Write(rd.Next(2));
            }

        }
    }
}
