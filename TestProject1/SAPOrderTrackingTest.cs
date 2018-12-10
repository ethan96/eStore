using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using eStore.POCOS.DAL;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for SAPOrderTrackingTest and is intended
    ///to contain all SAPOrderTrackingTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SAPOrderTrackingTest
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
        ///A test for getSAPOrderTrackingInXMLstr
        ///</summary>
        [TestMethod()]
        [DeploymentItem("eStore.BusinessModules.dll")]
        public void getSAPOrderTrackingInXMLstrTest()
        {
            //PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            string orderNo = "OUS020253";
            //string email = "jmuscarella@fluidiqs.com";
            //string order = "OUS007617";
            //string email = "jk2004q4@macroautomatics.com";

            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store store = eSolution.getStore("AUS");

            OrderHelper oHelper = new OrderHelper();
            Order order = oHelper.getOrderbyOrderno(orderNo);

            SAPOrderTracking target = new SAPOrderTracking(order); // TODO: Initialize to an appropriate value
            
            eStore.POCOS.Order saporder=  target.getStoreOrder();
            //foreach (CartItem c in saporder.cartX.CartItems)
            //{
            //    Console.WriteLine("SProductID: "+c.SProductID);
            //}
            if (saporder != null)
                showBtosItems(saporder.cartX);
            else
                showBtosItems(order.cartX);
            //Console.WriteLine(target.ResponseXMLString);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        private static void showBtosItems(Cart cart)
        {
            int k = 1;
            foreach (CartItem c in cart.CartItems)
            {         
                if (c.type == Product.PRODUCTTYPE.STANDARD)
                {
                    Console.WriteLine("["+k+++"] Standard SProductID: " + c.SProductID +",    UnitPrice: "+c.UnitPrice+",    Qty: "+c.Qty+",    Subtotal: "+c.AdjustedPrice);
                }
                
                if (c.type == Product.PRODUCTTYPE.CTOS)
                {
                    Console.WriteLine("[" + k+++ "]  BTOS: " + c.SProductID + "Price: "+ c.BTOSystem.Price);
                    foreach (BTOSConfig b in c.BTOSystem.BTOSConfigsWithoutNoneItems)
                    {
                        foreach (BTOSConfigDetail d in b.BTOSConfigDetails)
                        {
                            Console.Write("       SProductID: " + d.SProductID + ",    BTOSConfigUnitPrice:" + b.netPrice+ ",    BTOSConfigQty: " + b.Qty + ",   BTOSConfigSubtotal: "+b.AdjustedPrice);
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                    foreach (KeyValuePair<Part, int> p in c.BTOSystem.parts)
                    {
                        Console.WriteLine("       SProductID: " + p.Key.SProductID +",    PartUnitPrice: "+p.Key.getListingPrice().value+ ",   PartQty: " + p.Value);
                    }
                }
                Console.WriteLine();

            }
            Console.WriteLine();


            //Show cart total price
            Console.WriteLine("Cart Items subtotal: " + cart.TotalAmount);
        }

    }
}
