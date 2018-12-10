using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for MenuHelperTest and is intended
    ///to contain all MenuHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MenuHelperTest
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
        ///A test for getMenusByStoreid
        ///</summary>
        [TestMethod()]
        public void getMenusByStoreidTest()
        {
            MenuHelper target = new MenuHelper(); // TODO: Initialize to an appropriate value
            List<Menu> actual;       
  
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
 
            actual = target.getMenusByStoreid(storeid,Menu.MenuPosition.Header);
            foreach (Menu pc in actual)
            {

                Console.WriteLine(pc.MenuName);

                foreach (Menu pcs in pc.SubMenus)
                {
                    Console.WriteLine("\t" + pcs.MenuName);

                    foreach (Menu pcss in pcs.SubMenus)
                    {
                        Console.WriteLine("\t\t*" + pcss.MenuName);
                    }

                }

            }
        }

        /// <summary>
        ///A test for delete
        ///</summary>
        [TestMethod()]
        public void deleteTest()
        {
            //MenuHelper target = new MenuHelper(); // TODO: Initialize to an appropriate value
            //Menu _menu = new MenuHelper().getMenusByid(395); // TODO: Initialize to an appropriate value
            //int expected = 0; // TODO: Initialize to an appropriate value
            //int actual;
            //actual = target.delete(_menu);

            //Assert.IsTrue(actual == 0);
        }
    }
}
