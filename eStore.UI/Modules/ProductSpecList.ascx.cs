using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using System.Web.UI.HtmlControls;

namespace eStore.UI.Modules
{
    public partial class ProductSpecList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public IList<VProductMatrix> vProductMatrixs { get; set; }
        private bool _ShowCategory = true;
        public bool ShowCategory { get { return _ShowCategory; } set { _ShowCategory = value; } }

        private bool _ShowAttribute = true;
        public bool ShowAttribute { get { return _ShowAttribute; } set { _ShowAttribute = value; } } 
 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (vProductMatrixs == null)
                return;
            var pms = (from pm in vProductMatrixs
                       where pm.CatID!=999999
                       group pm by
                           new { pm.CatID, pm.LocalCatName }
                           into category
                           select new
                           {
                               categoryName = category.Key.LocalCatName,
                               count=category.Count(),
                               attributes=(from attr in category
                                          group attr by 
                                          new {attr.AttrID ,attr.LocalAttributeName}
                                          into attrgroup
                                          select  new
                                          {
                                          attributeName=attrgroup.Key.LocalAttributeName ,
                                          count = attrgroup.Count(),
                                          values=attrgroup
                                          })            
                           });

            this.tblProductSpecList.Rows.Clear();
            HtmlTableRow headerrow = new HtmlTableRow();
            if (ShowCategory)
                this.AddHeaderCell(headerrow, "Category");
            if (ShowAttribute)
                this.AddHeaderCell(headerrow, "Attribute");
            this.AddHeaderCell(headerrow, "Value");
            this.tblProductSpecList.Rows.Add(headerrow);
            foreach (var cat in pms)
            {
                HtmlTableRow catrow = new HtmlTableRow();
                if (ShowCategory)
                {
                    HtmlTableCell catcell = AddCell(catrow, cat.categoryName, cat.count);
                }
                for(int attrIndex=0;attrIndex<cat.attributes.Count();attrIndex++)
               
                {
                    var attr = cat.attributes.ElementAt(attrIndex);
                    if (attrIndex==0)
                    {
                        if (ShowAttribute)
                            AddCell(catrow, attr.attributeName, attr.count);
                        foreach (var val in attr.values)
                        {
                            if (val.Equals(attr.values.First()))
                            {
                                AddCell(catrow, val.LocalValueName);
                                tblProductSpecList.Rows.Add(catrow);
                            }
                            else
                            {
                                HtmlTableRow valrow = new HtmlTableRow();
                                AddCell(valrow, val.LocalValueName);
                                tblProductSpecList.Rows.Add(valrow);
                            }
                        }
                    }
                    else
                    {
                       
                        foreach (var val in attr.values)
                        {
                            if (val.Equals(attr.values.First()))
                            {
                                HtmlTableRow attrrow = new HtmlTableRow();
                                AddCell(attrrow, attr.attributeName, attr.count);
                                AddCell(attrrow, val.LocalValueName);
                                tblProductSpecList.Rows.Add(attrrow);
                            }
                            else
                            {
                                HtmlTableRow valrow = new HtmlTableRow();
                                AddCell(valrow, val.LocalValueName);
                                tblProductSpecList.Rows.Add(valrow);
                            }
                        }
                    }
                }
            
            }
        }


        private HtmlTableCell AddHeaderCell(HtmlTableRow row, string text, int rowspan = 1, int colspan = 1)
        {
            var cell = new HtmlTableCell("th");
            cell.InnerHtml = text;
            if (rowspan > 1)
                cell.RowSpan = rowspan;
            if (colspan > 1)
                cell.ColSpan = colspan;
            row.Cells.Add(cell);
            return cell;
        }

        private HtmlTableCell AddCell(HtmlTableRow row, string text, int rowspan = 1, int colspan = 1)
        {
            var cell = new HtmlTableCell();
            cell.InnerHtml = text;
            if (rowspan > 1)
                cell.RowSpan = rowspan;
            if (colspan > 1)
                cell.ColSpan = colspan;
            row.Cells.Add(cell);
            return cell;
             
        }
    }
}