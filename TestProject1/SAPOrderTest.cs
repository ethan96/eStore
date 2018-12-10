using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for SAPOrderTest and is intended
    ///to contain all SAPOrderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SAPOrderTest
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
        ///A test for getOrderDatail
        ///</summary>
        [TestMethod()]
        public void getOrderDatailTest()
        {
            string storeId = "AUS"; // TODO: Initialize to an appropriate value
            string orderNo1 = "OUS007231"; // TODO: Initialize to an appropriate value
            SAPOrder target1 = new SAPOrder(storeId, orderNo1); // TODO: Initialize to an appropriate value
            SAPOrderResponse response1 = target1.getSAPOrderResponse();

            string orderNo2 = "OUS007448";
            SAPOrder target2 = new SAPOrder(storeId, orderNo2);
            SAPOrderResponse response2 = target2.getSAPOrderResponse();

            //Diaplay response content
            showSAPOrderResponse(response1);
            Console.WriteLine("=========================================================================");
            showSAPOrderResponse(response2);

            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        public void showSAPOrderResponse(SAPOrderResponse response)
        {
            Console.WriteLine();
            Console.WriteLine("->Order Info:");
            Console.WriteLine("     OrderNo: " + response.OrderInfo.OrderNo);
            Console.WriteLine("     ERP ID: " + response.OrderInfo.ERPID);
            Console.WriteLine("     PO NO: " + response.OrderInfo.PONO);
            Console.WriteLine("     Order note: " + response.OrderInfo.OrderNote);
            Console.WriteLine("     Ship Via: " + response.OrderInfo.ShipgVia);
            Console.WriteLine("     Shipment term: " + response.OrderInfo.ShipmentTerm);
            Console.WriteLine("     Due date: " + response.OrderInfo.DueDate);

            Console.WriteLine();
            Console.WriteLine("->Address Info: ");
            Console.WriteLine("       AddressType: " + response.AddressInfo.AddressType);
            Console.WriteLine("       Value: " + response.AddressInfo.Value);

            Console.WriteLine();
            Console.WriteLine("-> Order details");
            Console.WriteLine("Order no  |  Line no  |  Product line  |  Part no  |  Order line type  |  Qty  |  List price  |  Unit price  |  Required date  |  Due date  |  ERP site  |  ERP location  |  Auto order flag  |  Auto order Qty  |  Parent line no  |  Supplier due date  |  Subtotal  ");
            foreach (SAPOrderDetail d in response.OrderDetail)
            {
                Console.WriteLine(d.OrderNo + " | " + d.LineNo + " | " + d.ProductLine + " | " + d.PartNo + " | " + d.OrderLineType + " | " + d.Qty + " | " + d.ListPrice + " | " + d.UnitPrice + " | " + d.RequiredDate + " | " + d.DueDate + " | " + d.ERPsite + " | " + d.ERPLocation + " | " + d.AutoOrderFlag + " | " + d.AutoOrderQty + " | " + d.ParentLineNo + " | " + d.SupplierDueDate + " | " + d.Subtotal);
            }
        }
    }
}
