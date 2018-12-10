using eStore.POCOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using eStore.BusinessModules;
using eStore.POCOS.DAL;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for QuotationUnitTest and is intended
    ///to contain all QuotationUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QuotationUnitTest
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
        ///A test for transfer
        ///</summary>
        [TestMethod()]
        public void transferTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store store = eSolution.getStore("AUS");
            User robotTester = store.login("eStoreTester@advantech.com", "PowerUser", "localhost");

            Quotation quotation = (new QuotationHelper()).getQuoteByQuoteno("QUOTE.20101014_165830_031");

            Console.WriteLine("Transferred quotation : " + quotation.QuotationNumber);
            Console.WriteLine("----------------------------------");

            User robotTester2 = store.login("jay.lee@advantech.com", "TestAccount", "localhost");
            quotation.transfer(robotTester2);

            printQuotationList(robotTester.quotations);
            printQuotationList(robotTester2.quotations);
        }

        private void printQuotationList(IList<Quotation> quotations)
        {
            foreach (Quotation quote in quotations)
                Console.WriteLine("Quotation No : " + quote.QuotationNumber);

            Console.WriteLine("------------------------");
        }
    }
}
