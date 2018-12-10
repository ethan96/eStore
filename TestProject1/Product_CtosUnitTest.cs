using eStore.POCOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using eStore.BusinessModules;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for Product_CtosUnitTest and is intended
    ///to contain all Product_CtosUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Product_CtosUnitTest
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
        ///A test for specSources
        ///</summary>
        [TestMethod()]
        public void specSourcesTest()
        {
            //add BTOS items, 2U-rackmount chassis
            eStore.BusinessModules.Store ausStore = eStore.BusinessModules.StoreSolution.getInstance().getStore("AUS");
            Product product = ausStore.getProduct("1326");
            Product_Ctos ctos = null;
            if (product is Product_Ctos)
                ctos = (Product_Ctos)product;
            List<Part> actual;
            actual = ctos.specSources;

            foreach (Part part in actual)
                Console.WriteLine(part.SProductID + " - " + part.name);

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for BTOS composing including BTOS creation, price update, and warranty calculation
        ///</summary>
        [TestMethod()]
        public void BTOSComposingTest()
        {
            DateTime start = DateTime.Now;

            Console.WriteLine("\n\n****  CTOS Integration Test start @ " + start.ToString("yyyyMMdd.HHmmss.fff"));
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();

            eStore.BusinessModules.Store ausStore = eSolution.getStore("AUS");
            User tester = ausStore.getUser("testdrive@advantech.com");

            Cart cart = tester.shoppingCart;

            //add BTOS items, 2U-rackmount chassis
            Product ProductOne = ausStore.getProduct("1326");
            Product_Ctos CTOSOne = null;
            if (ProductOne is Product_Ctos)
                CTOSOne = (Product_Ctos)ProductOne;
            BTOSystem btos = CTOSOne.getDefaultBTOS();

            Assert.IsTrue(CTOSOne.isValid());

            //print CTOS system details
            CTOSOne.print(2);
            //print default BTOS
            btos.print();

            int powerSupplyComponentId = 123393;
            int singleBoardComponentId = 123390;
            int powerSupplyDefaultOptionId = 123407;
            int powerSupplyUserChooseOptionId = 141579;
            int singleBoardDefaultOptionId = 132353;
            int singleBoardUserChooseOptionId = 123443;
            int warrantyComponentId = 188795;
            int warrantyDefaultId = 188796;
            int warranty3MonthId = 188797;

            //The following is to simulate user CTOS configuration process
            //deselect power supply default item and replace with user choosed item
            CTOSBOM powerSupply = CTOSOne.findComponent(powerSupplyComponentId);
            btos.removeItem(powerSupply, powerSupply.findOption(powerSupplyDefaultOptionId));
            btos.addItem(powerSupply, powerSupply.findOption(powerSupplyUserChooseOptionId), 1);
            btos.print();

            //deselect singleboard default item and replace with user choosed item
            CTOSBOM singleBoard = CTOSOne.findComponent(singleBoardComponentId);
            btos.removeItem(singleBoard, singleBoard.findOption(singleBoardDefaultOptionId));
            btos.addItem(singleBoard, singleBoard.findOption(singleBoardUserChooseOptionId), 1);
            CTOSBOM warranty = CTOSOne.findComponent(warrantyComponentId);
            btos.removeItem(warranty, warranty.findOption(warrantyDefaultId));
            btos.addItem(warranty, warranty.findOption(warranty3MonthId), 1);                        
            btos.print();

            Price price = new Price();
            Price priceBefore = new Price();
            //after recompose BTO, application shall call updateBTOSPrice to recalculate BTOS total price
            //**** failed to do so will result with Pricing issue
            CTOSOne.updateBTOSPrice(btos, price, priceBefore);

            btos.print();
            Console.WriteLine("BTOS total is " + btos.Price);
            btos.printPartList();

            cart.addItem(CTOSOne, 1, btos);

            cart.print();


            Console.WriteLine("*** End of Integration Test : CTOS -- elape time : " + (DateTime.Now - start).TotalMilliseconds);
        }

        /// <summary>
        ///A test for BTOS composing including BTOS creation, price update, and warranty calculation
        ///</summary>
        [TestMethod()]
        public void NoneCTOBaseS_BTOSComposingTest()
        {
            DateTime start = DateTime.Now;

            Console.WriteLine("\n\n****  NoneCTOS BTOS composing Test start @ " + start.ToString("yyyyMMdd.HHmmss.fff"));
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();

            eStore.BusinessModules.Store ausStore = eSolution.getStore("AUS");
            User tester = ausStore.getUser("testdrive@advantech.com");

            Cart cart = tester.shoppingCart;

            //add BTOS items, 2U-rackmount chassis
            Product ProductOne = ausStore.getProduct("SBC-BTO");
            Product_Ctos bundleCTOS = null;
            if (ProductOne is Product_Ctos)
                bundleCTOS = (Product_Ctos)ProductOne;

            BTOSystem btos = bundleCTOS.getDefaultBTOS();

            //new product for temporary usage
            Product RealOne = ausStore.getProduct("1700060201");
            Product RealTwo = ausStore.getProduct("96RM-KVM19-8PP-AH");
            Product RealThree = ausStore.getProduct("96TC-ANFC-4P-PE-DI");

            //create bundle
            List<BundleItem> bundle = new List<BundleItem>();
            bundle.Add(new BundleItem(RealOne, 1));
            bundle.Add(new BundleItem(RealTwo, 2));
            bundle.Add(new BundleItem(RealThree, 3));

            btos.addNoneCTOSBundle(bundle, 2);
            btos.print();

            cart.addItem(bundleCTOS, 1, btos);

            cart.print();

            Console.WriteLine("*** End of Integration Test : CTOS -- elape time : " + (DateTime.Now - start).TotalMilliseconds);
        }


        /// <summary>
        ///A test for BTOS composing including BTOS creation, price update, and warranty calculation
        ///</summary>
        [TestMethod()]
        public void NoneDB_BTOSComposingTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store ausStore = eSolution.getStore("AUS");
            User jay = new User();
            jay.UserID = "eStoreTester@advantech.com";
            DateTime start = DateTime.Now;

            Console.WriteLine("Unit Test : CTOS -- " + start.ToString("yyyyMMdd.HHmmss.fff"));
            //new standard product for temporary usage
            Product dummyOne = ausStore.getDummyProduct("DummyProduct A", 100);
            Product dummyTwo = ausStore.getDummyProduct("DummyProduct B", 200);

            //    //Compose dummy CTOS product
            Product_Ctos CTOSOne = ausStore.getDummyCTOS("CTOS xxxx", 1000);

            Product Chassie = ausStore.getDummyProduct("Chassie 4U-20001", 300);
            Product MB = ausStore.getDummyProduct("Mother Board Intel 3G", 400);
            Product OptionItemOne = ausStore.getDummyProduct("96xxxxA", 50);
            Product OptionItemTwo = ausStore.getDummyProduct("17xxxxA", 20);
            Product OptionItemThree = ausStore.getDummyProduct("OPTIONSxxxxA", 30);
            Product OptionItemFour = ausStore.getDummyProduct("AGSxxxxA", 30);

            //create three dummy CTOS category component
            CTOSBOM component1 = CTOSOne.createDummyComponent("CPU Board", "Bundle of CPU and Fan", 1);
            CTOSBOM cat1_option1 = component1.addDummyOption(false, 1.0m, OptionItemOne);
            CTOSBOM cat1_option2 = component1.addDummyOption(false, 1.0m, OptionItemOne, OptionItemTwo);
            CTOSOne.addDummyComponent(component1);
            CTOSBOM component2 = CTOSOne.createDummyComponent("Base system", "Base System", 2);
            CTOSBOM cat2_option1 = component2.addDummyOption(true, 1.0m, Chassie);
            CTOSBOM cat2_option2 = component2.addDummyOption(true, 1.0m, MB);
            CTOSOne.addDummyComponent(component2);
            CTOSBOM component3 = CTOSOne.createDummyComponent("Others", "Accessories", 3);
            CTOSBOM cat3_option1 = component3.addDummyOption(true, 1.0m, OptionItemThree);
            CTOSBOM cat3_option2 = component3.addDummyOption(true, 1.0m, OptionItemThree, OptionItemFour);

            CTOSOne.addDummyComponent(component3);
            CTOSOne.print(2);

            BTOSystem btos = CTOSOne.getDefaultBTOS();
            Price listingPrice = new Price();
            Price priceBeforeAdjustment = new Price();
            CTOSOne.updateBTOSPrice(btos, listingPrice, priceBeforeAdjustment);
            Console.WriteLine("BTOS Price : " + btos.Price + ", Sum : " + btos.getSum() + ", Before adjustment : " + priceBeforeAdjustment.value);

            btos.removeItem(component3, cat3_option1);
            btos.changeItemQuantity(component3, cat3_option2, 3);
            CTOSOne.updateBTOSPrice(btos, listingPrice, priceBeforeAdjustment);
            btos.print();
            Console.WriteLine("BTOS Price after configuration change: " + btos.Price + ", Sum : " + btos.getSum());

            Console.WriteLine("*** End of Unit Test : CTOS -- elape time : " + (DateTime.Now - start).TotalMilliseconds);
            Console.WriteLine();

            Assert.AreEqual(btos.Price, btos.getSum());
        }


        /// <summary>
        ///A test for recalculateCTOSDefaultPrice
        ///</summary>
        [TestMethod()]
        public void recalculateCTOSDefaultPriceTest()
        {
            eStore.BusinessModules.Store ausStore = eStore.BusinessModules.StoreSolution.getInstance().getStore("AUS");
            Product_Ctos ctos = (Product_Ctos)ausStore.getProduct("61");

            Assert.IsNotNull(ctos);

            Console.WriteLine(ctos.getListingPrice().value);
            Price newPrice = ctos.recalculateCTOSDefaultPrice();
            Console.WriteLine(newPrice.value);
        }

        /// <summary>
        ///A test for getListingPrice
        ///</summary>
        [TestMethod()]
        public void getListingPriceTest()
        {
            eStore.BusinessModules.Store atwStore = eStore.BusinessModules.StoreSolution.getInstance().getStore("ATW");
            Product_Ctos ctos = (Product_Ctos)atwStore.getProduct("3809");

            Assert.IsNotNull(ctos);

            //SYS
            Console.WriteLine(ctos.StorePrice);
            Console.WriteLine(ctos.getListingPrice().value);

            //IPPC
            ctos = (Product_Ctos)atwStore.getProduct("2540");
            Console.WriteLine(ctos.StorePrice);
            Console.WriteLine(ctos.getListingPrice().value);

            //standard product, SBC
            Product prod = atwStore.getProduct("PCM-9388F-S0A1E");
            Console.WriteLine(prod.VendorSuggestedPrice);
            Console.WriteLine(prod.getListingPrice().value);
            //standard product IM
            prod = atwStore.getProduct("AIMB-766VG-00A1E");
            Console.WriteLine(prod.VendorSuggestedPrice);
            Console.WriteLine(prod.getListingPrice().value);
        }
    }
}
