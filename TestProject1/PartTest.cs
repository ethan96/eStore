using eStore.POCOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS.DAL;
namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for PartTest and is intended
    ///to contain all PartTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PartTest
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
        ///A test for getATP
        ///</summary>
        [TestMethod()]
        [DeploymentItem("esPOCOS.dll")]
        public void getATPTest()
        {
            //PartHelper target = new PartHelper(); // TODO: Initialize to an appropriate value
            //ATP expected = null; // TODO: Initialize to an appropriate value
            //ATP actual;
            //actual = target.getat(sourcingQty);
        }

        /// <summary>
        ///A test for getATP
        ///</summary>
        [TestMethod()]
        public void getATPTest1()
        {
            //Store s = new StoreHelper().getStorebyStoreid("AUS");
            //Part target = new PartHelper().getPart("AIMB-780QG2-00A1E", s); // TODO: Initialize to an appropriate value
            //int sourcingQty = 1; // TODO: Initialize to an appropriate value
            //ATP expected = null; // TODO: Initialize to an appropriate value
            //ATP actual;
            //actual = target.getATP(sourcingQty);
            //Assert.IsNotNull(actual);
            //Console.WriteLine(actual.availableDate + ":" + actual.availableQty);
        }
    }
}
