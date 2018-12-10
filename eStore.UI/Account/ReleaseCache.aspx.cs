using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime;

namespace eStore.UI.Account
{
    public partial class ReleaseCache : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // this.lbReleaseCache.DataSource = HttpContext.Current.Cache.;
                //this.lbReleaseCache.DataBind();
            }
            if (Presentation.eStoreContext.Current.User != null)
            {
                if (!Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.SwitchRole))
                {
                    Response.Redirect("~/");
                }
                bind();
            }
            else
            { Response.Redirect("~/"); }

        }

        private void bind()
        {
            BusinessModules.Store store = null;
            if (!string.IsNullOrEmpty(Request["s"]))
            {
                if (Request["s"].ToString().Equals("all"))
                {
                    this.gvReleaseCache.DataSource = POCOS.CachePool.getInstance().getCacheList();
                    this.gvReleaseCache.DataBind();
                    return;
                }
                else
                store = BusinessModules.StoreSolution.getInstance().getStore(Request["s"]);
            }
            if (store == null)
                store = Presentation.eStoreContext.Current.Store;

            this.gvReleaseCache.DataSource = store.getCacheItemList();
            this.gvReleaseCache.DataBind();
        }

 

        protected void btnReleaseCache_Click(object sender, EventArgs e)
        {

            BusinessModules.Store store = null;
            if (!string.IsNullOrEmpty(Request["s"]))
            {
                if (Request["s"].ToString().Equals("all"))
                {
                    POCOS.CachePool.getInstance().reset();
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                }
                else if (Request["s"].ToString().Equals("gc"))
                {
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                }
                else
                    store = BusinessModules.StoreSolution.getInstance().getStore(Request["s"]);
            }
            if (store == null)
                store = Presentation.eStoreContext.Current.Store;

            store.releaseEntireCache();
            this.gvReleaseCache.DataSource = store.getCacheItemList();
            this.gvReleaseCache.DataBind();
        }

        protected void btnRemoveSelectedCache_Click(object sender, EventArgs e)
        {
            if (Request["selectedCacheItem"] != null)
            {
                List<string> caches = Request["selectedCacheItem"].Split(',').ToList();

                //trim padding space
                for (int i = 0; i < caches.Count(); i++)
                    caches[i] = caches[i].Trim();

                Presentation.eStoreContext.Current.Store.releaseCacheItems(caches);
                this.gvReleaseCache.DataSource = Presentation.eStoreContext.Current.Store.getCacheItemList();
                this.gvReleaseCache.DataBind();
            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("PleaseSelectItems");
            }
        }
    }
}