using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS.DAL;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for CTCBPosSolutionTest and is intended
    ///to contain all CTCBPosSolutionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CTCBPosSolutionTest
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
        ///A test for authTest
        ///</summary>
        [TestMethod()]
        [DeploymentItem("eStore.BusinessModules.dll")]
        public void authTestTest()
        {
            string[] agrs = null; // TODO: Initialize to an appropriate value
            int actual;
            actual = CTCBPosSolution.authTest(agrs);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        [DeploymentItem("eStore.BusinessModules.dll")]
        public void makePaymentTest()
        {
            string orderNo = "OUS020179";
            OrderHelper orderhelper = new OrderHelper();
            eStore.POCOS.Order order = orderhelper.getOrderbyOrderno(orderNo);

            if (orderNo == null)
            {
                order = new Order();
                order.TotalAmount = 1000m;
            }

            CTCBPosSolution ctcbPosSolution = new CTCBPosSolution();
            Payment payment = new Payment();
            payment.Amount = order.totalAmountX;
            payment.Order = order;

            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store store = eSolution.getStore("ATW");


            ctcbPosSolution.makePayment(order, payment, true);

            Console.WriteLine("-------- Order information --------");
            Console.WriteLine("Order No: " + order.OrderNo);
            Console.WriteLine("Order created date: " + order.OrderDate);
            Console.WriteLine("Order Amount: " + order.totalAmountX);
            Console.WriteLine();
            Console.WriteLine("-------- Payment information --------");
            Console.WriteLine("Payment order no: " + payment.OrderNo);
            Console.WriteLine("Card holder: " + payment.CardHolderName);
            Console.WriteLine("Card No: " + payment.cardNo);
            Console.WriteLine("Card type: " + payment.CardType);
            Console.WriteLine("Card CVV2: " + payment.CCAuthCode);
            Console.WriteLine("Card expire date: "+payment.cardExpiredYear+"/"+payment.cardExpiredMonth);
            Console.WriteLine("Payment date: " + payment.CreatedDate);
            Console.WriteLine("Payment total amount: " + payment.Amount);
            Console.WriteLine("Payment status: " + payment.statusX.ToString());
            Console.WriteLine();
            Console.WriteLine("-------- Error information --------");
            Console.WriteLine("ErrorCode: " + payment.errorCode);
        }
    }
}
