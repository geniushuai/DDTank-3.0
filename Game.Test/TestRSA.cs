using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Configuration;

namespace Road.Test
{
    /// <summary>
    /// Summary description for TestRSA
    /// </summary>
    [TestClass]
    public class TestRSA
    {
        public TestRSA()
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
        public void TestMethod1()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string pk = ConfigurationSettings.AppSettings["privateKey"];
            rsa.FromXmlString(pk);

            //byte[] cry = Encoding.UTF8.GetBytes("tionase,111111,2321");

            //byte[] t2 = rsa.Encrypt(cry,false);
            //string acry = Convert.ToBase64String(t2);

            //Console.WriteLine(acry);

            string p = "zFREtEEC12vPh8CXmGRPoRZjQjb44nvJJwunxtmM8DJcx3znaeYCEfhTlsxCBXWkmEoDnCvul/JNHirKkxf5AW39N/6C0DbAC0m6ipck2nCmkmSTVxzc/2T268Qb+5gykBeZKZKf7l0sR2mdZ9Btcdx1uu+9EAf+raXQCO1ML7s=";
          
            byte[] rgb = Convert.FromBase64String(p);

            string temp = Encoding.UTF8.GetString(rsa.Decrypt(rgb,false));

            //Console.WriteLine(temp);
        }

        [TestMethod]
        public void TestFormatString()
        {
            string line = "hello:";
            string[] splitted = new string[2];
            splitted[0] = line.Substring(0, line.IndexOf(':'));
            splitted[1] = line.Substring(line.IndexOf(':') + 1);

            splitted[1] = splitted[1].Replace("\t", "");

            string result = splitted[1];
            string[] args = new string[0] { };
            try
            {
                result = string.Format(result,args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("result="+result);
            Assert.IsNull(result);
        }
    }
}
