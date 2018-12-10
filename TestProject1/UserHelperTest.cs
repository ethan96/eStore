using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using eStore.POCOS.PocoX;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for UserHelperTest and is intended
    ///to contain all UserHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserHelperTest
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
        ///A test for getUserbyID
        ///</summary>
        [TestMethod()]
        public void getUserbyIDTest()
        {
            string email = "edward.keh@advantech.com.tw"; // TODO: Initialize to an appropriate value
            User expected = new User(); // TODO: Initialize to an appropriate value
            User actual;

            expected.CompanyID="UWE2225";
            actual = new UserHelper().getUserbyID(email);

            foreach (Contact c in actual.Contacts) {
                Console.WriteLine(c.UserID);
            }

            Assert.AreEqual(expected.CompanyID, actual.CompanyID);
            
        }

        /// <summary>
        /// Validation test
        ///</summary>
        [TestMethod()]
        public void saveTest()
        {
            //User _user = UserHelper.getUserbyID("edward.keh@advantech.com.tw"); // TODO: Initialize to an appropriate value
            User _user = new User();
            _user.CompanyID = "testID";

            UserHelper uhelper = new UserHelper();

            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;

            if (_user.validate() == false)

            {
                foreach (ErrorMessage em in _user.error_message)
                {
                    Console.Write(em.columname + ":" + em.message);
                }
            }

            Assert.IsTrue(_user.error_message.Count > 0);

            actual= uhelper.save(_user);

            expected = -5000; // expected will failed due to no userID
            Assert.AreEqual(expected, actual);

            //Retrieve after change
            User actualsave = uhelper.getUserbyID(_user.UserID);
            //Assert.AreEqual(_user.CompanyID, actualsave.CompanyID);
        }

        /// <summary>
        ///  Test if return by reference, purposely send an incomplete object to save
        ///</summary>
        [TestMethod()]
        public void saveTest_return()
        {
            
            User _user = new User();
            _user.CompanyID = "testID";
            UserHelper uhelper = new UserHelper();
             
            int actual;

            // Return -1 for validation false or null
            // Return -5000 for Entity framework operation error , please refer to table log4net for detail.        
            actual = uhelper.save(_user);
            _user.UserID = "test@advantech.com";
            actual = uhelper.delete(_user);

            //should return number over 1, since this is an incomplete object
            Console.WriteLine(_user.error_message.Count);

            //Should be true, since this object voilate validation rules.
            Assert.IsTrue(_user.error_message.Count > 0);
            
        }

        /// <summary>
        ///A test for getRegisteredUsers
        ///</summary>
        [TestMethod()]
        public void getRegisteredUsersTest()
        {
            UserHelper target = new UserHelper(); // TODO: Initialize to an appropriate value
            DMF dmf = new DMFHelper().getDMFbyID("ANA DMF");
            DateTime startdate = new DateTime(2010,12,15); // TODO: Initialize to an appropriate value
            DateTime enddate = new DateTime(2010,12,15); // TODO: Initialize to an appropriate value
            List<Member> actual;
            actual = target.getRegisteredUsers(dmf, startdate, enddate);

            foreach (Member m in actual)
                Console.WriteLine(m.EMAIL_ADDR + ":" + m.Insert_date);

        }
    }
}
