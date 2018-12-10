using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eStore.POCOS.DAL
{
    public class KitThemeHelper : Helper
    {
        public List<KitTheme> GetAll(string storeid)
        {
            return context.KitThemes.Where(c => c.StoreID.Equals(storeid)).ToList();
        }
        public List<KitTheme> GetKitThemeLs(string storeid)
        {
            return context.KitThemes.Where(c => c.StoreID.Equals(storeid, StringComparison.OrdinalIgnoreCase)
                && c.Publish == true && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now).OrderBy(c => c.Seq).ToList();
        }
        internal int save(KitTheme kitTheme)
        {
            if (kitTheme == null || kitTheme.validate() == false)
                return 1;
            try
            {
                var exitKitTheme = getKitThemeById(kitTheme.Id);
                if (exitKitTheme == null)
                {
                    context.KitThemes.AddObject(kitTheme);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = kitTheme.helper.context;
                    context.KitThemes.ApplyCurrentValues(kitTheme);
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

        public KitTheme getKitThemeById(int id)
        {
            return context.KitThemes.FirstOrDefault(c => c.Id == id);
        }

        internal int delete(KitTheme kitTheme)
        {
            if (kitTheme == null || kitTheme.validate() == false) return 1;
            try
            {

                context = kitTheme.helper.context;
                context.DeleteObject(kitTheme);
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
