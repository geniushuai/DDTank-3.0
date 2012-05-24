using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Collections;

namespace Road.Test
{
    /// <summary>
    /// Summary description for TestCollection
    /// </summary>
    [TestClass]
    public class TestCollection
    {
        public TestCollection()
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

        private Dictionary<string, string> test = new Dictionary<string, string>();
        private string[] arrTest = new string[3000];
        private ArrayList list = new ArrayList();

        [TestInitialize]
        public void Setup()
        {
            for (int i = 0; i < 3000; i++)
            {
                test.Add(i.ToString(), i.ToString());
                arrTest[i] = i.ToString();
                list.Add(i.ToString());
            }
        }

        [TestMethod]
        public void TestToArrayPeformance()
        {
            string[] temp = test.Keys.ToArray();

        }

        [TestMethod]
        public void TestArrayCopy()
        {
            string[] temp = (string[])arrTest.Clone();
        }

        [TestMethod]
        public void TestDicReadPeformance()
        {
            for (int i = 0; i < 3000; i++)
            {
                test[i.ToString()] = "tttt";
            }
        }

        [TestMethod]
        public void TestArrReadPerformance()
        {
            string temp = null;
            for (int i = 0; i < 3000; i++)
            {
                temp = arrTest[i];
            }
        }

        [TestMethod]
        public void TestForechPerformance()
        {
            string temp = null;
            foreach (string s in arrTest)
            {
                temp = s;
            }
        }

        [TestMethod]
        public void TestArrayListAddPerformance()
        {
            list = new ArrayList();
            for (int i = 0; i < 3000; i++)
            {
                list.Add(i);
            }
        }

        [TestMethod]
        public void TestArrayListRemovePerformance()
        {
            for (int i = 0; i < 3000; i++)
            {
                list.Remove(i.ToString());
            }
        }

        [TestMethod]
        public void TestDictionaryAddPerformance()
        {
            test = new Dictionary<string, string>();
            for (int i = 0; i < 3000; i++)
            {
                test.Add(i.ToString(), i.ToString());
            }
 
        }

        [TestMethod]
        public void TestDictionaryRemovePerformance()
        {
            for (int i = 0; i < 3000; i++)
            {
                test.Remove(i.ToString());
            }
        }

        [TestMethod]
        public void TestDictonary()
        {

            Console.WriteLine(test["1111111"]);
        }
    }
}
