using eStore.POCOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using eStore.POCOS.DAL;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ProductCategoryUnitTest and is intended
    ///to contain all ProductCategoryUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProductCategoryUnitTest
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
        ///A test for getLowestPrice
        ///</summary>
        [TestMethod()]
        public void getLowestPriceTest()
        {
            ProductCategoryHelper helper = new ProductCategoryHelper();

            //select one category of standard product and one category of CTOS
            ProductCategory sCategory = helper.getStandardRootProductCategory("AUS")[0];
            ProductCategory cCategory = helper.getCTOSRootProductCategory("AUS")[0];

            if (cCategory != null)
            {
                Decimal actual = cCategory.getLowestPrice();
                Assert.IsFalse((actual == 0));
                Console.WriteLine("Category {0} lowest price is {1}", cCategory.CategoryName, actual);

                actual = cCategory.getHighestPrice();
                Assert.IsFalse((actual == 0));
                Console.WriteLine("Category {0} highest price is {1}", cCategory.CategoryName, actual);
            }

            if (sCategory != null)
            {
                Decimal actual = sCategory.getLowestPrice();
                Assert.IsFalse((actual == 0));
                Console.WriteLine("Category {0} lowest price is {1}", sCategory.CategoryName, actual);

                actual = sCategory.getHighestPrice();
                Assert.IsFalse((actual == 0));
                Console.WriteLine("Category {0} highest price is {1}", sCategory.CategoryName, actual);
            }
        }

        /// <summary>
        ///A test for getLowestPrice
        ///</summary>
        [TestMethod()]
        public void getLowestPricePreciseTest()
        {
            ProductCategoryHelper helper = new ProductCategoryHelper();

            ProductCategory category = helper.getProductCategory("EPPEZIPC", "AUS");

            if (category != null)
            {
                Decimal actual = category.getLowestPrice();
                Assert.IsFalse((actual == 0));
                Console.WriteLine("Category {0} lowest price is {1}", category.CategoryName, actual);

                actual = category.getHighestPrice();
                Assert.IsFalse((actual == 0));
                Console.WriteLine("Category {0} highest price is {1}", category.CategoryName, actual);
            }

        }
    
    }
}
