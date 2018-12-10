using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for StoreSolutionTest and is intended
    ///to contain all StoreSolutionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StoreSolutionTest
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


        /*
        [TestMethod()]
        public void threadtest() {
            Thread t1 = new Thread(new ThreadStart(integration_Test_CartTest));
            Thread t2 = new Thread(new ThreadStart(integration_Test_CartTest));

            t1.Start();
            t2.Start();
        
        }
  */

        [TestMethod()]
        public void ShoppingCart_IntegrationTest()
        {
            DateTime start = DateTime.Now;

            Console.WriteLine("\n\n****  CTOS Integration Test start @ " + start.ToString("yyyyMMdd.HHmmss.fff"));

            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store ausStore = eSolution.getStore("AUS");
            Assert.IsNotNull(ausStore);

            User robotTester = ausStore.login("eStoreTester@advantech.com", "PowerUser", "localhost");
            Assert.IsNotNull(robotTester);

            Cart cart = ausStore.getUserShoppingCart(robotTester);

            //Testing adding standard product to shopping cart
            Part RealOne = ausStore.getPart("1700060201");
            Product RealTwo = ausStore.getProduct("96RM-KVM19-8PP-AH");
            Product RealThree = ausStore.getProduct("96TC-ANFC-4P-PE-DI");

            CartItem itemOne = cart.addItem(RealOne, 2);
            CartItem itemTwo = cart.addItem(RealTwo, 3);
            CartItem itemThree = cart.addItem(RealThree, 1);

            //change order quantity
            itemTwo.updateQty(1);
            cart.updateItem(itemTwo);

            //remove items
            cart.removeItem(itemOne);
            itemOne.CustomerMessage = "build instruction from robotTester";

            cart.save();

            //add BTOS items, 2U-rackmount chassis
            Product ProductOne = ausStore.getProduct("1326");
            Product_Ctos CTOSOne = null;
            if (ProductOne is Product_Ctos)
                CTOSOne = (Product_Ctos)ProductOne;
            BTOSystem btos = CTOSOne.getDefaultBTOS();

            Assert.IsTrue(CTOSOne.isValid());

            CTOSOne.print(2);
            btos.print();

            int powerSupplyComponentId = 123393;
            int singleBoardComponentId = 123390;
            int powerSupplyDefaultOptionId = 123407;
            int powerSupplyUserChooseOptionId = 141579;
            int singleBoardDefaultOptionId = 132353;
            int singleBoardUserChooseOptionId = 123443;

            //The following is to simulate user CTOS configuration process
            //deselect power supply default item and replace with user choosed item
            CTOSBOM powerSupply = CTOSOne.findComponent(powerSupplyComponentId);
            btos.removeItem(powerSupply, powerSupply.findOption(powerSupplyDefaultOptionId));
            btos.addItem(powerSupply, powerSupply.findOption(powerSupplyUserChooseOptionId), 1);
            
            //deselect singleboard default item and replace with user choosed item
            CTOSBOM singleBoard = CTOSOne.findComponent(singleBoardComponentId);
            btos.removeItem(singleBoard, singleBoard.findOption(singleBoardDefaultOptionId));
            btos.addItem(singleBoard, singleBoard.findOption(singleBoardUserChooseOptionId), 1);
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

            cart.save();

            int[] checkout = {2, 3};
            Order order = cart.checkOut(robotTester, checkout);
            cart.releaseToOrder(robotTester, order);
            Console.WriteLine("Order total amount : " + order.totalAmount);
             
            //remove all items
            cart.removeAllItem();
            cart.save();

            Console.WriteLine("*** End of Integration Test : CTOS -- elape time : " + (DateTime.Now - start).TotalMilliseconds);            
        }


        [TestMethod()]
        public void advProductsInCartTest()
        {
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            //actual = eStore.BusinessModules.StoreSolution.advProductsInCart();
            //Assert.AreEqual(expected, actual);
            //  Assert.Inconclusive("Verify the correctness of this test method.");
        }


#region advProductInCart
        [TestMethod()]
        public void advProductsInCart()
        {
            DateTime startTime = DateTime.Now;                                                      //remove after test
            Console.WriteLine("*** Test start @ " + startTime);
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store ausStore = eSolution.getStore("AUS");

            User robotTester = ausStore.getUser("eStoreTester@advantech.com");

            //products in store
            Product prod1 = ausStore.getProduct("PCA-6114P10-0B2E");//1701400972
            //Product prod2 = ausStore.getProduct("1938");
            //BTOSystem btos2 = ((Product_Ctos)prod2).getDefaultBTOS();
            //Product prod3 = ausStore.getProduct("1956");
            //BTOSystem btos3 = ((Product_Ctos)prod3).getDefaultBTOS();
            //Product prod4 = ausStore.getProduct("2435");
            //BTOSystem btos4 = ((Product_Ctos)prod4).getDefaultBTOS();
            Product prod5 = ausStore.getProduct("ADAM-3854-AE");                //unassigned box
            Product prod6 = ausStore.getProduct("1701400972");                      //unassigned box
            Product prod7 = ausStore.getProduct("ADAM-5000/485-AE");           //assigned box
            Product prod8 = ausStore.getProduct("EKI-1121L-AE");                    //unsigned box


            //DateTime prepareCTOSstart = DateTime.Now;                               //remove after test
            //Product prod9 = ausStore.getProduct("1326");                                //BTOS
            //Product prod10 = ausStore.getProduct("1095");

            //Product_Ctos _ctosProduct = null;
            //if (prod9 is Product_Ctos)
            //    _ctosProduct = (Product_Ctos)prod9;
            //BTOSystem _btos= _ctosProduct.getDefaultBTOS();
            //if (_ctosProduct.isValid() == false)
            //    return -1;
            ////_ctosProduct.print(2);
            //CTOSBOM kb = _ctosProduct.findComponent(123432);
            //_btos.removeItem(kb, kb.findOption(123434));
            //_btos.addItem(kb, kb.findOption(123435), 1);
            //CTOSBOM lcd = _ctosProduct.findComponent(123433);
            //_btos.removeItem(lcd, lcd.findOption(123439));
            //_btos.addItem(lcd, lcd.findOption(123438), 1);
            ////_btos.printPartList();
            ////_btos.print();
            //Product_Ctos _ctosProduct2 = null;
            //if (prod10 is Product_Ctos)
            //    _ctosProduct2 = (Product_Ctos)prod10;
            //BTOSystem _btos2 = _ctosProduct2.getDefaultBTOS();

            //DateTime prepareCTOSend = DateTime.Now;              //remove after test 
            //Console.WriteLine("*** Prepare a BTOS : -- elape time : " + (prepareCTOSend - prepareCTOSstart).TotalSeconds);

            Cart cart = ausStore.getUserShoppingCart(robotTester);

            //cart.addItem(prod1, 9);
            //cart.addItem(prod2, 2, btos2);
            //cart.addItem(prod3, 1, btos3);
            //cart.addItem(prod4, 2, btos4);
            //cart.addItem(prod5, 32);
            //cart.addItem(prod6, 17);
            //cart.addItem(prod7, 2);
            //cart.addItem(prod8, 3);
            //cart.addItem(prod9, 1, _btos);
            //cart.addItem(prod10, 1, _btos2);

            /* *
             * Shipping carrier solutions
             * */
            //get available shipping methods from thru shipping manager

            //ship to Grant Hyatt Hotel in Brazil
            Contact _shipto = new Contact();
            _shipto.City = "Cincinnati";
            _shipto.State = "OH";
            _shipto.ZipCode = "45240";
            _shipto.Country = "US";

            //ship to Grant Hyatt Hotel in Argentina
            //Contact _shipto = new Contact();
            //_shipto.City = "Roosendaal";
            //_shipto.State = "SB";
            //_shipto.ZipCode = "4704 ";
            //_shipto.Country = "NL";

            Contact _billto = new Contact();
            _billto.City = "Milpitas";
            _billto.State = "CA";
            _billto.ZipCode = "95035";
            _billto.Country = "US";
            cart.setShipTo(_shipto);
            cart.setBillTo(_billto);

            showOrdetailInfo(cart, ausStore);

            //remove all items
            cart.removeAllItem();
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("Remove all cart itesm");
            Console.WriteLine("Cart total charge is " + cart.TotalAmount);

            Console.WriteLine();
        }


        private static void showCartInfo(Cart cart)
        {
            int no = 0;
            Console.WriteLine("No    |    Product    |    UnitPrice    |    Qty    |    ListPrice(Subtotal)");
            Console.WriteLine("========================================");
            foreach (CartItem c in cart.CartItems)
            {
                no++;
                Product _prod = new Product();
                _prod = (Product)c.Part;
                Console.WriteLine(no + ",    " + _prod.name + "    " + c.UnitPrice + "  " + c.Qty + "   " + (decimal)c.UnitPrice * c.Qty);
                _prod = null;
            }
            Console.WriteLine("========================================");
            Console.WriteLine("Cart total charge is " + cart.TotalAmount);
            Console.WriteLine(" ");
        }

        private static void showShippingMethodInfo(List<ShippingMethod> avShippingMethods)
        {
            Console.WriteLine("***************** Shipping carriers ******************");
            Console.WriteLine("Ship to: " + avShippingMethods[2].PackingList.ShipTo.ZipCode + ", " + avShippingMethods[2].PackingList.ShipTo.Country);
            Console.WriteLine("Ship from: " + avShippingMethods[2].PackingList.ShipFrom.ZipCode + ", " + avShippingMethods[2].PackingList.ShipFrom.Country);
            Console.WriteLine("Bill to: " + avShippingMethods[2].PackingList.BillTo.ZipCode + ", " + avShippingMethods[2].PackingList.BillTo.Country);
            Console.WriteLine("Sold to: " + avShippingMethods[2].PackingList.SoldTo.ZipCode + ", " + avShippingMethods[2].PackingList.SoldTo.Country);
            Console.WriteLine("");
            foreach (ShippingMethod sml in avShippingMethods)
            {
                Console.WriteLine("[" + sml.ShippingCarrier + "] " + sml.ShippingMethodDescription + "(" + sml.ServiceCode + ")");
                Console.WriteLine("    Insured charge: " + sml.InsuredCharge + sml.UnitOfCurrency);
                Console.WriteLine("    Published rate: " + sml.PublishRate + sml.UnitOfCurrency);
                Console.WriteLine("    Negotiated rate: " + sml.NegotiatedRate + sml.UnitOfCurrency);
                Console.WriteLine("    Published shipping cost: " + sml.ShippingCostWithPublishedRate + sml.UnitOfCurrency);
                Console.WriteLine("    Negotiated shipping cost: " + sml.ShippingCostWithNegotiatedRate + sml.UnitOfCurrency);
            }
            Console.WriteLine();
        }

        private static void showPackingList(ICollection<PackagingBox> _packagingBox)
        {
            Console.WriteLine(" ********************* Packing List ***********************");
            int i = 1;
            foreach (PackagingBox pb in _packagingBox)
            {
                Console.WriteLine("【Package No." + i + "】");
                Console.WriteLine("========================================================");
                Console.WriteLine("    Packaging Box ID: " + pb.BoxId);
                Console.WriteLine("    Packaging Insured Value: " + pb.InsuredValue);
                //Console.WriteLine("    Item Qty in Packaging Box: " + pb.QtyOfItemInBox);

                //List<PackingBoxDetail> _packingBoxDetail = pb.PackingBoxDetails.ToList();
                ICollection<PackingBoxDetail> _packingBoxDetail = pb.PackingBoxDetails;
                Console.Write("    Product item in Packaging Box: ");
                foreach (PackingBoxDetail c in _packingBoxDetail)
                {
                    Console.WriteLine(c.SProductID + "(Qty: " + c.Qty + " pcs)");
                }
                Console.WriteLine("    Packaging box remain capacity: " + pb.RemainCapacity);
                Console.WriteLine("    Packaging box allow to merge: " + pb.AllowToMerge);
                Console.WriteLine("    Packaging box length: " + pb.Length + pb.DimensionUnit);
                Console.WriteLine("    Packaging box width: " + pb.Width + pb.DimensionUnit);
                Console.WriteLine("    Packaging box height: " + pb.Height + pb.DimensionUnit);
                Console.WriteLine("    Packaging box weight: " + pb.Weight + pb.WeightUnit);
                Console.WriteLine("    Packaging box dimensional weight: " + pb.DimensionalWeight + pb.WeightUnit);
                Console.WriteLine("    Packaging box dimensional base: " + pb.DimensionWeightBase);
                Console.WriteLine("    Packaging box shipping cost base on: " + pb.ShippingCostBase);

                Console.WriteLine("............................  BTOS Parts  ............................");

                int m = 1;
                foreach (PackingBoxDetail c in pb.PackingBoxDetails)
                {
                    if (c.BtosPart != null)
                    {
                        foreach (KeyValuePair<Part, int> p in c.BtosPart)
                        {
                            Console.WriteLine("     " + m++ + ". Part: " + p.Key.SProductID + ",  Qty: " + p.Value + ",  Total cost: " + p.Value * p.Key.Cost.Value + ",  Total weight: " + p.Value * (MeasureUnit.convertKG2LB(p.Key.ShipWeightKG.Value)) + "LBS");
                        }
                    }
                }

                Console.WriteLine("========================================================");
                Console.WriteLine();
                i++;
            }
            //test
            //float f = 3.123f;
            //float g = 3.888f;
            //Console.WriteLine("f = 3.123 ->(int)" + (int)f);
            //Console.WriteLine("g = 3.888->(int)" + (int)g);
        }

        private static void showOrdetailInfo(Cart cart, eStore.BusinessModules.Store ausStore)
        {
            Console.WriteLine();

            Console.WriteLine(" ********************  Order Detail *********************** ");
            showCartInfo(cart);
            showBtosItems(cart);

            //We face issue at running multi-thread process in Unit Test....this line will be enable
            //when the threading issue is  resolved.  ************************
            //List<ShippingMethod> avShippingMethods = ausStore.getAvailableShippingMethods(cart);

            List<ShippingMethod> avShippingMethods = new List<ShippingMethod>();

            //print out, shipping methods
            //showShippingMethodInfo(avShippingMethods);
            //ICollection<PackagingBox> _packagingBox = avShippingMethods[2].PackingList.PackagingBoxes;
            //showPackingList(_packagingBox);

        }

        private static void showBtosItems(Cart cart)
        {
            foreach (CartItem c in cart.CartItems)
            {
                int k = 1;
                if (c.type == Product.PRODUCTTYPE.CTOS)
                {
                    Console.WriteLine("Btos: " + c.SProductID);

                    foreach (KeyValuePair<Part, int> p in c.BTOSystem.parts)
                    {
                        Price _price = new Price();
                        _price = p.Key.getNetPrice();
                        ProductBox _pb = ProductBoxHelper.getProductBoxByPartno(p.Key.SProductID, cart.storeX);
                        string _additionalBox = "";
                        if (_pb != null)
                        {
                            if (_pb.AdditionalBox == true)
                                _additionalBox = " ★ ";
                        }
                        else
                            _additionalBox = " ☆ ";
                        Console.WriteLine(_additionalBox + "Part " + k++ + ": " + p.Key.SProductID + ".................Price: " + _price.value + _price.currency + " / Cost: " + p.Key.Cost);
                    }
                }

            }
            Console.WriteLine(" ★ means the item that needs an addtional box.");
            Console.WriteLine();
        }


        private void simple()
        {
            String _password = "m";
            MD5 md5 = MD5.Create();
            byte[] input = System.Text.Encoding.Unicode.GetBytes(_password);
            byte[] hashBytes = md5.ComputeHash(input);
            StringBuilder sb0 = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                sb0.Append(input[i].ToString("x2"));
            }

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            Console.WriteLine("Plain: " + sb0);
            Console.WriteLine("MD5: " + sb);
        }

        private void timeFormat()
        {
        }

#endregion

    }
}
