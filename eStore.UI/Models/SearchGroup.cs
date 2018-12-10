using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class SearchGroup
    {
        public SearchGroup() { }
        public SearchGroup(POCOS.Store store, string SiteName, int? CategoryID)
        {
            if (CategoryID.HasValue)
            {
                if (SiteName == "eStore")
                {
                    POCOS.DAL.ProductCategoryHelper estorepchelper = new POCOS.DAL.ProductCategoryHelper();

                    POCOS.ProductCategory rpc = estorepchelper.getProductCategory(CategoryID.Value, store.StoreID, false);
                    if (rpc != null)
                    {
                        this.Id = rpc.CategoryID;
                        this.Source = SiteName;
                        this.Name = rpc.localCategoryNameX;
                        this.MiniSite = rpc.MiniSite;
                    }
                }
                else if (SiteName == "ePAPS")
                {
                    POCOS.DAL.PStoreProductCategoryHelper pstorepchelper = new POCOS.DAL.PStoreProductCategoryHelper();

                    POCOS.PStoreProductCategory rpc = pstorepchelper.get(store, CategoryID.Value);
                    if (rpc != null)
                    {

                        this.Id = rpc.Id;
                        this.Source = SiteName;
                        this.Name = rpc.DisplayName;
                        this.MiniSite = null;
                    }
                }
            }
            if (this.Id == null)
            {
                this.Id = 0;
                this.Name = "Others";
            }
        }
        internal POCOS.MiniSite MiniSite { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int? Id { get; set; }
        public string Source { get; set; }
        public bool IsSelect { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SearchGroup)
                return (SearchGroup)obj == this;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(SearchGroup a, SearchGroup b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            if (a.Id == 0 && b.Id == 0)
                return true;
            else
                // Return true if the fields match:
                return a.Id == b.Id && a.Source == b.Source;
        }

        public static bool operator !=(SearchGroup a, SearchGroup b)
        {
            return !(a == b);
        }
    }
}