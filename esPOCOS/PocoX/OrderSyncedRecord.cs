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
using eStore.POCOS.DAL;
using System.Linq;

namespace eStore.POCOS
{
    public partial class OrderSyncedRecord
    {
        public bool validateDivision(Order syncOrder, ref string errorMsg)
        {
            bool isValid = true;
            SAPCompanyHelper helper = new SAPCompanyHelper();
            String soldToDivision = getCustomerGroupInfo(helper, syncOrder.Cart.SoldToContact.AddressID);
            String billToDivision = getCustomerGroupInfo(helper, syncOrder.Cart.BillToContact.AddressID);
            String shipToDivision = getCustomerGroupInfo(helper, syncOrder.Cart.ShipToContact.AddressID);
            Dictionary<string, string> errordivision = new Dictionary<string, string>();
            //�κβ�Ϊ���Ҳ�����order.division, ��ʾ������Ϣ��
            if (!string.IsNullOrEmpty(soldToDivision) && this.Division.Equals(soldToDivision) == false)
            {
                isValid = false;
                errordivision.Add("Sold-To", soldToDivision);
            }

            if (!string.IsNullOrEmpty(billToDivision) && this.Division.Equals(billToDivision) == false)
            {
                isValid = false;
                errordivision.Add("Bill-To", billToDivision);
            }

            if (!string.IsNullOrEmpty(shipToDivision) && this.Division.Equals(shipToDivision) == false)
            {
                isValid = false;
                errordivision.Add("Ship-To", shipToDivision);
            }

            if (isValid == false)
            {
                errorMsg = String.Format("Order & Customer division mismatch. Order [{0}] / {1}",
                                                     this.Division,
                                                     string.Join(" / ", errordivision.Select(x => string.Format("{0} [{1}]", x.Key, x.Value)).ToArray()));
            }

            return isValid;
        }

        /// <summary>
        /// This method is to retrieve customer group info of input ERPId
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="ERPId"></param>
        /// <returns></returns>
        private String getCustomerGroupInfo(SAPCompanyHelper helper, String ERPId)
        {
            VSAPCompany sapCompany = helper.getSAPCompanybyID(ERPId);
            if (sapCompany != null)
                return sapCompany.DIVISION;
            else
                return "";
        }
    }
}