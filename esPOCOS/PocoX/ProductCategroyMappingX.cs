using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class ProductCategroyMapping { 
 
	 #region Extension Methods 

 #endregion 

    public ProductCategroyMapping()
    { }

    public ProductCategroyMapping(ProductCategory pc, Part p)
    {
        this.CategoryPath = pc.CategoryPath;
        this.StoreID = pc.Storeid;
        this.CategoryID = pc.CategoryID;
        this.SProductID = p.SProductID;
        this.Seq = 0;
    }

	} 
 }