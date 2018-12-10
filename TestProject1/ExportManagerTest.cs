using eStore.BusinessModules.LTron;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ExportManagerTest and is intended
    ///to contain all ExportManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExportManagerTest
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
///A test for export
///</summary>
[TestMethod()]
public void exportTest()
{
    ExportManager target = new eStore.BusinessModules.LTron.ExportManager(); // TODO: Initialize to an appropriate value
    string expected = string.Empty; // TODO: Initialize to an appropriate value
    string actual;
    //actual = target.exportCategories();
    //System.IO.StreamWriter file = new System.IO.StreamWriter("d:\\temp\\Categories.xml");
    //file.WriteLine(actual);
    //file.Close();

    //actual = target.exportProducts();
    //System.IO.StreamWriter Productsfile = new System.IO.StreamWriter("d:\\temp\\Products.xml");
    //Productsfile.WriteLine(actual);
    //Productsfile.Close();

    actual = target.exportProduct("ADAM-5510M-A2E");
    System.IO.StreamWriter Productfile = new System.IO.StreamWriter("d:\\temp\\Product_ADAM-5510M-A2E.xml");
    Productfile.WriteLine(actual);
    Productfile.Close();

    actual = target.exportProduct("EKI-2725-BE");
    System.IO.StreamWriter Productfile2 = new System.IO.StreamWriter("d:\\temp\\Product_EKI-2725-BE.xml");
    Productfile2.WriteLine(actual);
    Productfile2.Close();
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("Verify the correctness of this test method.");
}

/// <summary>
///A test for exportProducts
///</summary>
[TestMethod()]
public void exportProductsTest()
{
    ExportManager target = new ExportManager(); // TODO: Initialize to an appropriate value
  string[] productids =  { "ADAM-3011-AE", "ADAM-3114-AE", "ADAM-3112-AE", "ADAM-3016-AE", "ADAM-3014-AE" };
   
    string expected = string.Empty; // TODO: Initialize to an appropriate value
    string actual;
    actual = target.exportProducts(productids.ToList());
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("Verify the correctness of this test method.");
}
    }
}
