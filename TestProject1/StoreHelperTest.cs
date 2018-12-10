using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for StoreHelperTest and is intended
    ///to contain all StoreHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StoreHelperTest
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
            Store _store = new Store(); // TODO: Initialize to an appropriate value
            StoreHelper shelper = new StoreHelper();
        
  
            _store.Status = "Active";
            _store.DMFID = "ANA DMF";

            DMF dmf = new DMF();
            dmf.ContactEmail = "test@advantech.com -- will you update???";
            dmf.DMFID = "edward dmf";

           // Hashtable ht =  _store.validate();

            //foreach (string k in ht.Keys) {
            //    Console.WriteLine(k + ":" + ht[k]);
            //}

           // _store.DMF = dmf;

            Console.WriteLine(_store.DMFID);
     
            shelper.save(_store);

            Store newstore = shelper.getStorebyHostname(_store.StoreURL);
            Assert.AreEqual(_store.StoreURL, newstore.StoreURL);

           // StoreHelper.delete(newstore);
        //    Store newstore2 = StoreHelper.getStorebyStoreid(_store.StoreID);

       //     Assert.AreEqual(newstore2, null);
       //     StoreHelper.delete(newstore);
       
        }

        /// <summary>
        ///A test for getStorebyHostname
        ///</summary>
        [TestMethod()]
        public void getStorebyHostnameTest()
        {
            string hostname = "buy.advantech.net.au"; // TODO: Initialize to an appropriate value
            StoreHelper shelper = new StoreHelper();
            Store actual;

            actual = shelper.getStorebyHostname(hostname);
           
            foreach (StoreCarier  sc in actual.StoreCariers)
            {
                Console.WriteLine(sc.CarierName);
                //sc.LoginAccount = "edward" + sc.CarierName + DateTime.Now;
                //sc.save();
                 
            }

            ShippingCarier newsc = new ShippingCarier();
            newsc.CarierName = "New Carier";
            newsc.CarierType = "TT";
            newsc.ShipperAccount = "ttt";
            newsc.WebServiceURL = "http://wwww";
            newsc.Measurement = "EN";
            //actual.ShippingCariers.Add(newsc);

            actual.StoreURL = DateTime.Now.ToString();
            shelper.save(actual);

            Assert.IsTrue(actual != null);
           
            
        }

        /// <summary>
        ///A test for getStorebyHostname
        ///</summary>
        [TestMethod()]
        public void getStorebystoreidTest()
        {
            string hostname = "AUS"; // TODO: Initialize to an appropriate value
            Store actual;
           
            StoreHelper shelper = new StoreHelper();

            actual = shelper.getStorebyStoreid(hostname);
           
           
            //Console.WriteLine(actual.DMF.ContactEmail);
            foreach (StoreCarier  sc in actual.StoreCariers)
            {
                sc.ShippingCarier.LoginAccount = "edward";
             //   actual.MetaDesc = sc.CarierName + "edward";
           //     sc.save();
                Console.WriteLine(sc.CarierName);
            }

            foreach (StoreFreightRate  sf in actual.StoreFreightRates )
            {

                Console.WriteLine(sf.Freight);
            }

            Console.WriteLine("------DMF ------------------------------------------------");

            

            foreach (StorePayment sp in actual.StorePayments )
            {

                Console.WriteLine(sp.PaymentMethod + ":" + sp.PaymentType);
            }


            foreach (string er in actual.ErrorCodes.Keys  )
            {

                Console.WriteLine(er + ":" + actual.ErrorCodes[er]);
            }

            foreach (SalesPerson  sp in actual.SalesPersons)
            {

                Console.WriteLine(sp.Name   + ":" +  sp.EmployeeID );
            }      
        }


        /// <summary>
        ///A test for getStorebyHostname
        ///</summary>
        [TestMethod()]
        public void getStorebyHostNameTest()
        {
            string hostname = "buy.advantech.com"; // TODO: Initialize to an appropriate value
            Store actual;

            StoreHelper shelper = new StoreHelper();

            actual = shelper.getStorebyHostname(hostname);
            

            //Console.WriteLine(actual.DMF.ContactEmail);
            foreach (StoreCarier  sc in actual.StoreCariers )
            {
                sc.ShippingCarier.LoginAccount = "edward";
                //   actual.MetaDesc = sc.CarierName + "edward";
                //     sc.save();
                Console.WriteLine(sc.CarierName);
            }

            foreach (StoreFreightRate sf in actual.StoreFreightRates)
            {

                Console.WriteLine(sf.Freight);
            }
 

            foreach (StorePayment sp in actual.StorePayments)
            {

                Console.WriteLine(sp.PaymentMethod + ":" + sp.PaymentType);
            }

            Console.WriteLine( actual.Localization[Store.TranslationKey.eStore_Title.ToString()]);
            
        }


        /// <summary>
        ///A test for update
        ///</summary>
     

        /// <summary>
        ///A test for delete
        ///</summary>
        [TestMethod()]
        public void deleteTest()
        {
            Store _store = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = new StoreHelper().delete(_store);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for isCompanyIDExist
        ///</summary>
        [TestMethod()]
        public void isCompanyIDExistTest()
        {
            StoreHelper target = new StoreHelper(); // TODO: Initialize to an appropriate value
            string _companyid = "UONOPA001"; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.isCompanyIDExist(_companyid);
            Assert.AreEqual(expected, actual);


            _companyid = "ImpossibleExist";
            expected = false;
            actual = target.isCompanyIDExist(_companyid);
            Assert.AreEqual(expected, actual);

            _companyid = null;
            actual = target.isCompanyIDExist(_companyid);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getStorebyStoreid
        ///</summary>
        [TestMethod()]
        public void getStorebyStoreidTest()
        {
            StoreHelper target = new StoreHelper(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            Store actual;
            actual = target.getStorebyStoreid(storeid);

            //foreach (OrderSyncParameter x in (from s in actual.OrderSyncParameters orderby s.ParameterSeq select s))
            //    Console.WriteLine(x.Parameter + ":" + x.ShowText);

            Console.WriteLine(actual.Settings["SAPDefaultPlant"]); 
        }



        /// <summary>
        ///A test for getSupportedShipping
        ///</summary>
        [TestMethod()]
        public void getSupportedShippingTest()
        {
            StoreHelper target = new StoreHelper(); // TODO: Initialize to an appropriate value
            string country = "USA"; // TODO: Initialize to an appropriate value
            List<RateServiceName> actual;
            actual = target.getSupportedShipping(country,"AUS");

            foreach (RateServiceName rs in actual)
                Console.WriteLine(rs.DefaultServiceName + ":" + rs.AdvantechCode);

        }
    }
}
