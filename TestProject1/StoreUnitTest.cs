using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Globalization;
using System.Collections.Generic;
using esUtilities;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for StoreUnitTestLogin and is intended
    ///to contain all StoreUnitTestLogin Unit Tests
    ///</summary>
    [TestClass()]
    public class StoreUnitTest
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
        ///A test for login
        ///</summary>
        [TestMethod()]
        public void loginTest()
        {
            eStore.BusinessModules.StoreSolution solution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store aus = solution.getStore("AUS");

            string userId = "testdrive@advantech.com"; // TODO: Initialize to an appropriate value
            string password = "testdrive"; // TODO: Initialize to an appropriate value
            string userHostIP = "172.21.1.100"; // TODO: Initialize to an appropriate value
            User actual = null;

            //sign in 
            actual = aus.login(userId, password, userHostIP);           
            Assert.IsNotNull(actual);

            //sign off
            aus.logout(actual);

            password = "Wrong"; // TODO: Initialize to an appropriate value

            actual = aus.login(userId, password, userHostIP);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for getTempalte
        ///</summary>
        [TestMethod()]
        public void getTemplateTest()
        {
            eStore.BusinessModules.StoreSolution solution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store aus = solution.getStore("AUS");

            String AccountEstablishDoc = aus.getTemplate(eStore.BusinessModules.Store.TEMPLATE_TYPE.AccountEstablish);

            Assert.IsNotNull(AccountEstablishDoc);
        }


        /// <summary>
        ///A test for getTempalte
        ///</summary>
        [TestMethod()]
        public void adjustTimeAndFormatTest()
        {
            eStore.BusinessModules.StoreSolution solution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store aus = solution.getStore("AUS");

            DateTime sourceTime = DateTime.Now;
            TimeSpan targetOffset = new TimeSpan(5, 30, 0);

            Console.WriteLine("US format -- {0}", aus.convertTimeAndFormat(sourceTime, "en-US", targetOffset));

            Console.WriteLine("Taiwan format -- {0}", aus.convertTimeAndFormat(sourceTime, "zh-TW", targetOffset));

            Console.WriteLine("Japan format -- {0}", aus.convertTimeAndFormat(sourceTime, "ja-JP", targetOffset));

            Console.WriteLine("Korea format -- {0}", aus.convertTimeAndFormat(sourceTime, "ko-KR", targetOffset));

            Console.WriteLine("US format -- {0}", aus.convertTimeAndFormat(sourceTime, "", targetOffset));

            eStore.BusinessModules.Store akr = solution.getStore("AKR");
            Console.WriteLine("Korea format -- {0}", akr.convertTimeAndFormat(sourceTime, ""));            
        }

        /// <summary>
        ///A test for IPtoNation
        ///</summary>
        [TestMethod()]
        public void IPtoNationTest()
        {
            Console.WriteLine(CommonHelper.IPtoNation("172.21.1.1."));

            Console.WriteLine("US {0}", CommonHelper.IPtoNation("71.4.62.29"));
            Console.WriteLine("Taiwan {0}", CommonHelper.IPtoNation("219.90.3.201"));
            Console.WriteLine("UK {0}", CommonHelper.IPtoNation("217.146.186.221"));
            Console.WriteLine("Germany {0}", CommonHelper.IPtoNation("212.202.248.202"));
            Console.WriteLine("China {0}", CommonHelper.IPtoNation("61.135.163.80"));
        }

        /// <summary>
        ///A test for getMatchParts
        ///</summary>
        [TestMethod()]
        public void getMatchPartsTest()
        {
            eStore.BusinessModules.StoreSolution solution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store aus = solution.getStore("AUS");

            String keyword = "AIMB-212N-S6A1E";
            IList<Part> actual = aus.getMatchParts(keyword);

            Assert.IsNotNull(actual);
            Assert.IsTrue((actual.Count > 0));
        }

        /// <summary>
        ///A test for getCurrencyExchangeValue
        ///</summary>
        [TestMethod()]
        public void getCurrencyExchangeValueTest()
        {
            eStore.BusinessModules.StoreSolution solution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store sap = solution.getStore("SAP");

            Decimal converted = sap.getCurrencyExchangeValue(100, "SGD");
            Assert.IsTrue(converted > 0);
            converted = sap.getCurrencyExchangeValue(100, "NTD");
            Assert.IsTrue(converted == 0);
        }
    }
}
