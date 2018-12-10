using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Web.UI.WebControls;
using System.Data;
using System.Linq.Expressions;

namespace eStore.POCOS.PocoX
{
    public partial class DashBoardHelperX : POCOS.DAL.Helper
    {
        #region 获取国家/省/source/LOB等信息
        //获取国家
        //public List<string> getRegisterCountry(string storeId)
        //{
        //    try
        //    {
        //        var countryList = (from p in context.Members
        //                           from c in context.Countries
        //                           where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
        //                           && !p.EMAIL_ADDR.Contains("advantech") && !p.EMAIL_ADDR.Contains("@damez.com")
        //                           && p.COUNTRY != null && p.COUNTRY.ToLower() != "null" && p.COUNTRY != ""
        //                           && c.StoreID == storeId
        //                           select p.COUNTRY).Distinct().ToList();
        //        if (countryList != null)
        //            return countryList;
        //        else
        //            return countryList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return new List<string>();
        //    }
        //}

        ////获取source
        //public List<string> getRegisterSource(string storeId)
        //{
        //    try
        //    {
        //        var sourceList = (from p in context.Members
        //                          from c in context.Countries
        //                          where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
        //                          && !p.EMAIL_ADDR.Contains("advantech") && !p.EMAIL_ADDR.Contains("@damez.com")
        //                          && p.SOURCE != null && p.SOURCE.ToLower() != "null" && p.SOURCE != ""
        //                          && c.StoreID == storeId
        //                          select p.SOURCE
        //                            ).Distinct().ToList();
        //        if (sourceList != null)
        //            return sourceList;
        //        else
        //            return sourceList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return new List<string>();
        //    }
        //}

        ////获取lob
        //public List<string> getRegisterLOB(string lob)
        //{
        //    try
        //    {
        //        var sourceList = (from c in context.Members
        //                          where (c.SOURCE != null || c.SOURCE.ToLower() != "null" || c.SOURCE != "")
        //                          && c.SOURCE.Contains(lob)
        //                          select c.SOURCE
        //                            ).Distinct().ToList();
        //        if (sourceList != null)
        //            return sourceList;
        //        else
        //            return sourceList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return new List<string>();
        //    }
        //}

        ////获取Assignee
        //public List<string> getRegisterAssignee()
        //{
        //    try
        //    {
        //        var AssigneeList = (from c in context.TrackingLogs
        //                            where (c.AssignTo != "")
        //                            select c.AssignTo
        //                            ).Distinct().ToList();
        //        if (AssigneeList != null)
        //            return AssigneeList;
        //        else
        //            return AssigneeList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return new List<string>();
        //    }
        //} 
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="showType">store DMF Country</param>
        /// <param name="showTypeItem">showType的子项</param>
        /// <param name="selectByProerty">按照 month country source</param>
        /// <param name="selectByProertyItem">selectByProperty的子项</param>
        /// <param name="groupbyProperty">分组依据</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public List<DashBoardReport> getRegisterByFactor(string showType, string showTypeItem, string groupbyProperty, string startDate, string endDate, string fromFactor=null, string fromFactorItem=null)
        {
            if (string.IsNullOrEmpty(showType) && string.IsNullOrEmpty(showTypeItem) && string.IsNullOrEmpty(groupbyProperty))
                return new List<DashBoardReport>();
            try
            {
                List<DashBoardReport> actual = null;
                DashBoardHelper target = new DashBoardHelper();
                Dictionary<string, object> conditions = new Dictionary<string, object>();
                //if (showType == "Store")//因为没有定义storeid属性,所以在此转换
                //    showType += "ID";
                if (groupbyProperty == "State")
                    groupbyProperty += "X";//因为要显示国家+省份,这里使用stateX属性,防止使用state时出现冲突
                DateTime dtTypeItem;
                DateTime dtFactorItem;
                if (DateTime.TryParse(showTypeItem, out dtTypeItem))
                {
                    conditions.Add(showType, dtTypeItem.Month);
                }
                else
                {
                    conditions.Add(showType, showTypeItem);
                }
                if (!string.IsNullOrEmpty(fromFactor) && !string.IsNullOrEmpty(fromFactorItem))
                {
                    if (DateTime.TryParse(fromFactorItem, out dtFactorItem))
                    {
                        conditions.Add(fromFactor, dtFactorItem.Month);
                        conditions.Add("Year",dtFactorItem.Year);
                    }
                    else
                    {
                        if (fromFactor == "State")//由于state的显示是country+state,所以这边需要拆开来
                        {
                            if (fromFactorItem.IndexOf('/') != -1)
                            {
                                string countryName = fromFactorItem.Substring(0, fromFactorItem.IndexOf('/')).Trim();
                                string StateName = fromFactorItem.Substring(fromFactorItem.IndexOf('/') + 1).Trim();

                                conditions.Add("Country", countryName);
                                conditions.Add(fromFactor, StateName);
                            }
                            else
                            {
                                conditions.Add("Country", fromFactorItem);
                                conditions.Add(fromFactor, "N/A");
                            }
                        }
                        else
                            conditions.Add(fromFactor, fromFactorItem);
                    }
                }
                actual = target.getRegisterDashboard(startDate, endDate, conditions, groupbyProperty);

                if (actual != null)
                    return actual;
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }
    }
}
