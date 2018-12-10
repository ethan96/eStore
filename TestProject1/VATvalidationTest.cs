using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for VATvalidationTest and is intended
    ///to contain all VATvalidationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VATvalidationTest
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
        ///A test for validateVATformat
        ///</summary>
        [TestMethod()]
        [DeploymentItem("eStore.BusinessModules.dll")]
        public void validateVATformatTest()
        {

            /* Following are valid VAT number from eStore
             *  Austrin                     ATU60364304 /   wolfgang.vogl1@aon.at
             *  Belgium                   n/a
             *  Bulgaria
             *  CY                              CY10041567G, CY10252176N
             *  CZ                              CZ61467839
             *  DE                              DE134800516
             *  DK                              DK14145486
             *  EE                               EE100900754
             *  EL                               EL095406833
             *  ES                               ESB81766149, ESN8001029A
             *  FI                                FI 22307759
             *  FR                               FR77521435388
             *  HU                             HU13117649
             *  IE                                IE 6405569S
             *  IT                                IT01460790338, IT00241050442
             *  LT                               LT100004514313
             *  LU                              LU22207924
             *  LV                              LV40003735477, LV40003947834
             * MT                              MT12979127
             * NL                               NL009631513B01
             * PL                                PL5262709819
             * PT                                PT504104039
             * RO                               RO14810481, RO7507036
             * SE                                SE556540702901
             * SK                                SK2020057754
             * GB                               GB769434487
             */

            string vatNumber = "GB769434487";  // EU vat number 
            string userId = "jimmy.xiao@advantech.com.tw";
            UserHelper uhelper = new UserHelper();
            User user = uhelper.getUserbyID(userId);

            VATvalidation target = new VATvalidation(vatNumber, VATvalidation.Method.FormatValidation, user); // TODO: Initialize to an appropriate value
            bool validationrResult;
            validationrResult = target.checkVAT();
            Console.WriteLine("VAT Number: " + vatNumber);
            Console.WriteLine(validationrResult);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
