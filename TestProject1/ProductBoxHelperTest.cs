using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ProductBoxHelperTest and is intended
    ///to contain all ProductBoxHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProductBoxHelperTest
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
        ///A test for getProductBoxByPartno
        ///</summary>
        [TestMethod()]
        public void getProductBoxByPartnoTest()
        {
            string partno = "PCA-6108-A1E"; // TODO: Initialize to an appropriate value
            Store _store = new StoreHelper().getStorebyHostname("buy.advantech.com"); // TODO: Initialize to an appropriate value
          
            ProductBox actual;
            actual = new ProductBoxHelper().getProductBoxByPartno(partno, _store);

            Assert.IsTrue(actual != null);
            Console.WriteLine(actual.HighINCH + ":" + actual.WidthINCH + ":" + actual.LengthINCH);

           
   
        }

        /// <summary>
        ///A test for getDefaultBox
        ///</summary>
        [TestMethod()]
        public void getDefaultBoxTest()
        {
            Store _store = new StoreHelper().getStorebyHostname("buy.advantech.com.my");
            ProductBox actual;
            actual = new ProductBoxHelper().getDefaultBox(_store);

            Console.WriteLine(actual.HighINCH + ":" + actual.WidthINCH + ":" + actual.LengthINCH);
             
        }

  
    }
}
