using esUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.IO;
using System.Configuration;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for EMailTest and is intended
    ///to contain all EMailTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EMailTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for sendTestMail
        ///</summary>
        [TestMethod()]
        public void sendTestMailTest()
        {
            EMail target = new EMail(); // TODO: Initialize to an appropriate value
            target.sendTestMail();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod()]
        public void myTest()
        {
            //StringBuilder filePath = new StringBuilder();
            //String fullPath = null;
            //string storeID = "AUS";
            //filePath.Append(AppDomain.CurrentDomain.BaseDirectory);
            //filePath.Append(ConfigurationSettings.AppSettings.Get("Mail_Path")).Append("/").Append(storeID).Append("/");
            //Console.WriteLine("file path:" + filePath);
            Console.WriteLine(DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"));
        }
    }
}
