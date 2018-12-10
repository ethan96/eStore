using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.BusinessModules;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for EMailDiscountRequestTest and is intended
    ///to contain all EMailDiscountRequestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EMailDiscountRequestTest
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
        ///A test for getDiscountRequestEmailTemplate
        ///</summary>
        [TestMethod()]
        public void getDiscountRequestEmailTemplateTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store store = eSolution.getStore("AUS") ; // TODO: Initialize to an appropriate value
            User user = store.getUser("jimmy.xiao@advantech.com.tw");
            Product product = store.getProduct("1");
            DateTime expectedDate = new DateTime(2010,10,22);
            //RequestDiscount request = new RequestDiscount(product, 20, expectedDate,"200000", "Jimmy", "Xiao", "US", "Advantech", "380 Fairview way, Milpitas, 95035 CA", "jimmy.xiao@advantech.com.tw", "n/a", "Email", "Install Linux", DateTime.Now);
                     
            //string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            //actual = EMailDiscountRequest.getDiscountRequestEmailTemplate(store, request);
            //Console.WriteLine("Volumn discount request email:<br />  " + actual);
        }
    }
}
