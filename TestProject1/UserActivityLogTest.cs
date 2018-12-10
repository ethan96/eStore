using eStore.POCOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for UserActivityLogTest and is intended
    ///to contain all UserActivityLogTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserActivityLogTest
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
        ///A test for save
        ///</summary>
        [TestMethod()]
        public void saveTest()
        {
            UserActivityLog target = new UserActivityLog(); // TODO: Initialize to an appropriate value
            target.CategoryID = "test";
            target.CategoryName = "test";
            target.ProductID = "PCM-233C-00A1E";
            target.CategoryType = Product.PRODUCTTYPE.STANDARD.ToString();
            target.CreatedDate = DateTime.Now;
            target.ReferredURL = "http://www.advantech.com";
            target.SessionID = "xxxxxx";
            target.UserId  = "edward.keh@advantech.com.tw";
            target.URL = "http://buy.advantech.com/default.aspx";

            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.save();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
