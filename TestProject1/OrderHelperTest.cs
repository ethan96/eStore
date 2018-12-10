using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for OrderHelperTest and is intended
    ///to contain all OrderHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OrderHelperTest
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
        ///A test for getUserOrders
        ///</summary>
        [TestMethod()]
        public void getUserOrdersTest()
        {
            OrderHelper target = new OrderHelper(); // TODO: Initialize to an appropriate value
            string userid = "jay.lee@advantech.com"; // TODO: Initialize to an appropriate value
            //bool includeAbandon = false; // TODO: Initialize to an appropriate value
            List<Order> actual;


            actual = target.getUserOrders(userid, "AUS",false);

            foreach (Order q in actual)
            {
                Console.WriteLine(q.OrderNo);
            }

            Console.WriteLine("All Orders");
            actual = target.getUserOrders(userid,"AUS" ,true);

            foreach (Order q in actual)
            {
                Console.WriteLine(q.OrderNo);
            }
        }

        /// <summary>
        ///A test for getOrderbyOrderno
        ///</summary>
        [TestMethod()]
        public void getOrderbyOrdernoTest()
        {
            OrderHelper target = new OrderHelper(); // TODO: Initialize to an appropriate value
            string orderno = "OUS020164"; // TODO: Initialize to an appropriate value
            //Order expected = null; // TODO: Initialize to an appropriate value
            Order actual;
            actual = target.getOrderbyOrderno(orderno);

            Console.WriteLine(actual.OrderNo);
            Console.WriteLine(actual.UserID);
            foreach (CartItem i in actual.cartX.CartItems)
            {
                Console.WriteLine(i.ItemNo + " | " + i.ProductName);
            }
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getOrders
        ///</summary>
        [TestMethod()]
        public void getOrdersTest()
        {
            OrderHelper target = new OrderHelper(); // TODO: Initialize to an appropriate value
            
            DMF dmf = new DMFHelper().getDMFbyID("ANA DMF");
            DateTime startdate = new DateTime(2010,12,13); // TODO: Initialize to an appropriate value
            DateTime enddate = new DateTime(2010,12,15); // TODO: Initialize to an appropriate value
            
            List<Order> actual;
            int[] openCloseCount = new int[]{};
            actual = target.getOrders(dmf, startdate, enddate, ref openCloseCount);

            Assert.IsNotNull(actual);

            foreach (Order o in actual) {
                Console.WriteLine(o.OrderNo +":"+ o.OrderDate + ":"+ o.Cart.BillToContact.Country);
            }
            
        }

        /// <summary>
        ///A test for getOrders
        ///</summary>
        [TestMethod()]
        public void getQuotationsTest()
        {
            QuotationHelper target = new QuotationHelper(); // TODO: Initialize to an appropriate value

            DMF dmf = new DMFHelper().getDMFbyID("ANA DMF");
            DateTime startdate = new DateTime(2010, 12, 3); // TODO: Initialize to an appropriate value
            DateTime enddate = new DateTime(2010, 12, 5); // TODO: Initialize to an appropriate value

            List<Quotation> actual;
            actual = target.getQuotations (dmf, startdate, enddate);

            Assert.IsNotNull(actual);

            foreach (Quotation  o in actual)
            {
                Console.WriteLine(o.QuotationNumber + ":" + o.QuoteDate + ":" + o.Cart.BillToContact.Country);
            }

        }

        /// <summary>
        ///A test for UpdatePayment
        ///</summary>
        [TestMethod()]
        public void UpdatePaymentTest()
        {
            OrderHelper target = new OrderHelper(); // TODO: Initialize to an appropriate value
            List<Payment> payments = new List<Payment>();

            Payment p = new Payment();

            p.PaymentID = "20101221_164819_982";

            p.cardNo = "0000000000";

            payments.Add(p);

            bool actual;
            actual = target.UpdatePayment(payments);
             
        }
    }
}
