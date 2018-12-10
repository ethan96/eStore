using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for WidgetPageHelperTest and is intended
    ///to contain all WidgetPageHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WidgetPageHelperTest
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
            WidgetPageHelper target = new WidgetPageHelper(); // TODO: Initialize to an appropriate value
            WidgetPage widgetpage = new WidgetPage() ; // TODO: Initialize to an appropriate value

            widgetpage.Publish = true;
            widgetpage.LastUpdated = DateTime.Now;
            widgetpage.LastUpdateBy = "edward.keh";
            widgetpage.PageDescription = "test";
            widgetpage.PageName = "aaa";
            widgetpage.Path = "c:\\";
            widgetpage.FileName = "ccc.html";
            widgetpage.StoreID = "AUS";

            int actual;
            actual = target.save(widgetpage);
            Assert.IsTrue(actual == 0);

            actual = target.delete(widgetpage);
            Assert.IsTrue(actual == 0);

        }


        [TestMethod()]
        public void saveTest2()
        {
            WidgetPageHelper target = new WidgetPageHelper(); // TODO: Initialize to an appropriate value
            WidgetPage widgetpage = target.getWidgetPageByID(11);
            List<Widget> wids = (from a in widgetpage.Widgets
                                 select a).ToList();

            foreach (Widget w in wids) {
                target.delete(w);                 
            
            }
             

        }
    }
}
