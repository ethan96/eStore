using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;
using eStore.POCOS.DAL;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for PackingMethodTest and is intended
    ///to contain all PackingMethodTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PackingMethodTest
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
        ///A test for getCtosBox
        ///</summary>
        [TestMethod()]
        public void getCtosBoxTest()
        {
            PackingMethod target = new PackingMethod(); // TODO: Initialize to an appropriate value
            eStore.POCOS.Store store = new StoreHelper().getStorebyStoreid("AUS");
            Product_Ctos ctos = (Product_Ctos)new PartHelper().getPart("1134", store);
            Decimal dimensionalWeightBase = new Decimal(0); // TODO: Initialize to an appropriate value
           
            string defaultCurrency = store.defaultCurrency.CurrencyID;
        }
    }
}
