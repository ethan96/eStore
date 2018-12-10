using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eStore.POCOS.DAL;
using eStore.POCOS;

namespace TestProject1
{
    /// <summary>
    /// Summary description for ProductResouce
    /// </summary>
    [TestClass]
    public class ProductResouce
    {
        public ProductResouce()
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
            //
            // TODO: Add test logic here
            //
        }



        [TestMethod]
        public void getCTOSProductTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store _store = eSolution.getStore("AUS");
            string productId = "3002";
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            Part part = new PartHelper().getPart(productId, store, true);
            Product_Ctos pc = null;
            if (part is Product_Ctos)
                pc = part as Product_Ctos;
            if (pc != null)
            {
                foreach (CTOSBOM component in pc.components)
                {
                    foreach (CTOSBOM option in component.options)
                    {
                        if (option.LimitedResources != null && option.LimitedResources.Count > 0)
                            Console.WriteLine("CBOM:{0} -->{1} Limiteds,  resource='{2}'", option.name, option.LimitedResources.Count, _store.getBOMlimitedResources(option));
                        if (option != null && option.componentDetails != null && option.componentDetails.Count > 0)
                        {
                            foreach (CTOSBOMComponentDetail cd in option.componentDetails)
                            {
                                if (cd != null && cd.part != null && cd.part.ProductLimitedResources != null && cd.part.ProductLimitedResources.Count > 0)
                                {
                                    foreach (var c in cd.part.ProductLimitedResources)
                                    {
                                        Console.WriteLine("partNo:{0} --> LimitedName:{1} --> AvaiableQty:{2} --> ConsumingQty:{3}", cd.part.SProductID, c.LimitedResource.Name, c.AvaiableQty, c.ConsumingQty);
                                    }
                                }
                            }
                        }
                    }
                }


                var plrLS = pc.ProductLimitedResources.ToList();


            }

        }


    }
}
