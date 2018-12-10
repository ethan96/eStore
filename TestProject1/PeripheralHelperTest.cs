using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for PeripheralHelperTest and is intended
    ///to contain all PeripheralHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PeripheralHelperTest
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
        ///A test for InsertPeripheral
        ///</summary>
        [TestMethod()]
        public void InsertPeripheralTest()
        {
            PeripheralHelper target = new PeripheralHelper(); // TODO: Initialize to an appropriate value
            string partno = string.Empty; // TODO: Initialize to an appropriate value
            string main_category = string.Empty; // TODO: Initialize to an appropriate value
            string sub_category = string.Empty; // TODO: Initialize to an appropriate value
            string description = string.Empty; // TODO: Initialize to an appropriate value
            Decimal price = new Decimal(); // TODO: Initialize to an appropriate value
            string sapstatus = string.Empty; // TODO: Initialize to an appropriate value
            string version = string.Empty; // TODO: Initialize to an appropriate value
            string storeid = "";
            target.InsertPeripheral(partno, main_category, sub_category, description, price, sapstatus, version,storeid);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
