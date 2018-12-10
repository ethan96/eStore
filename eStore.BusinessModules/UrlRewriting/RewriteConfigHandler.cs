using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web;

namespace eStore.BusinessModules.UrlRewrite
{
    public class RewriteConfigHandler: IConfigurationSectionHandler
    {
        XmlNode _Section;
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            _Section = section;
            return this;
        }

        /// <summary>
        ///  Get whether url mapping is enabled in the app.config
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool Enabled()
        {
            if (_Section.Attributes.GetNamedItem("enabled").Value  == "true")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Get the matching "mapped Url" from the web.config file if there is one.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string MappedUrl(string url)
	{
 
		Regex oReg ;
        foreach (XmlNode x in _Section.ChildNodes)
        {
			try {
				string pattern = x.Attributes.GetNamedItem("url").Value.ToLower();
				string replaceTarget = x.Attributes.GetNamedItem("mappedUrl").Value.ToLower();
				oReg = new Regex(pattern);
				if (oReg.Match(url).Success) {
					return oReg.Replace(url, replaceTarget);
				}

			} catch (Exception) {
				return "";
			}
		}
		return "";

	}
        string safeURLText(string input, int length)
        {
            string output = input;
            try
            {
               
                Regex regRemoveUnwords = new Regex(@"[\W\s<>*:&\\]+");
                Regex regRemoveHtmlTags = new Regex(@"</?[a-z][a-z0-9]*[^<>]*>");
                char[] trimChars = { ' ', '-' };
                output = regRemoveHtmlTags.Replace(input, "");
                output = esUtilities.CommonHelper.SubString(regRemoveUnwords.Replace(output, "-").Trim(trimChars), length, false).Trim();

            }
            catch (ArgumentException)
            { 
            
            }
            return output;
                  
        }
        internal string CreateMappedUrl(string key, string KeyWrod = "", string Name = "", string ID1 = "", string ID2 = "")
        {
            XmlNode x = _Section.SelectSingleNode(string.Format("add[@key='{0}']",key));
            if (x != null)
            {
                try
                {

                    KeyWrod = safeURLText(KeyWrod,500);
                    //if (key != "system")//do not replace special char for system name
                        Name = safeURLText(Name, 500);                }
                catch (ArgumentException)
                {
                    // Syntax error in the regular expression
                }
                if (string.IsNullOrWhiteSpace(KeyWrod) && string.IsNullOrWhiteSpace(Name))
                {
                    KeyWrod = "products";
                    Name = "eppro";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Name))
                        Name = "product";
                    if (string.IsNullOrWhiteSpace(KeyWrod))
                        KeyWrod = Name;

                }
               
                string url = x.Attributes.GetNamedItem("url").Value.ToLower();
                bool ismodified = false;
                int index = -1;
                if (!string.IsNullOrEmpty(KeyWrod))
                {
                    index = url.IndexOf(@"(.*)\");
                    if (index >= 0)
                    {
                        url = url.Remove(index, 5);
                        url = url.Insert(index, HttpUtility.UrlEncode(KeyWrod));
                        ismodified = true;
                    }
                }

                if (!string.IsNullOrEmpty(Name))
                {
                    index = url.IndexOf(@"(.*)\");
                    if (index >= 0)
                    {
                        url = url.Remove(index, 5);
                        url = url.Insert(index, HttpUtility.UrlEncode(Name));
                        ismodified = true;
                    }
                }
                if (!string.IsNullOrEmpty(ID1))
                {
                    index = url.IndexOf(@"(.*)\");
                    if (index >= 0)
                    {
                        url = url.Remove(index, 5);
                        url = url.Insert(index, ID1);
                        ismodified = true;
                    }
                }
                if (!string.IsNullOrEmpty(ID2))
                {
                    index = url.IndexOf(@"(.*)\");
                    if (index >= 0)
                    {
                        url = url.Remove(index, 5);
                        url = url.Insert(index, ID2);
                        ismodified = true;
                    }
                    else
                    {
                        index = url.IndexOf(@"(.*)");
                        if (index >= 0)
                        {
                            url = url.Remove(index, 4);
                            url = url.Insert(index, ID2);
                            ismodified = true;
                        }
                    }
                }
                if (ismodified)
                {
                    url = url.Replace(@"\", @"");
                    return url;
                }
                else
                    return string.Empty;
            }

            return string.Empty;
        }
    }
}
