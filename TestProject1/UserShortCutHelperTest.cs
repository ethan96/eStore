using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for UserShortCutHelperTest and is intended
    ///to contain all UserShortCutHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserShortCutHelperTest
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
        ///A test for save
        ///</summary>
        [TestMethod()]
        public void saveTest()
        {
            UserShortCutHelper target = new UserShortCutHelper(); // TODO: Initialize to an appropriate value
            UserShortCut _usershortcut =  new UserShortCut() ; // TODO: Initialize to an appropriate value

            _usershortcut.ShortCutName = "test";
            _usershortcut.SProductIDs = "1,2,3,4,5";
            _usershortcut.StoreID = "AUS";
            _usershortcut.URL = "http://test";
            _usershortcut.UserID = "edward.keh@advantech.com.tw";
            _usershortcut.CreatedDate = DateTime.Now;    
                

            int actual;
            actual = target.save(_usershortcut);
            Assert.IsTrue(actual == 0);



            actual = target.delete(_usershortcut);
            Assert.IsTrue(actual == 0);

        }
    }
}
