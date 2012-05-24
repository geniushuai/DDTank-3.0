using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoadTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        private RService.PlayerServiceClient client = new RService.PlayerServiceClient();

        public UnitTest1()
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
        public void TestWCF()
        {

            client.Login("tionse@hotmail.com", "E10ADC3949BA59ABBE56E057F20F883E");
        }

        [TestMethod]
        public void TestWCF1()
        {

            client.Login("tionse@hotmail.com", "E10ADC3949BA59ABBE56E057F20F883E");

        }

        [TestMethod]
        public void TestWCF2()
        {

            client.Login("tionse@hotmail.com", "E10ADC3949BA59ABBE56E057F20F883E");

        }

        [TestMethod]
        public void TestWCF3()
        {

            client.Login("tionse@hotmail.com", "E10ADC3949BA59ABBE56E057F20F883E");

        }

        [TestMethod]
        public void TestWCF4()
        {

            client.Login("tionse@hotmail.com", "E10ADC3949BA59ABBE56E057F20F883E");

        }

        [TestMethod]
        public void TestWCF5()
        {

            client.Login("tionse@hotmail.com", "E10ADC3949BA59ABBE56E057F20F883E");

        }

        
    }
}
