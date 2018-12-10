using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class SiteBuilderHelper : Helper
    {
        public List<SiteBuilder> getSiteBuilders()
        {
            return context.SiteBuilders.ToList();
        }
        public SiteBuilder getSiteBuilder(string UserId)
        {
            SiteBuilder siteBuilder = context.SiteBuilders.FirstOrDefault(x => x.UserId == UserId);
            if (siteBuilder != null)
                siteBuilder.helper = this;
            return siteBuilder;
        }
        public SiteBuilder getSiteBuilder(int Id)
        {
            SiteBuilder siteBuilder= context.SiteBuilders.FirstOrDefault(x => x.Id == Id);
            if (siteBuilder != null)
                siteBuilder.helper = this;
            return siteBuilder;
        }
        public int save(SiteBuilder siteBuilder)
        {
            if (siteBuilder == null || siteBuilder.validate() == false)
                return 1;
            try
            {
                var exitSiteBuilder = context.SiteBuilders.FirstOrDefault(x => x.UserId == siteBuilder.UserId);
                if (exitSiteBuilder == null)
                {
                    context.SiteBuilders.AddObject(siteBuilder);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = siteBuilder.helper.context;
                    context.SiteBuilders.ApplyCurrentValues(siteBuilder);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(SiteBuilder siteBuilder)
        {
            if (siteBuilder == null || siteBuilder.validate() == false)
                return 1;
            try
            {
                var exitSiteBuilder = context.SiteBuilders.FirstOrDefault(x => x.UserId == siteBuilder.UserId);
                if (exitSiteBuilder == null)
                {
                    return 0;

                }
                else
                {
                    siteBuilder.Status = "Deleted";
                    context = siteBuilder.helper.context;
                    context.SiteBuilders.ApplyCurrentValues(siteBuilder);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }

        }
    }
}
