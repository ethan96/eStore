using eStore.POCOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for CampaignStrategyTest and is intended
    ///to contain all CampaignStrategyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CampaignStrategyTest
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
        ///A test for reloadXRule
        ///</summary>
        [TestMethod()]
        public void reloadXRuleTest()
        {
            CampaignStrategy target; // TODO: Initialize to an appropriate value
            Campaign cam = (new eStore.POCOS.DAL.CampaignHelper()).getCampaignByID("AEU", 101);
            target = cam.CampaignStrategies.FirstOrDefault();
            if (target != null)
            {
                //target.reloadXRule();
                //target.CategoryIDList = "SYS-4U4000";
                eStore.POCOS.PocoX.XRule4Gift gift = new eStore.POCOS.PocoX.XRule4Gift();
                gift.CheckGiftInDetails = true;
                gift.PerUnit = false;
                List<eStore.POCOS.PocoX.XRule.Condition> estoregift = new List<eStore.POCOS.PocoX.XRule.Condition>();


                estoregift.Add(new eStore.POCOS.PocoX.XRule.Condition("PS8-300ATX-ZBE", 1));
                Dictionary<string, int> Criteria = new Dictionary<string, int>();
                List<eStore.POCOS.PocoX.XRule.Condition> estoreCriteria = new List<eStore.POCOS.PocoX.XRule.Condition>();
                //estoreCriteria.Add(new eStore.POCOS.PocoX.XRule.Condition("IPC-7220-00XQE", 1));
                //estoreCriteria.Add(new eStore.POCOS.PocoX.XRule.Condition("ACP-4320MB-00XQE", 1));

                gift.Gift = estoregift;
                gift.Criteria = estoreCriteria;

                target.xrule = gift;
            }


            cam.save();
        }
    }
}
