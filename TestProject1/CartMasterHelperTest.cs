using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using eStore.POCOS.PocoX;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for CartMasterHelperTest and is intended
    ///to contain all CartMasterHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CartMasterHelperTest
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
        /// A test for save
        /// </summary>
        [TestMethod()]
        public void saveTest()
        {
            Store _store = StoreHelper.getStorebyStoreid("AUS");
          User _user = UserHelper.getUserbyID("edward.keh@advantech.com.tw");
          Product prod = (Product)PartHelper.getPart("AIMB-210F-S6A1E", _store);

          prod.ProductDesc = "Should not CHANGE!";
       
         //  Product_Ctos  prod2 = Product_CtosHelper.getProductbyPartno("1", _store.StoreID);

             Cart _cartmaster = new  Cart(); // TODO: Initialize to an appropriate value
            _cartmaster.User = _user;
            _cartmaster.Creator = _user;
            _cartmaster.Store = _store;
            _cartmaster.Status = "open";
            _cartmaster.CreatedDate = DateTime.Now;
            _cartmaster.LastUpdateDate = DateTime.Now;
             _cartmaster.CartID = Guid.NewGuid().ToString();
                         
             CartItem ci = new CartItem();
            
             ci.Product = prod;
                
             //Assert.IsTrue(ci.SProductID != null);

             //Assert.IsTrue(ci.StoreID != null);
             //Assert.IsTrue(_cartmaster.CreatedBy != null);

             //ci.Product = null;
             Assert.IsTrue(ci.SProductID != null);
             ci.ItemNo = 1;
             ci.RequiredDate = DateTime.Now;
             ci.SupplierDueDate = DateTime.Now;
             ci.DueDate = DateTime.Now;
             ci.AdjustedPrice = 100;
             ci.UnitPrice = 20;
             ci.ItemType = "FG";
              

             _cartmaster.CartItems.Add(ci);
             //CartHelper.attach(_cartmaster);
        

          // _cartmaster.addItem(prod2, 1);
 

            foreach(Contact contact in _user.Contacts) {
                _cartmaster.BilltoID = contact.ContactID;
                _cartmaster.SoldtoID = contact.ContactID;
                _cartmaster.ShiptoID = contact.ContactID;
                break;
              }

 
            foreach ( StoreCurrency cur  in _store.StoreCurrencies){
                _cartmaster.Currency = cur.CurrencyID;
                break;
            }

         //   PackingList pkls ;
         ////   pkls.CreatedDate = DateTime.Now;
         //   PackagingBox box1 = new PackagingBox();
         //   box1.DimensionUnit = "";
         //   box1.Height = 10;
         //   box1.Width = 10;
         //   box1.Length = 10;
         //   box1.Weight = 100;
         //   box1.WeightUnit = "lbs";
         //   pkls.PackagingBoxes.Add(box1);

         //   ci.PackagingBox = box1;
         //   //Add packinglist to cart.
         //   _cartmaster.PackingLists.Add(pkls);

            //int expected = 0; // TODO: Initialize to an appropriate value
             int actual;
            actual = _cartmaster.save();

            foreach (CartItem cit in _cartmaster.CartItems)
            {
                Console.WriteLine(cit.Product.ProductDesc);
                
            }

           // _cartmaster.CartItems.Remove(ci);

           // _cartmaster.save();
            Assert.AreEqual(0, actual);

            //Save BTO
            //foreach (CTOSBOM ccp in prod2.CTOSBOMs)
            //{

            //    BTOSConfig btoc = new BTOSConfig();
            // //   btoc.BTOConfigID = cim2.BTOConfigID;
            //    btoc.CategoryComponentID = ccp.ComponentID;
            //    btoc.OptionComponentDesc = ccp.CTOSComponent.ComponentDesc;
            //    btoc.CategoryComponentDesc = ccp.CTOSComponent.ComponentDesc;
            //    btoc.save();
            //}

            //if (actual != 0) {
            //    foreach (ErrorMessage em in _cartmaster.error_message) {
            //        Console.WriteLine(em.message);
            //    }

            //}

            //_cartmaster.TotalAmount = 1000;
            //actual = _cartmaster.save();

            //Assert.AreEqual(expected, actual);

        }

           [TestMethod()]
        public void saveBTOSTest()
        {
            Store _store = StoreHelper.getStorebyStoreid("AUS");
            User _user = UserHelper.getUserbyID("edward.keh@advantech.com.tw");
            Product_Ctos prod = (Product_Ctos)PartHelper.getPart("3232", _store);


            //  Product_Ctos  prod2 = Product_CtosHelper.getProductbyPartno("1", _store.StoreID);

            Cart _cartmaster = new Cart(); // TODO: Initialize to an appropriate value
            _cartmaster.User = _user;
            _cartmaster.Creator = _user;
            _cartmaster.Store = _store;
            _cartmaster.Status = "open";
            _cartmaster.CreatedDate = DateTime.Now;
            _cartmaster.LastUpdateDate = DateTime.Now;
            _cartmaster.CartID = Guid.NewGuid().ToString();

            CartItem ci = new CartItem();

            ci.Product = prod;

            //Assert.IsTrue(ci.SProductID != null);

            //Assert.IsTrue(ci.StoreID != null);
            //Assert.IsTrue(_cartmaster.CreatedBy != null);

            //ci.Product = null;
            Assert.IsTrue(ci.SProductID != null);
            ci.ItemNo = 1;
            ci.RequiredDate = DateTime.Now;
            ci.SupplierDueDate = DateTime.Now;
            ci.DueDate = DateTime.Now;
            ci.AdjustedPrice = 100;
            ci.UnitPrice = 20;
            ci.ItemType = "CTOS";


      
             foreach (Contact contact in _user.Contacts)
            {
                _cartmaster.BilltoID = contact.ContactID;
                _cartmaster.SoldtoID = contact.ContactID;
                _cartmaster.ShiptoID = contact.ContactID;
                break;
            }


            foreach (StoreCurrency cur in _store.StoreCurrencies)
            {
                _cartmaster.Currency = cur.CurrencyID;
                break;
            }

       
            //int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            int expected=0;
            BTOSystem bstem = new BTOSystem();
            bstem.BTOConfigID = Guid.NewGuid().ToString();
            

            //Save BTO
            foreach (CTOSBOM cb in prod.CTOSBOMs)
            {

                if (cb.ChildComponents.Count > 0)
                {
                    Console.WriteLine(cb.CTOSComponent.ComponentName);
                    foreach (CTOSBOM ccp in cb.ChildComponents)
                    {
                        Console.WriteLine("-->" + ccp.CTOSComponent.ComponentName);
                        BTOSConfig bcg = new BTOSConfig();
                        //bcg.CategoryComponentDesc = ccp.

                        if (ccp.CTOSComponent.CTOSComponentDetails.Count > 0)
                        {  // Print out partno
                            foreach (CTOSComponentDetail ccd in ccp.CTOSComponent.CTOSComponentDetails)
                            {
                                BTOSConfigDetail bcd = new BTOSConfigDetail();
                                Console.Write("\t\t\t*" + ccd.SProductID + ":" + ccd.Qty + "*");
                                Part a = PartHelper.getPart(ccd.SProductID, _store);
                                if (a != null)
                                    //bcd.Part = a;
                                Console.WriteLine("$" + a.VendorSuggestedPrice);

                                bcg.BTOSConfigDetails.Add(bcd);
                            }

                            Console.WriteLine("");
                        }
                        bstem.BTOSConfigs.Add(bcg);
                    }
                }

            }

            ci.BTOSystem = bstem;
            _cartmaster.TotalAmount = 1000;
            _cartmaster.CartItems.Add(ci);

            actual = _cartmaster.save();

            Assert.AreEqual(expected, actual);

              if (actual != 0)
            {
                foreach (ErrorMessage em in _cartmaster.error_message)
                {
                    Console.WriteLine(em.message);
                }

            }

        }


        [TestMethod()]
        public void CartUpdateTest()
        {
            Cart _cartmaster = CartHelper.getCartMasterbyCartID("edward.keh@advantech.com.tw");
            _cartmaster.TotalAmount = 1000;
            int actual = _cartmaster.save();
            Assert.AreEqual(0, actual);
        }

        
        [TestMethod()]
        public void deleteTest()
        {
            Cart _cartmaster = CartHelper.getCartMasterbyCartID("edward.keh@advantech.com.tw"); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = CartHelper.delete(_cartmaster);
            Assert.AreEqual(expected, actual);

        }
    }
}
