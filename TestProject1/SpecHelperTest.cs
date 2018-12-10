using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for SpecHelperTest and is intended
    ///to contain all SpecHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpecHelperTest
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
        ///A test for getProductsByCategoryID
        ///</summary>
        [TestMethod()]
        public void getProductsByCategoryIDTest()
        {
            SpecHelper target = new SpecHelper(); // TODO: Initialize to an appropriate value
            string categoryid = "DAQ_USB_IO_Modules"; // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            eStore.BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);

           ProductSpecRules  actual;
            actual = target.getProductSpecRules(store.getProductCategory(categoryid));

            foreach (Product p in actual._products ) {
                Console.WriteLine(p.SProductID);
            }

            Console.WriteLine("Rules");

            //foreach (VProductMatrix  vsp in actual._specrules)
            //{
            //    Console.WriteLine(vsp.AttrCatName + ":" + vsp.AttrName + ":" + vsp.AttrValueName + "(" + vsp.productcount  +")" );
            //}

        }

        /// <summary>
        ///A test for getProductSpecRules
        ///</summary>
        [TestMethod()]
        public void getProductSpecRulesTest()
        {
            SpecHelper target = new SpecHelper(); // TODO: Initialize to an appropriate value
            string categorypath = string.Empty; // TODO: Initialize to an appropriate value
            string storeid = string.Empty; // TODO: Initialize to an appropriate value
            ProductSpecRules expected = null; // TODO: Initialize to an appropriate value
            ProductSpecRules actual;
            eStore.BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);


            actual = target.getProductSpecRules(store.getProductCategory(categorypath));
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getProductSpecRules
        ///</summary>
        [TestMethod()]
        public void getProductSpecRulesTest1()
        {
            SpecHelper target = new SpecHelper(); // TODO: Initialize to an appropriate value
            string categorypath = string.Empty; // TODO: Initialize to an appropriate value
            string storeid = string.Empty; // TODO: Initialize to an appropriate value
            string keyword = string.Empty; // TODO: Initialize to an appropriate value
            ProductSpecRules expected = null; // TODO: Initialize to an appropriate value
            ProductSpecRules actual;
            eStore.BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);


            actual = target.getProductSpecRules(store.getProductCategory(categorypath));
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for filterProducts
        ///</summary>
        [TestMethod()]
        public void filterProductsTest()
        {
            SpecHelper target = new SpecHelper(); // TODO: Initialize to an appropriate value
            string categorypath = "DAQ_USB_IO_Modules"; // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            List<VProductMatrix> selectedspec = new List<VProductMatrix>(); // TODO: Initialize to an appropriate value
            string keywords = null; // TODO: Initialize to an appropriate value
            ProductSpecRules actual;
            
            eStore.BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);


            actual = target.getProductSpecRules(store.getProductCategory(categorypath), keywords, selectedspec);
            foreach (Product p in actual._products) {
                Console.WriteLine(p.DisplayPartno);
            }

        }

       

        /// <summary>
        ///A test for getBaseType
        ///</summary>
        [TestMethod()]
        public void getBaseTypeTest1()
        {
            SpecHelper target = new SpecHelper(); // TODO: Initialize to an appropriate value
            List<SpecAttributeCat> actual;
            actual = target.getBaseType();

            foreach (SpecAttributeCat sc in actual){
                Console.WriteLine(sc.AttrCatName);
                foreach (SpecAttribute stv in sc.SpecAttributes)
                    Console.WriteLine(stv.AttrName);
                }
        }

        /// <summary>
        ///A test for getSpecsbyRuleSetID
        ///</summary>
        [TestMethod()]
        public void getSpecsbyRuleSetIDTest()
        {
            SpecHelper target = new SpecHelper(); // TODO: Initialize to an appropriate value
         
           
            List<VSpec> actual;
            actual = target.getSpecsbyRulesetID("AUS", 25053,"");

            foreach (VSpec vs in actual)
                Console.WriteLine(vs.AttrCatName + ":" + vs.AttrName + ":" + vs.AttrValueName);

        }

        /// <summary>
        ///A test for searchOnlybyKeywords
        ///</summary>
        [TestMethod()]
        public void searchOnlybyKeywordsTest()
        {
            SpecHelper target = new SpecHelper(); // TODO: Initialize to an appropriate value
            string keywords = "aimb";
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            ProductSpecRules actual;
            actual = target.searchOnlybyKeywords(keywords, storeid,1000,false);

            Assert.IsTrue(actual != null);
        }
    }
}
