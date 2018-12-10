using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for CTOSComparisionHelperTest and is intended
    ///to contain all CTOSComparisionHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CTOSComparisionHelperTest
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
        ///A test for getCtosAttribute
        ///</summary>
        [TestMethod()]
        public void getCtosAttributeTest()
        {
           // CTOSComparisionHelper target = new CTOSComparisionHelper(); // TODO: Initialize to an appropriate value
           // List<CTOSAttribute> expected = null; // TODO: Initialize to an appropriate value
           // List<CTOSAttribute> actual;
           //// actual = target.getCtosAttribute();
             
           // foreach(CTOSAttribute ct in actual){
           //     Console.WriteLine(ct.AttrName);
           //     foreach (CTOSAttributeValue ctv in ct.CTOSAttributeValues)
           //         Console.WriteLine(ctv.AttrID + ":" + ctv.AttrValue_name  + ":" + ctv.AttrValue_desc);
           // }
        }
    }
}
