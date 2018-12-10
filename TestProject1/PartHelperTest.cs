using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for PartHelperTest and is intended
    ///to contain all PartHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PartHelperTest
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
        ///A test for getSimpleProducts
        ///</summary>
        [TestMethod()]
        public void getSimpleProductsTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            List<Product> actual;
            actual = target.getActiveStandardProducts(storeid);

            foreach (Product p in actual)
            { 
              
                Console.WriteLine(p.SProductID + ":" + p.Status);
            }


        }

        /// <summary>
        ///A test for getCTOSProducts
        ///</summary>
        [TestMethod()]
        public void getCTOSProductsTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            List<Product_Ctos> actual;
            actual = target.getCTOSProducts(storeid);

            foreach (Product_Ctos p in actual)
            {
                 
                Console.WriteLine(p.SProductID + ":" + p.Status);
            }
        }

        /// <summary>
        ///A test for setATPs
        ///</summary>
        [TestMethod()]
        public void setATPsTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            Dictionary<Part, int> partnos = new Dictionary<Part, int>();

            Part p1 = target.getPart("AIMB-780QG2-00A1E",store);
            Part p2 = target.getPart("96D3-1G1066NN-AP",store);
            Part p3 = target.getPart("96D3-1G1333NN-AP", store);
            Part p4 = target.getPart("96D3-2G1066NN-AP", store);
            Part p5 = target.getPart("96D3-2G1333NN-AP", store);
            Part p6 = target.getPart("96MPI3-3.06-4M11B", store);
            Part p7 = target.getPart("96MPI7-2.8-8M11T", store);

            partnos.Add(p1, 2);
            partnos.Add(p2, 1);
            partnos.Add(p3, 1);
            partnos.Add(p4, 1);
            partnos.Add(p5, 1);
            partnos.Add(p6, 1);
            partnos.Add(p7, 1);

            target.setATPs(store, partnos);

            foreach(Part par in partnos.Keys) {
                Console.WriteLine(par.atp.availableDate + ":" + par.atp.availableQty);
            }
            
        }

        /// <summary>
        ///A test for getSimpleProducts
        ///</summary>
        [TestMethod()]
        public void getSimpleProductsTest1()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string storeid = string.Empty; // TODO: Initialize to an appropriate value
            List<Product> expected = null; // TODO: Initialize to an appropriate value
            List<Product> actual;
            actual = target.getActiveStandardProducts(storeid);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getOrderbyPNHint
        ///</summary>
        [TestMethod()]
        public void getOrderbyPNHintTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS"); // TODO: Initialize to an appropriate value
            Dictionary<string,string> actual;
            actual = target.getOrderbyPNHint("adam",store,false);

            foreach (string s in actual.Keys)
                Console.WriteLine(s);

            
        }

        /// <summary>
        ///A test for searchPartsOrderbyPN
        ///</summary>
        [TestMethod()]
        public void searchPartsOrderbyPNTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string keyword = "pci-1620A"; // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS"); // TODO: Initialize to an appropriate value
            List<Part> actual;
            actual = target.searchPartsOrderbyPN(keyword, store);

            eStore3Entities6 context = new eStore3Entities6();
            foreach (Part p in actual) {

                Console.WriteLine(p.SProductID + ":" + p.VendorSuggestedPrice + ":" + p.StockStatus );
               
                
            }

        }

        /// <summary>
        ///A test for getPart
        ///</summary>
        [TestMethod()]
        public void getPartTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string sproductid = "PCM-9576FV-02A1E"; // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            Part actual;
            actual = target.getPart(sproductid, storeid);

            foreach (PeripheralCompatible pc in actual.PeripheralCompatibles) {
                Console.WriteLine(pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory.name + "-->" 
                    + pc.PeripheralProduct.PeripheralsubCatagory.name + "-->" + pc.PeripheralProduct.SProductID);
            
            }

            
        }

        /// <summary>
        ///A test for getPart
        ///</summary>
        [TestMethod()]
        public void getPartTestReplace()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string sproductid = "2221"; // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            Product actual;
            actual = (Product) target.getPart(sproductid, storeid);

            foreach (ReplaceProduct pc in actual.ReplaceProducts)
            {
                Console.WriteLine(actual.DisplayPartno + ":" + actual.SProductID + " replaced by " + pc.ReplacedProduct.DisplayPartno + ":" + pc.ReplacedProduct.SProductID);

            }


        }


        /// <summary>
        ///A test for getProductsHint
        ///</summary>
        [TestMethod()]
        public void getProductsHintTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
             
            Dictionary<string,string>  actual;
            actual = target.getProductsHint(storeid);

            foreach (string sproductid in actual.Keys ) {
                Console.WriteLine(sproductid + ":" + actual[sproductid]);
            }
        }

        /// <summary>
        ///A test for getSAPPartsNonEstoreHint
        ///</summary>
        [TestMethod()]
        public void getSAPPartsNonEstoreHintTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            Dictionary<string, string> actual;
            actual = target.getSAPPartsNonEstoreHint(store);

            foreach (string sproductid in actual.Keys)
            {
                Console.WriteLine(sproductid + ":" + actual[sproductid]);
            }

        }

        /// <summary>
        ///A test for getCTOSProductsHint
        ///</summary>
        [TestMethod()]
        public void getCTOSProductsHintTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            Dictionary<string, string> actual;
            actual = target.getCTOSProductsHint (store.StoreID );

            foreach (string sproductid in actual.Keys)
            {
                Console.WriteLine(sproductid + ":" + actual[sproductid]);
            }
        }

        /// <summary>
        ///A test for AddtoStore
        ///</summary>
        [TestMethod()]
        public void AddtoStoreTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string partno = "APAX-5018-AE"; // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            bool issave = true; // TODO: Initialize to an appropriate value
            Product actual;
            actual = target.AddtoStore(partno, store, issave);
            
        }

        /// <summary>
        ///A test for iseStoreproduct
        ///</summary>
        [TestMethod()]
        public void iseStoreproductTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string partno = "SYS-4W512-4U01";
       
            string storeid = "AUS";
            List<String> products = null; // TODO: Initialize to an appropriate value
            List<String> ctoss = null; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.iseStoreproduct(partno, storeid, out products, out ctoss);
    
           
            Assert.AreEqual(expected, actual);
            
        }



        /// <summary>
        ///A test for searchPartsOrderbyPN
        ///</summary>
        [TestMethod()]
        public void searchPartsOrderbyPNTest1()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string keyword = string.Empty; // TODO: Initialize to an appropriate value
            Store store = null; // TODO: Initialize to an appropriate value
            bool includeInvalidStatus = false; // TODO: Initialize to an appropriate value
            List<Part> expected = null; // TODO: Initialize to an appropriate value
            List<Part> actual;
            actual = target.searchPartsOrderbyPN(keyword, store, includeInvalidStatus);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getModelname
        ///</summary>
        [TestMethod()]
        public void getModelnameTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string partno = "SYS-4W5120"; // TODO: Initialize to an appropriate value
            string expected = "SYS-4W5120"; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.getModelname(partno);
            Assert.AreEqual(expected, actual);
      
        }

        /// <summary>
        ///A test for exportCBOM
        ///</summary>
        [TestMethod()]
        public void exportCBOMTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            bool includeall = false; // TODO: Initialize to an appropriate value
            
            List<ExportCBOM_Result> actual;
            actual = target.exportCBOM(storeid, includeall);
            Console.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for getOrderbyPNHint
        ///</summary>
        [TestMethod()]
        public void getOrderbyPNHintTest1()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string keyword = "AG";
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            bool isom = false; // TODO: Initialize to an appropriate value
 
            Dictionary<string, string> actual;
            actual = target.getOrderbyPNHint(keyword, store, isom);
           
            foreach(string p in actual.Keys)
                Console.WriteLine(p);
        }

        /// <summary>
        ///Unit test for getSolutionProduct
        ///</summary>
        [TestMethod()]
        public void getSolutionProductTest1()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");

            Product sample = target.getSolutionProduct("DSS-642-TE", store);
            //sample = target.getSolutionProduct("DSS-642-TE", store);
            //sample = target.getSolutionProduct("ZTWVG0000000000063", store);

        }

        /// <summary>
        ///Unit test for isPTradePart
        ///</summary>
        [TestMethod()]
        public void isPTradePartTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");

            Part sample = target.getPart("96VG-512M-PE-EV4", store);
            Console.WriteLine(sample.SProductID + " is a PTrade part ? " + ((sample.isPTradePart()) ? "Yes" : "No"));
            sample = target.getPart("ADAM-3013-AE", store);
            Console.WriteLine(sample.SProductID + " is a PTrade part ? " + ((sample.isPTradePart()) ? "Yes" : "No"));
            sample = target.getPart("9680002100", store);
            Console.WriteLine(sample.SProductID + " is a PTrade part ? " + ((sample.isPTradePart()) ? "Yes" : "No"));
        }


        [TestMethod]
        public void isOrderProductTest()
        {
            string PartNumber = "BAS-3018BC-AE";
            string storeid = "AUS";
            Part part = new PartHelper().getPart(PartNumber, storeid, false);
            if (part is Product && part.isOrderable())
                Console.WriteLine("True");
            else
                Console.WriteLine("False");
        }



        /// <summary>
        ///A test for getPartsDynamically
        ///</summary>
        [TestMethod()]
        public void getPartsDynamicallyTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            Dictionary<string, object> conditions =  new Dictionary<string,object>(); // TODO: Initialize to an appropriate value
            conditions.Add("StoreID", "AUS");
            conditions.Add("SProductID", "PCA-6108P6-0B4E");
            List<Part> expected = null; // TODO: Initialize to an appropriate value
            List<Part> actual;
            //actual = target.getPartsDynamically(conditions);
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for geteStoreSystemsByOptionModel
        ///</summary>
        [TestMethod()]
        public void geteStoreSystemsByOptionModelTest()
        {
            PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            string modelno = "AIMB-780"; // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            Dictionary<string, List<string>> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<string, List<string>> actual;
            actual = target.geteStoreSystemsByOptionModel(modelno, storeid);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
