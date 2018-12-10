using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for DHLRateHelperTest and is intended
    ///to contain all DHLRateHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DHLRateHelperTest
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
        ///A test for getDHLRate
        ///</summary>
        [TestMethod()]
        public void getDHLRateTest()
        {
            string countryShort = "AT"; // Austria
            Decimal totalweight = new Decimal(14); // TODO: Initialize to an appropriate value
            DHLRate expected = new DHLRate(); // TODO: Initialize to an appropriate value
            expected.Price = 4319;
            DHLRate actual;
            actual = DHLRateHelper.getDHLRate(countryShort, totalweight);
            Console.WriteLine(actual.Price);
            Assert.AreEqual(expected.Price, actual.Price);
           
        }
    }
}
