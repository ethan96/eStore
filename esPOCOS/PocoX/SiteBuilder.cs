//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using System.Linq;
using System.Globalization;

namespace eStore.POCOS
{
    public partial class SiteBuilder
    {
        public SiteBuilder() { }
        public SiteBuilder(POCOS.User user) {
            this.UserId = user.UserID;
            this.CreatedDate = DateTime.Now;
        }
     public   enum StoreThemes
        {
            Palette1, Palette2, Palette3, Palette4, Palette5, Palette6
        }
        #region Customer input
        public List<int> SelectedCategories {
            get {
                return getParameters<int>("SelectedCategories");
            }
            set {
                this.setgetParameters<int>("SelectedCategories", value);
            }
        }
        public string FirstName
        {
            get
            {
                return getParameter<string>("FirstName");
            }
            set
            {
                this.Name = value;
                setParameter<string>("FirstName", value);
            }
        }
        public string LastName
        {
            get
            {
                return getParameter<string>("LastName");
            }
            set
            {
                this.Name = value;
                setParameter<string>("LastName", value);
            }
        }
        public string eMail
        {
            get
            {
                return getParameter<string>("eMail");
            }
            set
            {
                this.Name = value;
                setParameter<string>("eMail", value);
            }
        }
        public string CompanyName {
            get {
                return getParameter<string>("CompanyName");
            }
            set {
                this.Name = value;
                setParameter<string>("CompanyName",value);
            }
        }

        public string PhoneNumber 
        {
            get
            {
                return getParameter<string>("PhoneNumber");
            }
            set
            {
                setParameter<string>("PhoneNumber", value);
            }
        }

        public string StoreHours
        {
            get
            {
                return getParameter<string>("StoreHours");
            }
            set
            {
                setParameter<string>("StoreHours", value);
            }
        }

        public bool HasSSLCertificate
        {
            get
            {
                return getParameter<bool>("HasSSLCertificate");
            }
            set
            {
                setParameter<bool>("HasSSLCertificate", value);
            }
        }

        public bool HaseCommerceChat
        {
            get
            {
                return getParameter<bool>("HaseCommerceChat");
            }
            set
            {
                setParameter<bool>("HaseCommerceChat", value);
            }
        }


        public string Logo
        {
            get
            {
                return getParameter<string>("Logo");
            }
            set
            {
                setParameter<string>("Logo", value);
            }
        }


        public string Theme
        {
            get
            {
                return getParameter<string>("Theme");
            }
            set
            {
                setParameter<string>("Theme", value);
            }
        }

        #endregion

        #region System Config


        public string SQLServerNameOrIP
        {
            get
            {
                return getParameter<string>("SQLServerNameOrIP");
            }
            set
            {
                setParameter<string>("SQLServerNameOrIP", value);
            }
        }

        public string SQLServerUserId
        {
            get
            {
                return getParameter<string>("SQLServerUserId");
            }
            set
            {
                setParameter<string>("SQLServerUserId", value);
            }
        }

        public string SQLServerPassword
        {
            get
            {
                return getParameter<string>("SQLServerPassword");
            }
            set
            {
                setParameter<string>("SQLServerPassword", value);
            }
        }

        public string SQLServerDBName
        {
            get
            {
                return getParameter<string>("SQLServerDBName");
            }
            set
            {
                setParameter<string>("SQLServerDBName", value);
            }
        }


        public string SourceSTOREID
        {
            get
            {
                return getParameter<string>("SourceSTOREID");
            }
            set
            {
                setParameter<string>("SourceSTOREID", value);
            }
        }

        public string TargetSTOREID
        {
            get
            {
                return getParameter<string>("TargetSTOREID");
            }
            set
            {
                setParameter<string>("TargetSTOREID", value);
            }
        }

        public string SITENAME
        {
            get
            {
                return getParameter<string>("SITENAME");
            }
            set
            {
                setParameter<string>("SITENAME", value);
            }
        }

        public string domain
        {
            get
            {
                return getParameter<string>("domain");
            }
            set
            {
                setParameter<string>("domain", value);
            }
        }

        public string UserDeptEmail
        {
            get
            {
                return getParameter<string>("UserDeptEmail");
            }
            set
            {
                setParameter<string>("UserDeptEmail", value);
            }
        }
        public Dictionary<string, string> SiteParameters
        {
            get {
                return getSerialParameters<string>("SiteParameters");
            }
            set {
                  setSerialParameters<string>("SiteParameters",value);
            }

        }
        #endregion

        private Dictionary<string, T> getSerialParameters<T>(string group)
        {
            if (this.SiteBuilderParameters == null || this.SiteBuilderParameters.Any() == false)
                return new Dictionary<string, T>();
            return this.SiteBuilderParameters
                .Where(x => x.ParameterType == typeof(T).Name
                &&!string.IsNullOrEmpty(x.ParameterGroup)
                && x.ParameterGroup.Equals(group, StringComparison.InvariantCultureIgnoreCase))
                .ToDictionary(k => k.ParameterKey, x => (T)Convert.ChangeType(x.ParameterValue, typeof(T), CultureInfo.InvariantCulture));
        }
        private void setSerialParameters<T>(string group, Dictionary<string, T> paras)
        {
            if (string.IsNullOrEmpty(group) || paras == null || paras.Any() == false)
                return;
            var exists = getSerialParameters<T>(group);
            foreach (var p in exists.Where(x => paras.ContainsKey(x.Key) == false))
            {
                deleteParameter<T>(p.Key, p.Value);
            }

            foreach (var para in paras)
            {
                setParameter(para.Key, para.Value, group);
            }
        }
        private List<T> getParameters<T>(string key)
        {
            if (this.SiteBuilderParameters == null || this.SiteBuilderParameters.Any() == false)
                return new List<T>();
            return this.SiteBuilderParameters
                .Where(x => x.ParameterType == typeof(T).Name
                && x.ParameterKey.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => (T)Convert.ChangeType(x.ParameterValue, typeof(T), CultureInfo.InvariantCulture)).ToList();

        }
        private void setgetParameters<T>(string key, List<T> paras)
        {
            List<T> exists = new List<T>();
            if (this.SiteBuilderParameters != null && this.SiteBuilderParameters.Any())
            {
                exists = getParameters<T>(key);
                foreach (T value in exists.Where(x => paras.Contains(x) == false))
                {
                    deleteParameter<T>(key, value);
                }

            }
            foreach (var para in paras.Where(x => exists.Contains(x) == false))
            {
                var _para = new POCOS.SiteBuilderParameter()
                {
                    ParameterKey = key,
                    ParameterType = typeof(T).Name
                          ,
                    ParameterValue = para.ToString()
                };
                this.SiteBuilderParameters.Add(_para);
            }
        }
        private void deleteParameter<T>(string key, T value) 
        {
            if (this.SiteBuilderParameters != null && this.SiteBuilderParameters.Any())
            {
                var para = this.SiteBuilderParameters
                   .FirstOrDefault(x => x.ParameterType == typeof(T).Name
                   && x.ParameterKey.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                   && x.ParameterValue == value.ToString()
                   );
                if (para != null)
                {
                    if (para.Id > 0)
                    {
                        this.helper.getContext().SiteBuilderParameters.DeleteObject(para);
                       
                    }
                    else
                    {
                        this.SiteBuilderParameters.Remove(para);
                    }
                }
                  
            }
        }
        private T getParameter<T>(string key)
        {
            if (this.SiteBuilderParameters == null || this.SiteBuilderParameters.Any() == false)
                return default(T);
            return getParameters<T>(key).FirstOrDefault();
        }
        private void setParameter<T>(string key, T value,string group="")
        {
            if (value == null)
                return;
            if (this.SiteBuilderParameters == null)
                this.SiteBuilderParameters = new List<SiteBuilderParameter>();

            var para = this.SiteBuilderParameters
                .Where(x => x.ParameterType == typeof(T).Name
                && x.ParameterKey.Equals(key, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (para == null)
            {
                para = new POCOS.SiteBuilderParameter()
                {
                    ParameterKey = key,
                    ParameterType = typeof(T).Name
                };
                if (!string.IsNullOrEmpty(group))
                    para.ParameterGroup = group;
                this.SiteBuilderParameters.Add(para);
            }
            para.ParameterValue = value.ToString();
        }
    }
}