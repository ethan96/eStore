using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for CartHelperTest and is intended
    ///to contain all CartHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CartHelperTest
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
            CartHelper target = new CartHelper(); // TODO: Initialize to an appropriate value
            Cart _cartmaster = target.getCartMastersbyUserID("edward.keh@advantech.com.tw", "AUS");

            if (_cartmaster.BillToContact == null)
            {

                CartContact cartContact = new CartContact();
                cartContact.Address1 = "Test";
                cartContact.Address2 = "Test";
                cartContact.AddressID = "Test";
                cartContact.AttCompanyName = "Test";
                //cartContact.Attention = "Test";
                cartContact.FirstName = "Test";
                cartContact.City = "Test";
                cartContact.ContactType = "Test";
                cartContact.Country = "Test";
                cartContact.County = "Test";
                cartContact.CreatedBy = "edward.keh@advantech.com.tw";
                cartContact.CreatedDate = DateTime.Now;
                cartContact.FaxNo = "234324";
                cartContact.LastUpdateTime = DateTime.Now;
                cartContact.Mobile = "234234";
                cartContact.State = "CA";
                cartContact.TelExt = "3243";
                cartContact.TelNo = "23423";
                cartContact.UpdatedBy = "edward.keh@advantech.com.tw";
                cartContact.UserID = "edward.keh@advantech.com.tw";
                cartContact.ZipCode = "94538";

                new CartContactHelper().save(cartContact);
               // _cartmaster.BillToContact = cartContact;
                 _cartmaster.BilltoID = cartContact.ContactID;
            }
            else {
                _cartmaster.BillToContact.Address1 = "Test";
                _cartmaster.BillToContact.Address2 = "Test";
                _cartmaster.BillToContact.AddressID = "Test";
                _cartmaster.BillToContact.AttCompanyName = "Test";
                //_cartmaster.BillToContact.Attention = "Test";
                _cartmaster.BillToContact.FirstName = "Test";
                _cartmaster.BillToContact.City = "Test";
                _cartmaster.BillToContact.ContactType = "Test";
                _cartmaster.BillToContact.Country = "Test";
                _cartmaster.BillToContact.County = "Test";
                _cartmaster.BillToContact.CreatedBy = "edward.keh@advantech.com.tw";
                _cartmaster.BillToContact.CreatedDate = DateTime.Now;
                _cartmaster.BillToContact.FaxNo = "234324";
                _cartmaster.BillToContact.LastUpdateTime = DateTime.Now;
                _cartmaster.BillToContact.Mobile = "234234";
                _cartmaster.BillToContact.State = "CA";
                _cartmaster.BillToContact.TelExt = "3243";
                _cartmaster.BillToContact.TelNo = "23423";
                _cartmaster.BillToContact.UpdatedBy = "edward.keh@advantech.com.tw";
                _cartmaster.BillToContact.UserID = "edward.keh@advantech.com.tw";
                _cartmaster.BillToContact.ZipCode = "94538";
                        
            }                      
             

            int actual;
            actual = target.save(_cartmaster);
           
        }

        /// <summary>
        ///A test for getAbandonedCarts
        ///</summary>
        [TestMethod()]
        public void getAbandonedCartsTest()
        {
            CartHelper target = new CartHelper(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            DateTime startdate = new DateTime(2010,12,13); // TODO: Initialize to an appropriate value
            DateTime enddate = new DateTime(2010,12,13); // TODO: Initialize to an appropriate value
            List<Cart> actual;
            actual = target.getInactiveCarts(store, startdate, enddate);

            foreach (Cart c in actual) {
                Console.WriteLine(c.UserID + "::" + c.CreatedDate + ":" + c.StoreID);
            }

        }
    }
}
