using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Reflection;

namespace eStore.UI.Modules
{
    public partial class ProductSpec : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public List<POCOS.VProductMatrix> VProductMatrixList { get; set; }
        protected override void OnPreRender(EventArgs e)
        {
            this.Visible = false;
            //BindData();
            //BindTranslationFonts();
            base.OnPreRender(e);
        }

        /// <summary>
        /// Bind Translation Fonts
        /// </summary>
        private void BindTranslationFonts()
        {
            btnFilter.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Filter);
        }

        private void BindData()
        {
            if (VProductMatrixList != null && VProductMatrixList.Count() > 0)
            {

                var Cat = from cat in VProductMatrixList
                          group cat by
                          new { cat.CatID, cat.LocalCatName }
                              into catgroup
                              select new
                              {
                                  id = catgroup.Key.CatID,
                                  key = "c_" + catgroup.Key.CatID,
                                  display = catgroup.Key.LocalCatName
                              };
                rpSpecFilter.DataSource = Cat;
                rpSpecFilter.DataBind();

                var atributeValues = from attrvalue in VProductMatrixList
                                     where attrvalue.selected == true
                                     select new
                                     {
                                         key = string.Format("c_{0}_a_{1}_v_{2}", attrvalue.CatID, attrvalue.AttrID, attrvalue.AttrValueID),
                                         display = string.Format("{0} : {1}", attrvalue.LocalAttributeName, attrvalue.LocalValueName)
                                     };
                rpseletedfilter.DataSource = atributeValues;
                rpseletedfilter.DataBind();
            }
        }
        protected void rpSpecFilter_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rpAttributes = (Repeater)e.Item.FindControl("rpAttributes");
                int catid = 0;
                var di = e.Item.DataItem;

                Type type = di.GetType();
                PropertyInfo pi = type.GetProperty("id");
                catid = (int)pi.GetValue(di, null);

                var attibute = from attr in VProductMatrixList
                               where attr.CatID == catid
                               group attr by
                               new { attr.AttrID, attr.LocalAttributeName }
                                   into attrgroup
                                   select new
                                   {
                                       cid = catid,
                                       id = attrgroup.Key.AttrID,
                                       key = string.Format("c_{0}_a_{1}", catid, attrgroup.Key.AttrID),
                                       display = string.Format("{0}", attrgroup.Key.LocalAttributeName)
                                   };
                rpAttributes.DataSource = attibute;
                rpAttributes.DataBind();
            }
        }
        protected void rpAttributes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rpAttributeValues = (Repeater)e.Item.FindControl("rpAttributeValues");
                int catid = 0;
                int attrid = 0;
                var di = e.Item.DataItem;

                Type type = di.GetType();
                PropertyInfo pi = type.GetProperty("cid");
                catid = (int)pi.GetValue(di, null);
                pi = type.GetProperty("id");
                attrid = (int)pi.GetValue(di, null);
                var atributeValues = from attrvalue in VProductMatrixList
                                     where attrvalue.CatID == catid && attrvalue.AttrID == attrid
                                     select new
                                     {
                                         key = string.Format("c_{0}_a_{1}_v_{2}", catid, attrid, attrvalue.AttrValueID),
                                         display = string.Format(" {0} ({1})", attrvalue.LocalValueName, attrvalue.productcount),
                                         ischecked = attrvalue.selected ? " checked=\"checked\" " : ""
                                     };
                rpAttributeValues.DataSource = atributeValues;
                rpAttributeValues.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {

        }

        public string keyword
        {
            get
            {
                if (!string.IsNullOrEmpty(Request["speckeyword"]))
                {
                    return Request["speckeyword"];
                }
                else
                    return string.Empty;
            }
        }
        public List<POCOS.VProductMatrix> getSelectedSpec()
        {
            string filter = Request["specfilter"];
            List<POCOS.VProductMatrix> vpms = new List<POCOS.VProductMatrix>();

            if (!string.IsNullOrEmpty(filter))
            {
                POCOS.VProductMatrix vpm;
                foreach (string sf in filter.Split(','))
                {
                    try
                    {
                        Match match = Regex.Match(sf, @"c_(\d+)_a_(\d+)_v_(\d+)");
                        if (match.Success)
                        {
                            vpm = new POCOS.VProductMatrix();
                            vpm.CatID = int.Parse(match.Groups[1].Value);
                            vpm.AttrID = int.Parse(match.Groups[2].Value);
                            vpm.AttrValueID = int.Parse(match.Groups[3].Value);
                            vpms.Add(vpm);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // Syntax error in the regular expression
                    }
                }


            }
            return vpms;
        }
    }
}