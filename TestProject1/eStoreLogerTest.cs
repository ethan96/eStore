using eStore.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
 

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for eStoreLogerTest and is intended
    ///to contain all eStoreLogerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class eStoreLogerTest
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
        ///A test for Fatal
        ///</summary>
        [TestMethod()]
        public void FatalTest()
        {
          
            string message = "test erro" ; // TODO: Initialize to an appropriate value
            Exception ex = new NotImplementedException("test");

            eStoreLoger.Fatal(message, "", "", "", ex);
            eStoreLoger.Fatal(message);
            eStoreLoger.Warn(message);
            eStoreLoger.Info(message);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");


        }
    }
}
