using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for StateZipHelperTest and is intended
    ///to contain all StateZipHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StateZipHelperTest
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
        ///A test for getStatebyZip
        ///</summary>
        [TestMethod()]
        public void getStatebyZipTest()
        {
            string country = "Australia"; // TODO: Initialize to an appropriate value
            string zipcode = "4753"; // TODO: Initialize to an appropriate value
            StateZip actual = null; // TODO: Initialize to an appropriate value
            StateZip expected  = new StateZip();
            expected.Zipcode = "QLD";
            actual = new StateZipHelper().getStatebyZip(country, zipcode);
            Assert.AreEqual(expected.Zipcode, actual.State );
            
        }
    }
}
