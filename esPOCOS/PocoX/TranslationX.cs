using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class Translation { 
 
	 #region Extension Methods 
    
    public void reSetTransGlobalResource(List<TranslationGlobalResource> resLs) 
    {
        if (resLs.Any())
        {
            foreach (var c in resLs)
            {
                var obj = this.TranslationGlobalResources.FirstOrDefault(t => t.LanguageId == c.LanguageId);
                if (obj != null)
                    obj.copyfrom(c);
                else
                    this.TranslationGlobalResources.Add(c);
            }
        }
        foreach (var c in this.TranslationGlobalResources.ToList())
        {
            var obj = resLs.FirstOrDefault(t => c.LanguageId == t.LanguageId);
            if (obj == null)
                this.TranslationGlobalResources.Remove(c);
        }
    }

 #endregion 
	} 
 }