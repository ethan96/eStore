using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ProductHelperTest and is intended
    ///to contain all ProductHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProductHelperTest
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
        ///A test for getProductbyPartno
        ///</summary>
        [TestMethod()]
        public void getProductbyPartnoTest()
        {
            string sproductid = "ADAM-3014-AE"; // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            Product actual;
            actual = (Product) new PartHelper().getPart(sproductid, storeid);

            List<eStore.POCOS.Product.PRODUCTMARKETINGSTATUS> marketingstatus = new List<Product.PRODUCTMARKETINGSTATUS>();
            marketingstatus.Add(Product.PRODUCTMARKETINGSTATUS.HOT);
            marketingstatus.Add(Product.PRODUCTMARKETINGSTATUS.NEW);
            marketingstatus.Add(Product.PRODUCTMARKETINGSTATUS.PROMOTION);
            //actual.marketingstatus = marketingstatus;
            actual.MarketingStatus = 352;
            var cc = actual.MarketingStatus;
            var tt = actual.marketingstatus;
        }

        /// <summary>
        ///A test for getProductbyPartno
        ///</summary>
        [TestMethod()]
        public void getProductbyPartnoTest1()
        {
            
            string sproductid = "1"; // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value

            Product_Ctos actual;
            actual = (Product_Ctos)new PartHelper().getPart(sproductid, storeid);


            foreach (CTOSBOM ctbom in actual.CTOSBOMs) { 
                
            }

            
             //Console.WriteLine(typeof(Product_Ctos));

            Assert.AreEqual(Product.PRODUCTTYPE.CTOS, actual.productType);
           // Assert.AreEqual(Type.GetType("Product_Ctos"), actual.GetType());
        }
    }
}
