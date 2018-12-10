using eStore.WindowService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ProductSpecProcessTest and is intended
    ///to contain all ProductSpecProcessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProductSpecProcessTest
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
        ///A test for createMainCatAttributeValue
        ///</summary>
        [TestMethod()]
        public void createMainCatAttributeValueTest()
        {
            ProductSpecProcess target = new ProductSpecProcess(); // TODO: Initialize to an appropriate value
            target.createMainCatAttributeValue();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

      
        /// <summary>
        ///A test for createAttributeValue_Category2
        ///</summary>
        [TestMethod()]
        public void createAttributeValue_CategoryTest()
        {
            ProductSpecProcess target = new ProductSpecProcess(); // TODO: Initialize to an appropriate value
            target.createAttributeValue();
             
        }

      

        /// <summary>
        ///Create Category & Category 2 product specs
        ///</summary>
        [TestMethod()]
        public void createproductspecTest()
        {
            ProductSpecProcess target = new ProductSpecProcess(); // TODO: Initialize to an appropriate value
            target.createproductspec();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }



        /// <summary>
        ///A test for createRules
        ///</summary>
        [TestMethod()]
        public void createRulesTest()
        {
            ProductSpecProcess target = new ProductSpecProcess(); // TODO: Initialize to an appropriate value
            target.createRules("AUS");
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for createAttributeValue
        ///</summary>
        [TestMethod()]
        public void createAttributeValueTest()
        {
            ProductSpecProcess target = new ProductSpecProcess(); // TODO: Initialize to an appropriate value
            target.createAttributeValue();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }




       






        /// <summary>
        ///A test for createRules
        ///</summary>
        [TestMethod()]
        public void createRulesTest1()
        {
            ProductSpecProcess target = new ProductSpecProcess(); // TODO: Initialize to an appropriate value
            target.createRules("AEU");
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for createproductspec
        ///</summary>
        [TestMethod()]
        public void createproductspecTest1()
        {
            ProductSpecProcess target = new ProductSpecProcess(); // TODO: Initialize to an appropriate value
            target.createproductspec();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
