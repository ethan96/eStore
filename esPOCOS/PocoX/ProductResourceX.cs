using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class ProductResource { 
 
	 #region Extension Methods 
    public ProductResource()
    {
    }

    public ProductResource(String name, String type, String url)
    {
        this.ResourceName = name;
        this.ResourceType = type;
        this.ResourceURL = url;
    }

    public ProductResource(Part part, String name, String type, String url) : this(name, type, url)
    {
        this.StoreID = part.StoreID;
        this.SProductID = part.SProductID;
    }

    private Advertisement _advertisementX = null;
    public Advertisement advertisementX
    {
        get { return _advertisementX; }
        set { _advertisementX = value; }
    }


    public bool isActivity()
    {
        if (this.ResourceType == "video")
        {
            if (string.IsNullOrEmpty(this.ResourceURL))
                return false;
            int advid = 0;
            if (int.TryParse(this.ResourceURL, out advid))
            {
                Advertisement adv = (new AdvertisementHelper()).getAdvByID(this.StoreID, advid);
                if (adv == null || !adv.isInActivity || !adv.MenuAdvertisements.Any(c => c.TreeID == this.SProductID))
                    return false;
                this.advertisementX = adv;
                return true;
            }
            else
                return false;
        }
        else
            return true;
    }

 #endregion 
	} 
 }