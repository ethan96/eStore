using System;
using System.Collections.Generic;

namespace eStore.BusinessModules
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AdvantechCmsModel
    {
        public string CmsID { get; set; }
        public string Type { get; set; }
        public string ReleaseDate { get; set; }
        public string EndDate { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string ImageUrl { get; set; }
        public string URL { get; set; }
        public string Seq { get; set; }
        public string Tag { get; set; }
        public string Source { get; set; }
        public string CreatedName { get; set; }
        public string CreatedEmail { get; set; }
        public string CreatedDate { get; set; }
        public string ModifyName { get; set; }
        public string ModifyEmail { get; set; }
        public string ModifyDate { get; set; }
        public string Status { get; set; }
        public string BaaIDs { get; set; }
        public string BAANames { get; set; }

        public DateTime CreatedDateX
        {
            get
            {
                DateTime dt = DateTime.Now;
                DateTime.TryParse(ModifyDate, out dt);
                return dt;
            }
        }
        public string CreatedDateShort
        {
            get
            {
                return CreatedDateX.ToShortDateString();
            }
        }


    }
}
