using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class KitThemeTypeHelper : Helper
    {
        internal int save(KitThemeType kitThemeType)
        {
            if (kitThemeType == null || kitThemeType.validate() == false)
                return 1;
            try
            {
                var exitAddress = getKitThemeTypeById(kitThemeType.Id);
                if (exitAddress == null)
                {
                    context.KitThemeTypes.AddObject(kitThemeType);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = kitThemeType.helper.context;
                    context.KitThemeTypes.ApplyCurrentValues(kitThemeType);
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

        public KitThemeType getKitThemeTypeById(int id)
        {
            return context.KitThemeTypes.FirstOrDefault(c => c.Id == id);
        }

        public List<KitThemeType> getKitThemeByThemeId(int themeID)
        {
            return context.KitThemeTypes.Where(k => k.ThemeId == themeID).ToList();
        }

        internal int delete(KitThemeType kitThemeType)
        {
            if (kitThemeType == null || kitThemeType.validate() == false) return 1;
            try
            {
                var _kitThemeType = getKitThemeTypeById(kitThemeType.Id);
                if (_kitThemeType != null)
                {
                    context.DeleteObject(_kitThemeType);
                    context.SaveChanges();
                    return 0;
                }
                return -5000;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
