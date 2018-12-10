using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for AllSequenceHelperTest and is intended
    ///to contain all AllSequenceHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AllSequenceHelperTest
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
        ///A test for GetNewSeq
        ///</summary>
        [TestMethod()]
        public void GetNewSeqTest()
        {
            Store _store = new StoreHelper().getStorebyStoreid("AUS"); // TODO: Initialize to an appropriate value
            string _seqname = "OrderPrefix"; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;

            for (int i = 0; i < 10; )
            {
                actual = AllSequenceHelper.GetNewSeq(_store, _seqname);
                Console.WriteLine(actual);
                i++;

            }
             
        }
    }
}
