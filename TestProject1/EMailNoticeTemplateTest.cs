using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using esUtilities;
using eStore.POCOS.DAL;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for EMailNoticeTemplateTest and is intended
    ///to contain all EMailNoticeTemplateTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EMailNoticeTemplateTest
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
        [TestMethod()]
        public void resendorder()
        {
            eStore.BusinessModules.StoreSolution solution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store store = solution.getStore("AEU");
            string ORDERNO = "OEU022275";
            Order order = store.getOrder(ORDERNO);
            EMailNoticeTemplate target = new EMailNoticeTemplate(store); // TODO: Initialize to an appropriate value
            EMailReponse actual;
            actual = target.getOrderMailContent(order);
        }

        /// <summary>
        ///A test for getTransferredQuoteMailContent
        ///</summary>
        [TestMethod()]
        public void getTransferredQuoteMailContentTest()
        {
            eStore.BusinessModules.StoreSolution solution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store store = solution.getStore("AUS");

            string userId = "cherry.tsai@advantech.com.tw"; // TODO: Initialize to an appropriate value
            EMailNoticeTemplate target = new EMailNoticeTemplate(store); // TODO: Initialize to an appropriate value
            
            Quotation quote = null; // TODO: Initialize to an appropriate value
            string quoteNo = "QUS020210";
            QuotationHelper quotationHelper = new QuotationHelper();
            quote = quotationHelper.getQuoteByQuoteno(quoteNo);
            UserHelper userHelper = new UserHelper();
            User receiverUser = userHelper.getUserbyID(userId);  // TODO: Initialize to an appropriate value
            //EMailReponse expected = null; // TODO: Initialize to an appropriate value
            EMailReponse actual;
            actual = target.getTransferredQuoteMailContent(quote, receiverUser);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getCallMeNowContent
        ///</summary>
        [TestMethod()]
        public void getCallMeNowContentTest()
        {
            eStore.BusinessModules.StoreSolution solution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store store = solution.getStore("AUS");
            EMailNoticeTemplate target = new EMailNoticeTemplate(store); // TODO: Initialize to an appropriate value
            Dictionary<string, string> user = new Dictionary<string,string>(); // TODO: Initialize to an appropriate value
            user.Add("Country", "Taiwan");
            user.Add("EMail", "think.jimmy@gmail.com");
            user.Add("Name", "Jimmy Xiao");
            user.Add("Phone", "7781");
           
            //EMailReponse actual;
            //actual = target.getCallMeNowContent(user, store);
        }
    }
}
