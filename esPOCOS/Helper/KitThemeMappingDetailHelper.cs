using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class KitThemeMappingDetailHelper : Helper
    {
        internal int save(KitThemeMappingDetail kitThemeMappingDetail)
        {
            if (kitThemeMappingDetail == null || kitThemeMappingDetail.validate() == false)
                return 1;
            try
            {
                var exitKitThemeMappingDetail = getKitThemeMappingDetailById(kitThemeMappingDetail.Id);
                if (exitKitThemeMappingDetail == null)
                {
                    context.KitThemeMappingDetails.AddObject(kitThemeMappingDetail);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = kitThemeMappingDetail.helper.context;
                    context.KitThemeMappingDetails.ApplyCurrentValues(kitThemeMappingDetail);
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

        public object getKitThemeMappingDetailById(int id)
        {
            var detail = context.KitThemeMappingDetails.FirstOrDefault(c => c.Id == id);
            if(detail != null && detail.helper != null)
                detail.helper.setContext(context);
            return detail;
        }

        internal int delete(KitThemeMappingDetail kitThemeMappingDetail)
        {
            if (kitThemeMappingDetail == null || kitThemeMappingDetail.validate() == false) return 1;
            try
            {
                var _kitThemeMappingDetail = getKitThemeMappingDetailById(kitThemeMappingDetail.Id);
                context.DeleteObject(_kitThemeMappingDetail);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
