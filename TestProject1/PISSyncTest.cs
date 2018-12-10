using eStore.POCOS.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using eStore.POCOS.DAL;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1
{


    /// <summary>
    ///This is a test class for PISSyncTest and is intended
    ///to contain all PISSyncTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PISSyncTest
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
        ///A test for SyncStandardProducts
        ///</summary>
        [TestMethod()]
        public void SyncStandardProductsTest()
        {
            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value
            List<Store> _stores = new StoreHelper().getStores();
          //  foreach (Store _store in _stores)

            Store _store = new StoreHelper().getStorebyStoreid("SAP");
                target.syncStoreActiveProducts(_store);

       /// }
       }

        /// <summary>
        ///A test for SyncCTOSProducts
        ///</summary>
        [TestMethod()]
        public void SyncCTOSProductsTest()
        {
            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value 
            target.SyncCTOSProducts("SAP","SAP");

        }

        /// <summary>
        ///A test for syncAllPIS
        ///</summary>
        [TestMethod()]
        public void syncAllPISTest()
        {
            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value
            List<Store> _stores = new StoreHelper().getStores();
       
            foreach (Store _store in _stores)
            {
                if (_store.StoreID.Equals("AUS"))
                {
                    //Store _store = new StoreHelper().getStorebyStoreid("SAP");
                    target.syncAllPIS(_store, "");
                }
                else
                {
                    //Store _store = new StoreHelper().getStorebyStoreid("SAP");
                    target.syncAllPIS(_store, "");
                }
            }


        }




        /// <summary>
        ///A test for SyncAllPriceFromSAP
        ///</summary>
        [TestMethod()]
        public void SyncAllPriceFromSAPTest()
        {
            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value       
            List<Store> _stores = new StoreHelper().getStores();          
            

            try
            {
                target.SyncPrice( (new StoreHelper()).getStorebyStoreid("ARG"), "");
            }
            catch (Exception)
            {
            }

            /*
        foreach (Store _store in _stores)
        {
            if (_store.StoreID == "AUS")
            {
                target.SyncPrice(_store, "", true);
            }
            else
            {
                //Store _store = new StoreHelper().getStorebyStoreid("SAP");
                target.SyncPrice(_store, "", false);
            }
       }
             */

        }



        /// <summary>
        ///A test for SyncPeriphalMaster
        ///</summary>
        [TestMethod()]
        public void SyncPeriphalMasterTest()
        {
            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            target.SyncPeriphalMaster(store);
            
        }

        /// <summary>
        ///A test for RoundUpCTOSPrice
        ///</summary>
        [TestMethod()]
        public void RoundUpCTOSPriceTest()
        {
            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("ATW");
            String result = target.RoundUpCTOSPrice(store);
            Assert.AreEqual(result, "");
            
        }

        /// <summary>
        ///A test for SyncPrice
        ///</summary>
        [TestMethod()]
        public void SyncPriceTest()
        {
            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value
            Store _store = new StoreHelper().getStorebyStoreid("SAP"); // TODO: Initialize to an appropriate value
            string partprefix = "1254001121"; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.SyncPrice(_store, partprefix);
        }


        /// <summary>
        ///A test for SyncPrice
        ///</summary>
        [TestMethod()]
        public void SyncPISTest()
        {
            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value
            Store _store = new StoreHelper().getStorebyStoreid("AEU"); // TODO: Initialize to an appropriate value
            Part part = (new PartHelper()).getPart("PSD-A40W12", _store); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.SyncPIS(part, _store, false);
        }

        /// <summary>
        ///A test for syncProductCategoryMinimumPrice
        ///</summary>
        [TestMethod()]
        public void syncProductCategoryMinimumPriceTest()
        {
            var mstore = eStore.BusinessModules.StoreManager.getInstance().getStoreByID("AEU", new StoreHelper());
            //var category = mstore.getProductCategory(6717);

            //if (category.refreshMinimumPrice())
            //{
            //    category.save();
            //}

            PISSync target = new PISSync(); // TODO: Initialize to an appropriate value
            string storeId = "AEU"; // TODO: Initialize to an appropriate value
            //target.syncProductCategoryMinimumPrice(storeId);
        }


        [TestMethod()]
        public void syncProductCategoryStoreUrl()
        {
            try
            {
                ProductCategoryHelper _hepler = new ProductCategoryHelper();
                var allCategories = _hepler.getProductCategories("IPCR");
                var publishCategories = allCategories
                    .Where(c => c.Publish == true).ToList();
                foreach (var category in publishCategories)
                {
                    category.StoreUrl = eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(category);
                    category.save();

                    foreach (var c1 in category.ChildCategories)
                    {
                        c1.StoreUrl = eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(c1);
                        c1.save();

                        foreach (var c2 in c1.ChildCategories)
                        {
                            c2.StoreUrl = eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(c2);
                            c2.save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
