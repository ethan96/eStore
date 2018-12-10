using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.Presentation
{
    public class IDKProductUI
    {
        public class IDKUI
        {
            private string _name;
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private string _nameX;
            public string NameX
            {
                get
                {
                    if (string.IsNullOrEmpty(_nameX))
                        _nameX = Name.Replace(" ", "_");
                    return _nameX;
                }
                set { _nameX = value; }
            }

            private List<IDKAttribute> _value = new List<IDKAttribute>();
            public List<IDKAttribute> Value
            {
                get { return _value; }
                set { _value = value; }
            }
        }

        public class IDKUIHelper
        {
            /// <summary>
            /// 把IDKAttribute 转换为前台需要的格式
            /// 进一步转为JSON数据
            /// </summary>
            /// <param name="ls"></param>
            /// <returns></returns>
            private List<IDKUI> setIDKAttributeToIDKUI(List<IDKAttribute> ls)
            {
                List<IDKUI> list = new List<IDKUI>();
                foreach (var item in ls)
                {
                    var cc = list.FirstOrDefault(c => c.Name == item.AttributeName);
                    if (cc != null)
                        cc.Value.Add(item);
                    else
                    {
                        IDKUI idk = new IDKUI();
                        idk.Name = item.AttributeName;
                        idk.Value.Add(item);
                        list.Add(idk);
                    }
                }
                return list;
            }

            public string getAttributeDDLDiv(string sproductid)
            {
                return string.Empty;
            }

            /// <summary>
            /// 拼装product.aspx页面中 Product Addon的信息
            /// </summary>
            /// <param name="sproductID"></param>
            /// <returns></returns>
            public string getAddonList(string sproductID)
            {

                if (string.IsNullOrEmpty(sproductID))
                    return string.Empty;
                POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(sproductID, true);
                if (product == null)
                    return string.Empty;

                StringBuilder sb = new StringBuilder();
                
                if (product.PeripheralAddOns.Any())
                {
                    var addonList = string.Join("|", product.PeripheralAddOns.Select<PeripheralAddOn,string>(c => c.AddOnItemID.ToString()).ToList());
                    int i = 0;
                    sb.Append("<table class='estoretable fontbold hiddenitem' width='100%' id='productaddons' style='display: table; '>");
                    sb.Append("<thead><tr>");
                    sb.Append(string.Format("<th class='tablecolwidth145'>{0}</th>",eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)));
                    sb.Append(string.Format("<th>{0}</th>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)));
                    sb.Append(string.Format("<th class='tablecolwidth75'>{0}</th>",eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)));
                    sb.Append(string.Format("<th class='tablecolwidth45'>{0}</th>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)));
                    sb.Append("</tr></thead><tbody>");

                    var parts = string.Join(",", (from addon in product.PeripheralAddOns
                                                  select string.Join(",", (from item in addon.PeripheralAddOnBundleItems
                                                                           select item.SProductID).ToArray())).ToArray()); ;

                    eStore.Presentation.eStoreContext.Current.Store.getPartList(parts);
                    foreach (var p in product.PeripheralAddOns)
                    {
                        if (p.addOnProduct is Product_Bundle)
                        {
                            var pp = p.addOnProduct as Product_Bundle;
                            sb.Append(string.Format("<tr id='mainSTR{0}' class='mainSTR hiddenitem'>", p.AddOnItemID));
                            sb.Append(string.Format("<td style='padding-left:15px;' class='tablecolwidth145 coloptionimg' id='STR{0}' onclick='checkTRShowOrHide(this.id)'><span id='{1}' name='{1}' class='jTipProductDetail'>{1}</span>", i, p.addOnProduct.name));
                            sb.Append(string.Format("<input type='hidden' name='inputAddonName_{0}' id='inputAddonName_{0}' value='{0}'></td>", p.AddOnItemID));
                            sb.Append(string.Format("<td class='left'>{0} <a class='mousehand' onclick='checkTRShowOrHide(\"STR{1}\")'> &nbsp;See Detail</a></td>", p.addOnProduct.productDescX, i));
                            sb.Append(string.Format("<td class='tablecolwidth75 colorRed right'>{0}</td>", eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)p.addOnProduct.getListingPrice().value)));
                            sb.Append(string.Format("<td class='tablecolwidth45'><input name='inputAddonQTY_{0}' type='text' id='inputAddonQTY_{0}' class='qtytextbox'  warrantyprice='{1}'></td>"
                                , p.AddOnItemID
                                , pp.bundle.getWarrantableTotal()
                                ));
                            sb.Append("</tr>");

                            if (pp.bundle != null && pp.bundle.BundleItems.Any())
                            {

                                foreach (var b in pp.bundle.BundleItems)
                                {
                                    sb.Append(string.Format("<tr class='STR{0} subTR hiddenitem' style='display: none; '>", i));
                                    sb.Append(string.Format("<td class='' style='padding-left:25px;'>{0}</td>", b.ItemSProductID));
                                    sb.Append(string.Format("<td style='padding-left:25px;'>{0}</td>", b.ItemDescription));
                                    sb.Append("<td class=' colorRed right'></td>");
                                    sb.Append("<td class=''></td>");
                                    sb.Append("</tr>");

                                }
                            }
                        }
                        i++;
                    }
                    sb.Append("</tbody></table>");
                    sb.Append(string.Format("<input type=\"hidden\" name=\"hdProductAddonQtyList\" id=\"hdProductAddonQtyList\" value=\"{0}\" />", addonList));                    
                    sb.Append("<script type='text/javascript' language='javascript'>$(document).ready(JT_init);checkEstoreInputQTY();</script>");
                }
                return sb.ToString();
            }
        }
    }
}
