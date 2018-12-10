using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using System.IO;
using System.Text;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for AddressValidationTest and is intended
    ///to contain all AddressValidationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddressValidationTest
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
        ///A test for validateAddress
        ///</summary>
        [TestMethod()]
        [DeploymentItem("eStore.BusinessModules.dll")]
        public void validateAddressTest()
        {
            //PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            //AddressValidation_Accessor target = new AddressValidation_Accessor(param0); // TODO: Initialize to an appropriate value
            //target.validateAddress();
            Address address = new Address();
            address.ZipCode = "04578-000";
            address.City = "";
            address.State = "";
            address.Country = "US";

            AddressValidation addr = new AddressValidation(address, AddressValidation.ValidateType.Zipcode);
            Console.WriteLine("Validation result: " + addr.ValidationResult);
            Console.WriteLine("Error code: " + addr.ValidationResultDescription);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod()]
        public void myTest()
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("Now: " + now.ToString());
        }

    }
}
