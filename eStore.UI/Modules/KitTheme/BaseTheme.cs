using eStore.BusinessModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Modules.KitTheme
{
    public class BaseTheme: Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.KitTheme Theme { get; set; }

        private List<string> _tags;
        private List<string> _subtags;
        private List<string> _baas;
        public List<string> Tags
        {
            get
            {
                if (_tags == null)
                    InitSource();
                return _tags;
            }
        }

        protected List<string> subtags
        {
            get
            {
                if (_subtags == null)
                    InitSource();
                return _subtags;
            }
        }

        protected List<string> baas
        {
            get
            {
                if (_baas == null)
                    InitSource();
                return _baas;
            }
        }

        private void InitSource()
        {
            _tags = new List<string>();
            _subtags = new List<string>();
            _baas = new List<string>();
            foreach (var item in CmsModels)
            {
                foreach (var tag in item.Tag.Split(',').Select(c=>c.TrimStart()).OrderBy(c=>c.Contains("_") ? 0 : 1).ThenBy(c=>c).Take(3))
                {
                    if (tag.StartsWith(Theme.Tags + "_"))
                    {
                        if (!_subtags.Contains(tag.Replace(Theme.Tags + "_", "")))
                            _subtags.Add(tag.Replace(Theme.Tags + "_", ""));
                    }
                }
                foreach (var baa in item.BAANames.Split(',').OrderBy(c => c).Take(3))
                {
                    if (!string.IsNullOrEmpty(baa) && !_baas.Contains(baa))
                        _baas.Add(baa);
                }
            }
            if (_subtags.Any())
                _tags = _subtags;
            else
                _tags = _baas;
        }

        public string Title { get; set; }
        public enum TitleExt
        {
            Featured_Products,
            Industry_Focus,
            News,
            Online_Catalogs,
            Online_Resources,
            Products,
            Success_Stories,
            Videos,
            White_Papers,
            None
        }
        public TitleExt TitleX
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Title))
                {
                    string title = this.Title.Replace(" ", "_");
                    TitleExt x = TitleExt.None;
                    Enum.TryParse(title, out x);
                    return x;
                }
                return BaseTheme.TitleExt.None;
            }
        }
        public List<AdvantechCmsModel> CmsModels { get; set; }

        public string ShowTags(object id)
        {
            string str = "";
            if (id != null)
            {
                string cmsid = id.ToString();
                AdvantechCmsModel item = CmsModels.FirstOrDefault(c => c.CmsID.Equals(cmsid));
                if (item != null)
                {
                    if (subtags.Any())
                    {
                        foreach (var tag in item.Tag.Split(',').Select(c => c.TrimStart()).OrderBy(c => c.Contains("_") ? 0 : 1).ThenBy(c => c).Take(3))
                        {
                            if (tag.StartsWith(Theme.Tags + "_", StringComparison.OrdinalIgnoreCase))
                                str += string.Format("<span>{0}</span>", tag.Replace(Theme.Tags + "_", ""));
                        }
                    }
                    else
                    {
                        foreach (var baa in item.BAANames.Split(',').Where(c => baas.Contains(c)).OrderBy(c => c).Take(3))
                            str += string.Format("<span>{0}</span>", baa);
                    }
                }
            }
            return str;
        }

        public string JoinTags(object id)
        {
            List<string> strs = new List<string>();
            if (id != null)
            {
                string cmsid = id.ToString();
                AdvantechCmsModel item = CmsModels.FirstOrDefault(c => c.CmsID.Equals(cmsid));
                if (item != null)
                {
                    if (subtags.Any())
                    {
                        foreach (var tag in item.Tag.Split(',').Select(c => c.TrimStart()).OrderBy(c => c.Contains("_") ? 0 : 1).ThenBy(c => c).Take(3))
                        {
                            if (tag.StartsWith(Theme.Tags + "_"))
                                strs.Add(tag.Replace(Theme.Tags + "_", ""));
                        }
                    }
                    else
                    {
                        foreach (var baa in item.BAANames.Split(',').Where(c => baas.Contains(c)).OrderBy(c => c).Take(3))
                            strs.Add(baa);
                    }
                }
            }
            return string.Join(",", strs);
        }
    }
}