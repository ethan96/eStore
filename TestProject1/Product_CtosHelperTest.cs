using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for Product_CtosHelperTest and is intended
    ///to contain all Product_CtosHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Product_CtosHelperTest
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
        ///Print CTOS products
        ///</summary>
        [TestMethod()]
        public void getProductbyPartnoTest()
        {
            string sproductid = "21035"; // SYS-4U4010-4S01
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid(storeid);
            
            Product_Ctos actual;
            PartHelper ph = new PartHelper();
            

            actual = (Product_Ctos) ph.getPart(sproductid, storeid);

            Price price = actual.recalculateCTOSDefaultPrice(5);

            Console.WriteLine("CTOS System Price $" + actual.VendorSuggestedPrice);

            foreach (CTOSBOM cb in actual.CTOSBOMs) {

                if (cb.ChildComponents.Count > 0)
                {
                    Console.WriteLine(cb.CTOSComponent.ComponentName);
                    foreach (CTOSBOM ccp in cb.ChildComponents)
                    {
                        Console.WriteLine("-->" + ccp.CTOSComponent.ComponentName);
           
                        if (ccp.CTOSComponent.CTOSComponentDetails.Count > 0) {  // Print out partno
                            foreach (CTOSComponentDetail ccd in ccp.CTOSComponent.CTOSComponentDetails)
                            {

                                Console.Write("\t\t\t*" + ccd.SProductID + ":" + ccd.Qty +"*");
                                Part a = ph.getPart(ccd.SProductID, store);
                                 
                                if (a != null)
                                    Console.WriteLine("$" + a.VendorSuggestedPrice);
                            }
                            Console.WriteLine("");
                        }

                    }
                }           
            }

         
             
        }



        /// <summary>
        ///Print CTOS products
        ///</summary>
        [TestMethod()]
        public void getProductResourceTest()
        {
            string sproductid = "ACP-4320BP-30ZE"; // SYS-4U4010-4S01
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid(storeid);

            Part actual;
            PartHelper ph = new PartHelper();
            actual = ph.getPart(sproductid, storeid);

            Console.WriteLine("VendorDesc" + "===>" + actual.VendorProductDesc);
            Console.WriteLine("");
            Console.WriteLine("VendorFeatures" + "==>" +  actual.VendorFeatures);
            Console.WriteLine("");
            Console.WriteLine("VendorExtendedDesc" + "\n\r" + actual.VendorExtendedDesc);

            Console.WriteLine("");

            Console.WriteLine("ProductResource");
            foreach (ProductResource pr in actual.ProductResources) {
                Console.WriteLine( pr.ResourceName + ":" + pr.ResourceURL);
            }

            Console.WriteLine("");

            Console.WriteLine("RelatedProduct");
            foreach (RelatedProduct rproduct in actual.RelatedProducts)
            {
               Console.WriteLine(rproduct.SProductID  + "-->" + rproduct.Relationship + ":" + rproduct.RelatedSProductID  );
            }
 

        }
    }
}
