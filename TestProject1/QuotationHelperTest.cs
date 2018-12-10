using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for QuotationHelperTest and is intended
    ///to contain all QuotationHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QuotationHelperTest
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
        ///A test for getUserQuotes
        ///</summary>
        [TestMethod()]
        public void getUserQuotesTest()
        {
            QuotationHelper target = new QuotationHelper(); // TODO: Initialize to an appropriate value
            string userid = "james.wu@advantech.com.cn"; // TODO: Initialize to an appropriate value
            //bool includeAbandon = false; // TODO: Initialize to an appropriate value
            List<Quotation> actual;


            actual = target.getUserQuotes(userid,"AUS", false);

            target.setSource(actual);

            foreach (Quotation q in actual)
            {
                Console.WriteLine(q.QuotationNumber);
            }

            Console.WriteLine("All quotes");
            actual = target.getUserQuotes(userid, "AUS", true);

            foreach (Quotation q in actual) {
                Console.WriteLine(q.QuotationNumber);
            }
        }

        /// <summary>
        ///A test for getLastestOpenQuote
        ///</summary>
        [TestMethod()]
        public void getLastestOpenQuoteTest()
        {
            QuotationHelper target = new QuotationHelper(); // TODO: Initialize to an appropriate value
            string userid = "mike.liu@advantech.com.cn"; // TODO: Initialize to an appropriate value
            Quotation actual;
            actual = target.getLastestOpenQuote(userid,"AUS");

            Console.WriteLine(actual.QuotationNumber);
        }

        /// <summary>
        ///A test for getQuoteByQuoteno
        ///</summary>
        [TestMethod()]
        public void getQuoteByQuotenoTest()
        {
            QuotationHelper target = new QuotationHelper(); // TODO: Initialize to an appropriate value
            string Quoteno = "QUS020210"; // TODO: Initialize to an appropriate value
            Quotation actual;
            actual = target.getQuoteByQuoteno(Quoteno);
            showCartItems(actual.cartX.CartItems, true);
        }

        private static string showCartItems(ICollection<CartItem> cartItems, bool detailMode)
        {
            string quoteContent = null;
            var _cartitem = cartItems.OrderBy(cartitem => cartitem.type);

            foreach (CartItem c in _cartitem)
            {
                Console.WriteLine("ProductType = " + c.type);
                Console.WriteLine("ProductNameX = " + c.productNameX);
                Console.WriteLine("Description = " + c.Description);
                Console.WriteLine("Available Date = "+c.atp.availableDate);
                Console.WriteLine("Available Qty = "+c.atp.availableQty);
                Console.WriteLine("Unit price = " + c.UnitPrice);
                Console.WriteLine("Order Qty = " + c.Qty);
                Console.WriteLine("Adjusted price = " + c.AdjustedPrice);
                if (c.type == Product.PRODUCTTYPE.CTOS)
                {
                    Console.WriteLine("-- BTOS --");
                    Console.WriteLine("BTOS no = " + c.BTOSystem.BTONo);
                    foreach (BTOSConfig bc in c.BTOSystem.BTOSConfigsWithoutNoneItems)
                    {
                        //foreach (BTOSConfigDetail bcd in bc.BTOSConfigDetails)
                        //{
                        //    Console.Write("SProductID -> " + bcd.SProductID);
                        //}
                        ATP _apt = null;
                        foreach (KeyValuePair<Part, int> pair in bc.parts)
                        {
                            _apt = pair.Key.atp;
                        }

                        Console.WriteLine("Available date : " + _apt.availableDate + ", Qty: " + _apt.availableQty);
                    }
                }
                
                Console.WriteLine();
                Console.WriteLine();
            }

            return quoteContent;
        }

        /// <summary>
        ///A test for setSource
        ///</summary>
        [TestMethod()]
        public void setSourceTest()
        {
            QuotationHelper target = new QuotationHelper(); // TODO: Initialize to an appropriate value
            List<Quotation> quotes = null; // TODO: Initialize to an appropriate value
            List<Quotation> expected = null; // TODO: Initialize to an appropriate value
            List<Quotation> actual;
            actual = target.setSource(quotes);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
