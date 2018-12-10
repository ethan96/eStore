using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for INIpaySolutionTest and is intended
    ///to contain all INIpaySolutionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class INIpaySolutionTest
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
        ///A test for StartChkFake
        ///</summary>
        [TestMethod()]
        [DeploymentItem("eStore.BusinessModules.dll")]
        public void StartChkFakeTest()
        {
            INIpaySolution target = new INIpaySolution();
            //INIpaySolution_Accessor target = new INIpaySolution_Accessor(); // TODO: Initialize to an appropriate value
            //target.StartChkFake();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
