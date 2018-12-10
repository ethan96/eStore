using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for CTOSAttributeLangHelperTest and is intended
    ///to contain all CTOSAttributeLangHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CTOSAttributeLangHelperTest
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
        ///A test for save
        ///</summary>
        [TestMethod()]
        public void saveTest()
        {
            CTOSAttributeLangHelper target = new CTOSAttributeLangHelper(); // TODO: Initialize to an appropriate value
            CTOSAttributelang _spec = target.getCTOSLang(1, "EN");
             
            _spec.LangValue = "Edward test";
            
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.save(_spec);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
