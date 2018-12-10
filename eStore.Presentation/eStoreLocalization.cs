using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.BusinessModules;

namespace eStore.Presentation
{
    [Serializable]
    public class eStoreLocalization
    {
        public static string Date(object date)
        {
            if (date is DateTime)
                return Date((DateTime)date);
            else
                return "";
        }
        public static string Date(DateTime date)
        {
            String targetCulture;
            TimeSpan targetOffset = new TimeSpan();
            string rlt;
            if (eStoreContext.Current.User != null && eStoreContext.Current.User.UserLanguages != null)
            {
                if (eStoreContext.Current.User.actingUser.UserLanguages.Length > 0)
                    targetCulture = eStoreContext.Current.User.actingUser.UserLanguages[0];
                else
                    targetCulture = eStoreContext.Current.Store.profile.CultureCode;
                targetOffset = eStoreContext.Current.User.actingUser.timeSpan;
            }
            else
            {
                targetCulture = eStoreContext.Current.Store.profile.CultureCode;
            }
            if (targetOffset.Hours == 0 && targetOffset.Minutes == 0)
                rlt = eStoreContext.Current.Store.convertTimeAndFormat(date, targetCulture, true);
            else
                rlt = eStoreContext.Current.Store.convertTimeAndFormat(date, targetCulture, targetOffset, true);
            return rlt;
        }

        public static string DateTime(string date)
        {
            return "";
        }
        public static string DateTime(DateTime date)
        {
            String targetCulture;
            TimeSpan targetOffset = new TimeSpan();
            string rlt;
            if (eStoreContext.Current.User != null && eStoreContext.Current.User.UserLanguages!=null)
            {
                if (eStoreContext.Current.User.UserLanguages.Length > 0)
                    targetCulture = eStoreContext.Current.User.UserLanguages[0];
                else
                    targetCulture = eStoreContext.Current.Store.profile.CultureCode;
                targetOffset = eStoreContext.Current.User.actingUser.timeSpan;
            }
            else
            {
                targetCulture = eStoreContext.Current.Store.profile.CultureCode;
            }
            if (targetOffset.Hours == 0 && targetOffset.Minutes == 0)
                rlt = eStoreContext.Current.Store.convertTimeAndFormat(date, targetCulture);
            else
                rlt = eStoreContext.Current.Store.convertTimeAndFormat(date, targetCulture, targetOffset);
            return rlt;
        }

        public static string Tanslation(POCOS.Store.TranslationKey key,Boolean returnUSValueIfNull=true)
        {
            return eStoreContext.Current.Store.Tanslation(key, returnUSValueIfNull, eStoreContext.Current.CurrentLanguage, eStoreContext.Current.MiniSite);
        }
 

        public static string Tanslation(string key,Boolean returnUSValueIfNull=true)
        {
            return eStoreContext.Current.Store.Tanslation(key, returnUSValueIfNull, eStoreContext.Current.CurrentLanguage, eStoreContext.Current.MiniSite);
        }
    }
}
