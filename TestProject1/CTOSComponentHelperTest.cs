using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for CTOSComponentHelperTest and is intended
    ///to contain all CTOSComponentHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CTOSComponentHelperTest
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
        ///A test for getRootCTOSComponent
        ///</summary>
        [TestMethod()]
        public void getRootCTOSComponentTest()
        {
            CTOSComponentHelper target = new CTOSComponentHelper(); // TODO: Initialize to an appropriate value
            List<CTOSComponent> actual;
            actual = target.getRootCTOSComponent("AUS");

            foreach (CTOSComponent cc in actual)
                Console.WriteLine(cc.ComponentName + ":" + cc.ComponentDesc);

        }
    }
}
