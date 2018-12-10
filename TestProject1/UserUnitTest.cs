using eStore.POCOS;
using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for UserUnitTest and is intended
    ///to contain all UserUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserUnitTest
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
        ///A test for hasRight
        ///</summary>
        [TestMethod()]
        public void AccessRightTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            eStore.BusinessModules.Store store = eSolution.getStore("AUS");
            User robotTester = store.getUser("eStoreTester@advantech.com");
            Console.WriteLine("Role : {0}    ActingRole : {1}", robotTester.role.ToString(), robotTester.actingRole.ToString());
            Assert.IsNotNull(robotTester);

            //at this time, user is not authenticated yet
            Console.WriteLine("ATP Access : {0}", robotTester.hasRight(User.ACToken.ATP).ToString());
            Assert.IsFalse(robotTester.hasRight(User.ACToken.ATP)); //not authenticated user shall not have this right
            Console.WriteLine("Can switch role : {0}", robotTester.hasRight(User.ACToken.SwitchRole).ToString());
            Assert.IsFalse(robotTester.hasRight(User.ACToken.SwitchRole)); //not authenticated user shall not have this right
            Console.WriteLine("Acting user is {0}", robotTester.actingUser.UserID);
            Console.WriteLine();

            robotTester = store.login("eStoreTester@advantech.com", "PowerUser", "localhost");
            Console.WriteLine("Role : {0}    ActingRole : {1}", robotTester.role.ToString(), robotTester.actingRole.ToString());
            Assert.IsNotNull(robotTester); //login shall return a user instance after successful authentication
            Console.WriteLine("ATP Access : {0}", robotTester.hasRight(User.ACToken.ATP).ToString());
            Assert.IsTrue(robotTester.hasRight(User.ACToken.ATP));  //robot tester shall have the access right after sign in
            Console.WriteLine("Can switch role : {0}", robotTester.hasRight(User.ACToken.SwitchRole).ToString());
            Assert.IsTrue(robotTester.hasRight(User.ACToken.SwitchRole)); //robot tester shall have the access right after sign in
            Console.WriteLine("Acting user is {0}", robotTester.actingUser.UserID);
            Console.WriteLine();

            robotTester.switchRole(User.Role.Customer, "jay.lee@advantech.com"); //act as customer
            Console.WriteLine("Role : {0}    ActingRole : {1}", robotTester.role.ToString(), robotTester.actingRole.ToString());
            Console.WriteLine("ATP Access : {0}", robotTester.hasRight(User.ACToken.ATP).ToString());
            Assert.IsFalse(robotTester.hasRight(User.ACToken.ATP)); //robotTester shall have no ATP access right when simulating as customer
            Console.WriteLine("Can switch role : {0}", robotTester.hasRight(User.ACToken.SwitchRole).ToString());
            Assert.IsTrue(robotTester.hasRight(User.ACToken.SwitchRole)); //robot tester shall have the access right after sign in
            Console.WriteLine("Acting user is {0}", robotTester.actingUser.UserID);
            Console.WriteLine();

            robotTester.switchRole(User.Role.Default);  //switch back to default role
            Console.WriteLine("Role : {0}    ActingRole : {1}", robotTester.role.ToString(), robotTester.actingRole.ToString());
            Console.WriteLine("ATP Access : {0}", robotTester.hasRight(User.ACToken.ATP).ToString());
            Assert.IsTrue(robotTester.hasRight(User.ACToken.ATP));  //robot tester shall have the access right after sign in
            Console.WriteLine("Can switch role : {0}", robotTester.hasRight(User.ACToken.SwitchRole).ToString());
            Assert.IsTrue(robotTester.hasRight(User.ACToken.SwitchRole)); //robot tester shall have the access right after sign in
            Console.WriteLine("Acting user is {0}", robotTester.actingUser.UserID);
        }
    }
}
