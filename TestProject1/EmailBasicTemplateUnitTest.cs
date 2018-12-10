using eStore.BusinessModules.Templates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using eStore.BusinessModules;
using esUtilities;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for EmailBasicTemplateUnitTest and is intended
    ///to contain all EmailBasicTemplateUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EmailBasicTemplateUnitTest
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
        ///A test for sendEmailInBasicTemplate
        ///</summary>
        [TestMethod()]
        public void sendEmailInBasicTemplateTest()
        {
            EmailBasicTemplate target = new EmailBasicTemplate(); // TODO: Initialize to an appropriate value
            Store store = StoreSolution.getInstance().getStore("AUS");
            Dictionary<String, String> requestInfo = new Dictionary<String, String>();
            string templateContent = string.Empty; // TODO: Initialize to an appropriate value
            EMailReponse actual;
            //prepare requestInfo
            ///     1. EmailSubject
            ///     3. EmailTo          optional -- if this is not provided, the email will be sent to emailInternalGroup or eStore IT only
            ///     7. EmailInternalGroup optional  -- if this is not provided, the email will be sent to eStore IT only
            ///     2. EmailSenderName  optional -- if not provided, it uses sendTo email as sender name
            ///     4. EmailFrom        optional  -- if not provided, it uses store.defaultContactEmail
            ///     5. EmailCC          optional 
            ///     6. EmailBcc         optional
            requestInfo.Add("EmailSubject", "Email Basic Template Unit test 1");
            actual = target.sendEmailInBasicTemplate(store, requestInfo, false, templateContent);

            requestInfo.Clear();
            requestInfo.Add("EmailSubject", "Email Basic Template Unit test 2");
            requestInfo.Add("EmailTo", "jay.lee@advantech.com");
            actual = target.sendEmailInBasicTemplate(store, requestInfo, false, templateContent);

            requestInfo.Clear();
            requestInfo.Add("EmailSubject", "Email Basic Template Unit test 3");
            requestInfo.Add("EmailTo", "jay.lee@advantech.com");
            requestInfo.Add("EmailSenderName", "Unit Tester");
            actual = target.sendEmailInBasicTemplate(store, requestInfo, false, templateContent);

            requestInfo.Clear();
            requestInfo.Add("EmailSubject", "Email Basic Template Unit test 4");
            requestInfo.Add("EmailTo", "jay.lee@advantech.com");
            requestInfo.Add("EmailSenderName", "Unit Tester");
            requestInfo.Add("EmailFrom", "jay.lee@advantech.com");
            requestInfo.Add("Testing message", "Unit test message");
            actual = target.sendEmailInBasicTemplate(store, requestInfo, false, templateContent);

            requestInfo.Clear();
            requestInfo.Add("EmailSubject", "Email Basic Template Unit test 5");
            requestInfo.Add("EmailTo", "jay.lee@advantech.com");
            requestInfo.Add("EmailSenderName", "Unit Tester");
            requestInfo.Add("EmailFrom", "jay.lee@advantech.com");
            requestInfo.Add("EmailCC", "missingland@yahoo.com");
            requestInfo.Add("Testing message", "Unit test message");
            actual = target.sendEmailInBasicTemplate(store, requestInfo, false, templateContent);

            requestInfo.Clear();
            requestInfo.Add("EmailSubject", "Email Basic Template Unit test 6");
            requestInfo.Add("EmailTo", "jay.lee@advantech.com");
            requestInfo.Add("EmailSenderName", "Unit Tester");
            requestInfo.Add("EmailFrom", "jay.lee@advantech.com");
            requestInfo.Add("EmailBcc", "missingland@yahoo.com");
            requestInfo.Add("Testing message", "Unit test message");
            actual = target.sendEmailInBasicTemplate(store, requestInfo, false, templateContent);

            requestInfo.Clear();
            requestInfo.Add("EmailSubject", "Email Basic Template Unit test 7");
            requestInfo.Add("EmailTo", "jay.lee@advantech.com");
            requestInfo.Add("EmailSenderName", "Unit Tester");
            requestInfo.Add("EmailFrom", "jay.lee@advantech.com");
            requestInfo.Add("EmailInternalGroup", "missingland@yahoo.com");
            requestInfo.Add("Testing message", "Unit test message");
            actual = target.sendEmailInBasicTemplate(store, requestInfo, false, templateContent);

            requestInfo.Clear();
            requestInfo.Add("EmailSubject", "Email Basic Template Unit test 8");
            requestInfo.Add("EmailTo", "jay.lee@advantech.com");
            requestInfo.Add("EmailSenderName", "Unit Tester");
            requestInfo.Add("EmailFrom", "jay.lee@advantech.com");
            requestInfo.Add("EmailInternalGroup", "missingland@yahoo.com");
            requestInfo.Add("Testing message", "Unit test message");
            actual = target.sendEmailInBasicTemplate(store, requestInfo, true, templateContent);

            Assert.IsNotNull(requestInfo);
        }
    }
}
