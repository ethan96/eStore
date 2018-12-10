using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ProductCategoryHelperTest and is intended
    ///to contain all ProductCategoryHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProductCategoryHelperTest
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


        // Extension method for IEnumerable<Item>
        public static void Traverse( IEnumerable<ProductCategory> source)
        {

            string tab = "";
            foreach(ProductCategory pc in source){
                if (pc.ParentCategory != null)
                    tab = "\t";
                else if (pc.ParentCategory != null && pc.ParentCategory.ParentCategory != null)
                    tab = "\t\t";
                
                
                Console.WriteLine(tab + pc.CategoryName);
                Traverse(pc.ChildCategories);

            }

        }


        /// <summary>
        ///A test for getCTOSRootProductCategory
        ///</summary>
        [TestMethod()]
        public void getCTOSRootProductCategoryTest()
        {
            ProductCategoryHelper target = new ProductCategoryHelper(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            List<ProductCategory> actual2; // TODO: Initialize to an appropriate value
            List<ProductCategory> actual;
            actual = target.getCTOSRootProductCategory(storeid);

            foreach (ProductCategory pc in actual) {
                Console.WriteLine(pc.CategoryName);            
            }

            actual2 = target.getCTOSRootProductCategory(storeid);

            foreach (ProductCategory pc in actual2)
            {
                Console.WriteLine(pc.CategoryName);
            }

        }

        /// <summary>
        ///A test for getStandardRootProductCategory
        ///</summary>
        [TestMethod()]
        public void getStandardRootProductCategoryTest()
        {
            ProductCategoryHelper target = new ProductCategoryHelper(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            List<ProductCategory> actual;
            actual = target.getStandardRootProductCategory(storeid);
            foreach (ProductCategory pc in actual)
            {
                Console.WriteLine(pc.CategoryName);
            }
        }

        /// <summary>
        ///A test for getProductCategoryByPartno
        ///</summary>
        [TestMethod()]
        public void getProductCategoryByPartnoTest()
        {
            ProductCategoryHelper target = new ProductCategoryHelper(); // TODO: Initialize to an appropriate value
            Part part = new PartHelper().getPart("EKI-1121L-AE",new StoreHelper().getStorebyStoreid("AUS")); // TODO: Initialize to an appropriate value
            string storeid = "AUS"              ; // TODO: Initialize to an appropriate value
            List<ProductCategory> actual;
            actual = target.getProductCategoryByPartno(part, storeid);

            Assert.IsNotNull(actual);

            foreach (ProductCategory pc in actual) {

                Console.WriteLine(pc.CategoryName);
                if(pc.ParentCategory!=null)
                    Console.WriteLine(pc.ParentCategory.CategoryName );
            }

        }


        [TestMethod()]
        public void RefreshCategoryMinPrice()
        {
            ProductCategoryHelper _hepler = new ProductCategoryHelper();
            var rootCategories = _hepler.getProductCategories("AKR")
                .Where(c => (c.ParentCategoryID.HasValue == false)).ToList();
            foreach (var category in rootCategories)
                category.refreshMinimumPrice();
        }

    }

}
