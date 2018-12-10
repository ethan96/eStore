using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Linq.Expressions;

namespace eStore.POCOS.DAL
{
    public partial class DashBoardHelper : Helper
    {
        #region Search DaskBoard
        /// <summary>
        /// For OM use, return Order and Quotation total amount and count by given date range
        /// use in Report/DashBoard
        /// </summary>
        /// <returns></returns>
        public List<VDashboard> getDashboard(DateTime startdate, DateTime enddate, Decimal currencyToUSDRate = 1)
        {
            if (startdate == null && enddate == null)
                return new List<VDashboard>();
            try
            {
                enddate = enddate.AddHours(24);
                var _dash = from d in context.sp_getDashBoard(startdate, enddate)
                            select new VDashboard
                            {
                                Type = d.Type,
                                StoreID = d.StoreID,
                                Cnt = d.Cnt,
                                YYYYM = d.YYYYM,
                                TotalAmount = d.TotalAmount,
                                LocalAmount = d.LocalAmount / currencyToUSDRate,
                            };

                if (_dash != null)
                    return _dash.ToList();
                else
                    return new List<VDashboard>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<VDashboard>();
            }

        }

        /// <summary>
        /// For OM use,Return Order and Quotation Detail Report
        /// use in Report/Performance/DashBoard
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="showType"></param>
        /// <returns></returns>
        public List<DashBoardReport> getDashboardDetail(DateTime startdate, DateTime enddate, string showType, Decimal currencyToUSDRate = 1)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> dashBoardPerformance = new List<DashBoardReport>();
                if (showType == "Order")
                {
                    var dashOrderCountryList = (from a in context.Orders
                                                join b in context.Carts on a.CartID equals b.CartID
                                                join c in context.CartContacts on b.BilltoID equals c.ContactID
                                                join d in context.Countries on c.Country equals d.CountryName
                                                from e in context.vStoreExchangeRates
                                                from f in context.Currencies.GroupBy(t => new { t.CurrencyID, t.ToUSDRate })
                                                where POCOS.Order.SuccessfullyStatus.Contains(a.OrderStatus) && !a.UserID.Contains("@advantech.")
                                                && !a.UserID.Contains("@damez.com") && a.StoreID == e.StoreID
                                                && a.OrderDate >= startdate && a.OrderDate < enddate && a.TotalAmount != null && f.Key.CurrencyID == b.Currency
                                                orderby a.OrderDate
                                                select new
                                                {
                                                    a.StoreID,
                                                    a.Source,
                                                    a.OrderDate.Value,
                                                    UserID = "",
                                                    LocalAmount = Math.Round((a.TotalAmount.Value * f.Key.ToUSDRate.Value) / currencyToUSDRate, 2),
                                                    d.CountryName,
                                                    d.DMF

                                                }).ToList();

                    if (dashOrderCountryList.Count() > 0)
                    {
                        foreach (var item in dashOrderCountryList)
                        {
                            DashBoardReport dashPerformance = new DashBoardReport();
                            dashPerformance.StoreID = item.StoreID;
                            dashPerformance.Source = item.Source;
                            dashPerformance.YYYYM = item.Value.ToString();
                            dashPerformance.UserID = item.UserID;
                            dashPerformance.LocalAmount = item.LocalAmount;
                            dashPerformance.CountryName = item.CountryName;
                            dashPerformance.DMF = item.DMF;
                            dashBoardPerformance.Add(dashPerformance);
                        }
                    }

                    var dashOrderShortList = (from a in context.Orders
                                              join b in context.Carts on a.CartID equals b.CartID
                                              join c in context.CartContacts on b.BilltoID equals c.ContactID
                                              join d in context.Countries on c.Country equals d.Shorts
                                              from e in context.vStoreExchangeRates
                                              from f in context.Currencies.GroupBy(t => new { t.CurrencyID, t.ToUSDRate })
                                              where POCOS.Order.SuccessfullyStatus.Contains(a.OrderStatus) && !a.UserID.Contains("@advantech.")
                                              && !a.UserID.Contains("@damez.com") && a.StoreID == e.StoreID
                                              && a.OrderDate >= startdate && a.OrderDate <= enddate && a.TotalAmount != null && f.Key.CurrencyID == b.Currency
                                              orderby a.OrderDate
                                              select new
                                              {
                                                  a.StoreID,
                                                  a.Source,
                                                  a.OrderDate.Value,
                                                  UserID = "",
                                                  LocalAmount = Math.Round((a.TotalAmount.Value * f.Key.ToUSDRate.Value) / currencyToUSDRate, 2),
                                                  d.CountryName,
                                                  d.DMF

                                              }).ToList();

                    if (dashOrderShortList.Count() > 0)
                    {
                        foreach (var item in dashOrderShortList)
                        {
                            DashBoardReport dashPerformance = new DashBoardReport();
                            dashPerformance.StoreID = item.StoreID;
                            dashPerformance.Source = item.Source;
                            dashPerformance.YYYYM = item.Value.ToString();
                            dashPerformance.UserID = item.UserID;
                            dashPerformance.LocalAmount = item.LocalAmount;
                            dashPerformance.CountryName = item.CountryName;
                            dashPerformance.DMF = item.DMF;
                            dashBoardPerformance.Add(dashPerformance);
                        }
                    }

                }
                else
                {
                    var dashQuotationCountryList = (from a in context.Quotations
                                                    join b in context.Carts on a.CartID equals b.CartID
                                                    join c in context.CartContacts on b.BilltoID equals c.ContactID
                                                    join d in context.Countries on c.Country equals d.CountryName
                                                    from e in context.vStoreExchangeRates
                                                    from f in context.Currencies.GroupBy(t => new { t.CurrencyID, t.ToUSDRate })
                                                    where POCOS.Quotation.SuccessfullyStatus.Contains(a.Status) && a.UserID != null && a.StoreID == e.StoreID
                                                    && a.QuoteDate >= startdate && a.QuoteDate <= enddate && a.TotalAmount != null && f.Key.CurrencyID == b.Currency
                                                    orderby a.QuoteDate
                                                    select new
                                                    {
                                                        a.StoreID,
                                                        Source = "",
                                                        a.QuoteDate.Value,
                                                        a.UserID,
                                                        LocalAmount = Math.Round((a.TotalAmount.Value * f.Key.ToUSDRate.Value) / currencyToUSDRate, 2),
                                                        d.CountryName,
                                                        d.DMF
                                                    }).ToList();

                    if (dashQuotationCountryList.Count() > 0)
                    {
                        foreach (var item in dashQuotationCountryList)
                        {
                            DashBoardReport dashPerformance = new DashBoardReport();
                            dashPerformance.StoreID = item.StoreID;
                            dashPerformance.Source = item.Source;
                            dashPerformance.YYYYM = item.Value.ToString();
                            dashPerformance.UserID = item.UserID;
                            dashPerformance.LocalAmount = item.LocalAmount;
                            dashPerformance.CountryName = item.CountryName;
                            dashPerformance.DMF = item.DMF;
                            dashBoardPerformance.Add(dashPerformance);
                        }
                    }

                    var dashOrderShortList = (from a in context.Quotations
                                              join b in context.Carts on a.CartID equals b.CartID
                                              join c in context.CartContacts on b.BilltoID equals c.ContactID
                                              join d in context.Countries on c.Country equals d.Shorts
                                              from e in context.vStoreExchangeRates
                                              from f in context.Currencies.GroupBy(t => new { t.CurrencyID, t.ToUSDRate })
                                              where POCOS.Quotation.SuccessfullyStatus.Contains(a.Status) && a.UserID != null && a.StoreID == e.StoreID
                                              && a.QuoteDate >= startdate && a.QuoteDate <= enddate && a.TotalAmount != null && f.Key.CurrencyID == b.Currency
                                              orderby a.QuoteDate
                                              select new
                                              {
                                                  a.StoreID,
                                                  Source = "",
                                                  a.QuoteDate.Value,
                                                  a.UserID,
                                                  LocalAmount = Math.Round((a.TotalAmount.Value * f.Key.ToUSDRate.Value) / currencyToUSDRate, 2),
                                                  d.CountryName,
                                                  d.DMF
                                              }).ToList();

                    if (dashOrderShortList.Count() > 0)
                    {
                        foreach (var item in dashOrderShortList)
                        {
                            DashBoardReport dashPerformance = new DashBoardReport();
                            dashPerformance.StoreID = item.StoreID;
                            dashPerformance.Source = item.Source;
                            dashPerformance.YYYYM = item.Value.ToString();
                            dashPerformance.UserID = item.UserID;
                            dashPerformance.LocalAmount = item.LocalAmount;
                            dashPerformance.CountryName = item.CountryName;
                            dashPerformance.DMF = item.DMF;
                            dashBoardPerformance.Add(dashPerformance);
                        }
                    }
                }

                if (dashBoardPerformance != null)
                    return dashBoardPerformance.ToList();
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }

        /// <summary>
        /// use in Report/Performance/RegistrationDashBoard
        /// </summary>
        /// <returns></returns>
        public List<DashBoardReport> getRegisterDashboard(DateTime startdate, DateTime enddate, string showType)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> _dashRegister = new List<DashBoardReport>();
                if (showType == "Store")
                {
                    var memberList = from p in context.Members
                                     from c in context.Countries
                                     where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                                     && p.Insert_date >= startdate && p.Insert_date <= enddate
                                     && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                     orderby c.StoreID
                                     group c by c.StoreID into g
                                     select new
                                     {
                                         g.Key,
                                         Count = g.Count()
                                     };

                    if (memberList.Count() > 0)
                    {
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key;
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(c => c.ShowSource).ToList();
                    }
                }
                else if (showType == "Country")
                {
                    var memberList = from p in context.Members
                                     from c in context.Countries
                                     where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                                     && p.Insert_date >= startdate && p.Insert_date <= enddate
                                     && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                     orderby c.CountryName
                                     group c by c.CountryName into g
                                     select new
                                     {
                                         g.Key,
                                         Count = g.Count()
                                     };

                    if (memberList.Count() > 0)
                    {
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key;
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(c => c.ShowSource).ToList();
                    }
                }
                else if (showType == "DMF")
                {
                    var memberList = from p in context.Members
                                     from c in context.Countries
                                     where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                                     && p.Insert_date >= startdate && p.Insert_date <= enddate
                                     && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                     orderby c.DMF
                                     group c by c.DMF into g
                                     select new
                                     {
                                         g.Key,
                                         Count = g.Count()
                                     };

                    if (memberList.Count() > 0)
                    {
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key;
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(c => c.ShowSource).ToList();
                    }
                }
                else if (showType == "Month")
                {
                    var memberList = from p in context.Members
                                     where p.Insert_date >= startdate && p.Insert_date <= enddate
                                        && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                     orderby p.Insert_date.Value.Month
                                     group p by
                                         new
                                         {
                                             month = p.Insert_date.Value.Month,
                                             year = p.Insert_date.Value.Year
                                         }
                                         into g
                                         select new
                                         {
                                             g.Key,
                                             Count = g.Count()
                                         };

                    if (memberList.Count() > 0)
                    {
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key.month.ToString("00") + "/" + item.Key.year.ToString("00");
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(p => p.ShowSource).ToList();
                    }
                }
                else if (showType == "Week")
                {
                    var dateList = (from p in context.Members
                                    where p.Insert_date >= startdate && p.Insert_date <= enddate
                                       && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                    orderby p.Insert_date.Value
                                    select new
                                    {
                                        p.Insert_date.Value
                                    }).ToList();

                    if (dateList.Count() > 0)
                    {
                        var memberList = from p in dateList
                                         group p by new
                                         {
                                             week = p.Value.DayOfWeek
                                         }
                                             into g
                                             select new
                                             {
                                                 g.Key,
                                                 Count = g.Count()
                                             };
                        foreach (var item in memberList.OrderBy(c=>c.Key.week))
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key.week.ToString();
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                    }
                }
                else if (showType == "Day")
                {
                    var memberList = from p in context.Members
                                     where p.Insert_date >= startdate && p.Insert_date <= enddate
                                        && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                     orderby p.Insert_date.Value.Day
                                     group p by
                                         new
                                         {
                                             day = p.Insert_date.Value.Day
                                         }
                                         into g
                                         select new
                                         {
                                             g.Key,
                                             Count = g.Count()
                                         };

                    if (memberList.Count() > 0)
                    {
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key.day.ToString("00");
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(p => p.ShowSource).ToList();
                    }
                }
                else if (showType == "Date")
                {
                    var dateList = (from p in context.Members
                                    where p.Insert_date >= startdate && p.Insert_date <= enddate
                                       && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                    orderby p.Insert_date
                                    select new
                                    {
                                        p.Insert_date.Value
                                    }).ToList();

                    if (dateList.Count() > 0)
                    {
                        var memberList = from p in dateList
                                         group p by new
                                         {
                                             DateGroup = p.Value.ToString("MM/dd/yyyy")
                                         }
                                             into g
                                             select new
                                             {
                                                 g.Key,
                                                 Count = g.Count()
                                             };
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key.DateGroup;
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                    }
                }
                else
                {
                    var memberList = from p in context.Members
                                     where p.Insert_date >= startdate && p.Insert_date <= enddate
                                        && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                     orderby p.Insert_date.Value.Hour
                                     group p by
                                         new
                                         {
                                             hourly = p.Insert_date.Value.Hour
                                         }
                                         into g
                                         select new
                                         {
                                             g.Key,
                                             Count = g.Count()
                                         };

                    if (memberList.Count() > 0)
                    {
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key.hourly.ToString("00");
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(p => p.ShowSource).ToList();
                    }
                }


                if (_dashRegister != null)
                    return _dashRegister;
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }

        /// <summary>
        /// return Register Dashboard data source according variable parameter and group 
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="conditions">
        /// Key is DashBoardReport.property name, Value type is same as DashBoardReport.property
        /// </param>
        /// <param name="groupbyProperty">
        /// DashBoardReport.property name
        /// </param>
        /// <returns></returns>
        public List<DashBoardReport> getRegisterDashboard(string startdate, string enddate, Dictionary<string, object> conditions, string groupbyProperty)
        {
            if (startdate == null && enddate == null && string.IsNullOrEmpty(groupbyProperty))
                return new List<DashBoardReport>();
            DateTime startDate = DateTime.Parse(startdate);
            DateTime endDate = DateTime.Parse(enddate);
            endDate = endDate.AddHours(24);
            List<DashBoardReport> _dashRegister = new List<DashBoardReport>();


            ParameterExpression countryPara = Expression.Parameter(typeof(DashBoardReport), "c");

            IQueryable<DashBoardReport> data;

            string oldspace = ((char)160).ToString();
            string newspace = ((char)32).ToString();

            if (conditions.Where(p => p.Key == "Assignee").Count() != 0)
            {
                data = from p in context.Members
                       from t in context.TrackingLogs
                       where (p.EMAIL_ADDR == t.TrackingNo || p.EMAIL_ADDR2 == t.TrackingNo)
                       && p.Insert_date >= startDate && p.Insert_date <= endDate
                       && !p.EMAIL_ADDR.Contains("advantech") && !p.EMAIL_ADDR.Contains("@damez.com")
                       select new DashBoardReport
                       {
                           StoreID = t.StoreID,
                           Store = t.StoreID,
                           Country = p.COUNTRY,
                           LOB = string.IsNullOrEmpty(p.IN_PRODUCT) ? "N/A" : p.IN_PRODUCT,
                           State = string.IsNullOrEmpty(p.STATE) ? "N/A" : p.STATE.Replace(oldspace, newspace).Trim(),
                           StateX = string.IsNullOrEmpty(p.STATE) ? p.COUNTRY : p.COUNTRY + " / " + p.STATE.Replace(oldspace, newspace).Trim(),
                           Source = string.IsNullOrEmpty(p.SOURCE) ? "N/A" : p.SOURCE,
                           Year = p.DATE_REGISTERED.Year,
                           Month = p.DATE_REGISTERED.Month,
                           Day = p.DATE_REGISTERED.Day,
                           Assignee = t.AssignTo
                       };
            }
            else
            {
                if (groupbyProperty != "Assignee")
                {
                    data = from p in context.Members
                           from c in context.Countries
                           where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                           && p.Insert_date >= startDate && p.Insert_date <= endDate
                           && !p.EMAIL_ADDR.Contains("advantech") && !p.EMAIL_ADDR.Contains("@damez.com")
                           select new DashBoardReport
                           {
                               StoreID = c.StoreID,
                               Store = c.StoreID,
                               DMF = c.DMF,
                               Country = c.CountryName,
                               LOB = string.IsNullOrEmpty(p.IN_PRODUCT) ? "N/A" : p.IN_PRODUCT,
                               State = string.IsNullOrEmpty(p.STATE) ? "N/A" : p.STATE.Replace(oldspace, newspace).Trim(),
                               StateX = string.IsNullOrEmpty(p.STATE) ? p.COUNTRY : c.CountryName + " / " + p.STATE.Replace(oldspace, newspace).Trim(),
                               Source = string.IsNullOrEmpty(p.SOURCE) ? "N/A" : p.SOURCE,
                               Year = p.DATE_REGISTERED.Year,
                               Month = p.DATE_REGISTERED.Month,
                               Day = p.DATE_REGISTERED.Day,
                           };
                }
                else
                {
                    data = from p in context.Members
                           join c in context.Countries on p.COUNTRY equals c.CountryName into countries
                           let t = context.TrackingLogs.Where(x => x.TrackingNo == p.EMAIL_ADDR).OrderByDescending(x => x.LastUpdated).FirstOrDefault()
                           where p.Insert_date >= startDate && p.Insert_date <= endDate
                           && !p.EMAIL_ADDR.Contains("advantech") && !p.EMAIL_ADDR.Contains("@damez.com")
                           from c in countries.DefaultIfEmpty()
                           select new DashBoardReport
                           {
                               StoreID = c == null ? "N/A" : c.StoreID,
                               Store = c == null ? "N/A" : c.StoreID,
                               Assignee = t == null ? "N/A" : string.IsNullOrEmpty(t.AssignTo) ? "N/A" : t.AssignTo,
                               Country = p.COUNTRY,
                               LOB = string.IsNullOrEmpty(p.IN_PRODUCT) ? "N/A" : p.IN_PRODUCT,
                               State = string.IsNullOrEmpty(p.STATE) ? "N/A" : p.STATE.Replace(oldspace, newspace).Trim(),
                               StateX = string.IsNullOrEmpty(p.STATE) ? p.COUNTRY : p.COUNTRY + " / " + p.STATE.Replace(oldspace, newspace).Trim(),
                               Source = string.IsNullOrEmpty(p.SOURCE) ? "N/A" : p.SOURCE,
                               Year = p.DATE_REGISTERED.Year,
                               Month = p.DATE_REGISTERED.Month,
                               Day = p.DATE_REGISTERED.Day,
                           };
                }
            }


            if (conditions.Any())
            {
                List<ParameterExpression> pes = new List<ParameterExpression>();
                List<Expression> predicates = new List<Expression>();
                foreach (var c in conditions)
                {
                    Expression left = Expression.Property(countryPara, typeof(DashBoardReport).GetProperty(c.Key));
                    Expression right = Expression.Constant(c.Value);
                    Expression e1 = Expression.Equal(left, right);
                    if (predicates.Any())
                        predicates[0] = Expression.AndAlso(predicates[0], e1);
                    else

                        predicates.Add(e1);
                }
                pes.Add(countryPara);
                var conditional = Expression.Lambda<Func<DashBoardReport, bool>>(predicates[0], pes.ToArray());
                data = data.Where(conditional);
            }

            //only accept int and string, if have more type for group by, please copy below and change int to the type
            if (typeof(DashBoardReport).GetProperty(groupbyProperty).PropertyType.Equals(typeof(int)))
            {
                var selector2 = Expression.Lambda<Func<DashBoardReport, int>>(Expression.Property(countryPara, groupbyProperty), countryPara);

                var memberList = data
              .GroupBy(selector2)
                    .Select((group) => new
                    {
                        Key = group.Key,
                        Count = group.Count()
                    });


                if (memberList.Count() > 0)
                {
                    foreach (var item in memberList)
                    {
                        DashBoardReport dbReport = new DashBoardReport();
                        dbReport.ShowSource = item.Key.ToString();
                        dbReport.RegisterCount = item.Count;
                        _dashRegister.Add(dbReport);
                    }
                }


            }
            else
            {
                var selector = Expression.Lambda<Func<DashBoardReport, string>>(Expression.Property(countryPara, groupbyProperty), countryPara);
                var memberList = data
              .GroupBy(selector)
                    .Select((group) => new
                    {
                        Key = group.Key,
                        Count = group.Count()
                    });


                if (memberList.Count() > 0)
                {
                    foreach (var item in memberList)
                    {
                        DashBoardReport dbReport = new DashBoardReport();
                        dbReport.ShowSource = item.Key;
                        dbReport.RegisterCount = item.Count;
                        _dashRegister.Add(dbReport);
                    }
                }

            }
            return _dashRegister;

        }
        /// <summary>
        /// 点击gridview或者bar 查询相应的信息
        /// </summary>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <param name="showType">查询方式  如store month</param>
        /// <param name="showTypeItem">查询方式的子项  如 store中的'AAU' 'ACN'</param>
        /// <returns></returns>
        public List<DashBoardReport> getRegisterDashboardByMonthly(DateTime startdate, DateTime enddate, string showType, string showTypeItem)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> _dashRegister = new List<DashBoardReport>();
                if (showType == "Month")
                {
                    int month = 0;
                    int.TryParse(showTypeItem, out month);
                    var memberList = from p in context.Members
                                     from c in context.Countries
                                     where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                                     && p.Insert_date >= startdate && p.Insert_date <= enddate
                                     && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                     && p.Insert_date.Value.Month == month
                                     group p by
                                         new
                                         {
                                             store = c.StoreID
                                         }
                                         into g
                                         select new
                                         {
                                             g.Key,
                                             Count = g.Count()
                                         };

                    if (memberList.Count() > 0)
                    {
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key.store;
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(c => c.ShowSource).ToList();
                    }
                }
                else if (showType == "Date")
                {
                    var dateList = (from p in context.Members
                                    from c in context.Countries
                                    where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                                    && p.Insert_date.Value.Year == startdate.Year && p.Insert_date.Value.Month == startdate.Month
                                    && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                    && (c.StoreID == showTypeItem || c.CountryName == showTypeItem || c.DMF == showTypeItem)
                                    orderby p.Insert_date
                                    select new
                                    {
                                        p.Insert_date.Value
                                    }).ToList();

                    if (dateList.Count() > 0)
                    {
                        var memberList = from p in dateList
                                         group p by new
                                         {
                                             DateGroup = p.Value.ToString("MM/dd/yyyy")
                                         }
                                             into g
                                             select new
                                             {
                                                 g.Key,
                                                 Count = g.Count()
                                             };
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key.DateGroup;
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(c => c.ShowSource).ToList();
                    }
                }
                else
                {
                    var memberList = from p in context.Members
                                     from c in context.Countries
                                     where (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                                     && p.Insert_date >= startdate && p.Insert_date <= enddate
                                     && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                     && (c.StoreID == showTypeItem || c.CountryName == showTypeItem || c.DMF == showTypeItem)
                                     group p by
                                         new
                                         {
                                             month = p.Insert_date.Value.Month,
                                             year = p.Insert_date.Value.Year
                                         }
                                         into g
                                         select new
                                         {
                                             g.Key,
                                             Count = g.Count()
                                         };

                    if (memberList.Count() > 0)
                    {
                        foreach (var item in memberList)
                        {
                            DashBoardReport dbReport = new DashBoardReport();
                            dbReport.ShowSource = item.Key.month.ToString("00") + "/" + item.Key.year.ToString("00");
                            dbReport.RegisterCount = item.Count;
                            _dashRegister.Add(dbReport);
                        }
                        _dashRegister = _dashRegister.OrderBy(c => c.ShowSource).ToList();
                    }
                }


                if (_dashRegister != null)
                    return _dashRegister;
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }

        /// <summary>
        /// For OM use,Return Order Quotaion Detail Report
        /// use in Report/Performance/OrderQuotationDetail.aspx
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="showType"></param>
        /// <returns></returns>
        public List<DashBoardReport> getOrderQutaionPerformance(DateTime startdate, DateTime enddate, Decimal currencyToUSDRate = 1)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> dashBoardPerformance = new List<DashBoardReport>();
                var dashOrderCountryList = (from a in context.Orders
                                            join b in context.Carts on a.CartID equals b.CartID
                                            join c in context.CartContacts on b.BilltoID equals c.ContactID
                                            join d in context.Countries on c.Country equals d.CountryName
                                            from e in context.vStoreExchangeRates
                                            where POCOS.Order.SuccessfullyStatus.Contains(a.OrderStatus) && !a.UserID.Contains("@advantech.")
                                                && !a.UserID.Contains("@damez.com") && a.StoreID == e.StoreID
                                                && a.OrderDate >= startdate && a.OrderDate <= enddate && a.TotalAmount != null
                                            select new
                                            {
                                                Type = "Order",
                                                a.StoreID,
                                                a.OrderDate.Value,
                                                LocalAmount = Math.Round((a.TotalAmount.Value * e.ToUSDRate.Value) / currencyToUSDRate, 2),
                                                d.CountryName,
                                                d.DMF
                                            }).ToList();

                if (dashOrderCountryList.Count() > 0)
                {
                    foreach (var item in dashOrderCountryList)
                    {
                        DashBoardReport dashPerformance = new DashBoardReport();
                        dashPerformance.Type = item.Type;
                        dashPerformance.StoreID = item.StoreID;
                        dashPerformance.YYYYM = item.Value.ToString();
                        dashPerformance.LocalAmount = item.LocalAmount;
                        dashPerformance.CountryName = item.CountryName;
                        dashPerformance.DMF = item.DMF;
                        dashBoardPerformance.Add(dashPerformance);
                    }
                }

                var dashOrderShortList = (from a in context.Orders
                                          join b in context.Carts on a.CartID equals b.CartID
                                          join c in context.CartContacts on b.BilltoID equals c.ContactID
                                          join d in context.Countries on c.Country equals d.Shorts
                                          from e in context.vStoreExchangeRates
                                          where POCOS.Order.SuccessfullyStatus.Contains(a.OrderStatus) && !a.UserID.Contains("@advantech.")
                                            && !a.UserID.Contains("@damez.com") && a.StoreID == e.StoreID
                                              && a.OrderDate >= startdate && a.OrderDate <= enddate && a.TotalAmount != null
                                          select new
                                          {
                                              Type = "Order",
                                              a.StoreID,
                                              a.OrderDate.Value,
                                              LocalAmount = Math.Round((a.TotalAmount.Value * e.ToUSDRate.Value) / currencyToUSDRate, 2),
                                              d.CountryName,
                                              d.DMF
                                          }).ToList();

                if (dashOrderShortList.Count() > 0)
                {
                    foreach (var item in dashOrderShortList)
                    {
                        DashBoardReport dashPerformance = new DashBoardReport();
                        dashPerformance.Type = item.Type;
                        dashPerformance.StoreID = item.StoreID;
                        dashPerformance.YYYYM = item.Value.ToString();
                        dashPerformance.LocalAmount = item.LocalAmount;
                        dashPerformance.CountryName = item.CountryName;
                        dashPerformance.DMF = item.DMF;
                        dashBoardPerformance.Add(dashPerformance);
                    }
                }

                var dashQuotationCountryList = (from a in context.Quotations
                                                join b in context.Carts on a.CartID equals b.CartID
                                                join c in context.CartContacts on b.BilltoID equals c.ContactID
                                                join d in context.Countries on c.Country equals d.CountryName
                                                from e in context.vStoreExchangeRates
                                                where POCOS.Quotation.SuccessfullyStatus.Contains(a.Status) && !a.UserID.Contains("@advantech.")
                                                && !a.UserID.Contains("@damez.com") && a.StoreID == e.StoreID
                                                    && a.QuoteDate >= startdate && a.QuoteDate <= enddate && a.TotalAmount != null
                                                select new
                                                {
                                                    Type = "Quotation",
                                                    a.StoreID,
                                                    a.QuoteDate.Value,
                                                    LocalAmount = Math.Round((a.TotalAmount.Value * e.ToUSDRate.Value) / currencyToUSDRate, 2),
                                                    d.CountryName,
                                                    d.DMF
                                                }).ToList();

                if (dashQuotationCountryList.Count() > 0)
                {
                    foreach (var item in dashQuotationCountryList)
                    {
                        DashBoardReport dashPerformance = new DashBoardReport();
                        dashPerformance.Type = item.Type;
                        dashPerformance.StoreID = item.StoreID;
                        dashPerformance.YYYYM = item.Value.ToString();
                        dashPerformance.LocalAmount = item.LocalAmount;
                        dashPerformance.CountryName = item.CountryName;
                        dashPerformance.DMF = item.DMF;
                        dashBoardPerformance.Add(dashPerformance);
                    }
                }

                var dashQuotationShortList = (from a in context.Quotations
                                              join b in context.Carts on a.CartID equals b.CartID
                                              join c in context.CartContacts on b.BilltoID equals c.ContactID
                                              join d in context.Countries on c.Country equals d.Shorts
                                              from e in context.vStoreExchangeRates
                                              where POCOS.Quotation.SuccessfullyStatus.Contains(a.Status) && !a.UserID.Contains("@advantech.")
                                                && !a.UserID.Contains("@damez.com") && a.StoreID == e.StoreID
                                                && a.QuoteDate >= startdate && a.QuoteDate <= enddate && a.TotalAmount != null
                                              select new
                                              {
                                                  Type = "Quotation",
                                                  a.StoreID,
                                                  a.QuoteDate.Value,
                                                  LocalAmount = Math.Round((a.TotalAmount.Value * e.ToUSDRate.Value) / currencyToUSDRate, 2),
                                                  d.CountryName,
                                                  d.DMF
                                              }).ToList();

                if (dashQuotationShortList.Count() > 0)
                {
                    foreach (var item in dashQuotationShortList)
                    {
                        DashBoardReport dashPerformance = new DashBoardReport();
                        dashPerformance.Type = item.Type;
                        dashPerformance.StoreID = item.StoreID;
                        dashPerformance.YYYYM = item.Value.ToString();
                        dashPerformance.LocalAmount = item.LocalAmount;
                        dashPerformance.CountryName = item.CountryName;
                        dashPerformance.DMF = item.DMF;
                        dashBoardPerformance.Add(dashPerformance);
                    }
                }

                if (dashBoardPerformance != null)
                    return dashBoardPerformance.ToList();
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }

        /// <summary>
        /// use in Report/Performance/UserActivityReport.aspx
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="storeId"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public List<sp_getUserActivityDashBoard_Result> getUserActivityReport(DateTime startdate, DateTime enddate, string storeId, string searchType, int topCount = 10)
        {
            if (startdate == null && enddate == null)
                return new List<sp_getUserActivityDashBoard_Result>();
            try
            {
                enddate = enddate.AddHours(24);
                List<sp_getUserActivityDashBoard_Result> _dashPerformance = new List<sp_getUserActivityDashBoard_Result>();
                _dashPerformance = (from p in context.sp_getUserActivityDashBoard(startdate, enddate, storeId, searchType)
                                    select p
                                    ).ToList();

                if (_dashPerformance != null)
                {
                    if (storeId != "ALL" && (searchType == "Site" || searchType == "Page"))
                        _dashPerformance = _dashPerformance.Take(topCount).ToList();
                    return _dashPerformance;
                }
                else
                    return new List<sp_getUserActivityDashBoard_Result>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<sp_getUserActivityDashBoard_Result>();
            }
        }

        /// <summary>
        /// use in Report/Performance/UserActivityReport.aspx keyword
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="storeId"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public List<sp_getUserActivityDashBoard_Result> getUserActivityKeyWordReport(DateTime startdate, DateTime enddate, string storeId, int topCount = 10)
        {
            if (startdate == null && enddate == null)
                return new List<sp_getUserActivityDashBoard_Result>();
            try
            {
                enddate = enddate.AddHours(24);
                List<sp_getUserActivityDashBoard_Result> _dashPerformance = new List<sp_getUserActivityDashBoard_Result>();
                var keyWrodList = (from a in context.UserActivityLogs
                                   where a.CategoryType.Equals("SEARCH") && !string.IsNullOrEmpty(a.ProductID)
                          && (storeId == "ALL" ? true : a.StoreID == storeId)
                                   group a by a.ProductID
                                       into g
                                       select new
                                       {
                                           ShowType = g.Key,
                                           VisitUser = g.Count()
                                       }
                            ).OrderByDescending(p => p.VisitUser).Take(topCount).ToList();

                if (keyWrodList.Count > 0)
                {
                    foreach (var item in keyWrodList)
                    {
                        sp_getUserActivityDashBoard_Result p = new sp_getUserActivityDashBoard_Result();
                        p.ShowType = item.ShowType;
                        p.VisitUser = item.VisitUser;
                        _dashPerformance.Add(p);
                    }
                }
                return _dashPerformance;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<sp_getUserActivityDashBoard_Result>();
            }
        }

        /// <summary>
        /// search hot product Report
        /// use in Report/Performance/HotProductReport.aspx
        /// </summary>
        /// <returns></returns>
        public List<DashBoardReport> getHotProductReport(DateTime startdate, DateTime enddate, string storeId, string searchType, int topCount = 10)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> _dashProduct = new List<DashBoardReport>();
                if (searchType == "Order")
                {
                    var allUserList = (from p in context.Orders
                                       join c in context.CartItems on p.CartID equals c.CartID
                                       join part in context.Parts on c.Part equals part
                                       where POCOS.Order.SuccessfullyStatus.Contains(p.OrderStatus) && !p.UserID.ToLower().Contains("@advantech.")
                                       && !p.UserID.ToLower().Contains("@damez.com")
                                       && p.OrderDate >= startdate && p.OrderDate <= enddate
                                       && (storeId == "ALL" ? true : p.StoreID == storeId && c.StoreID == storeId)
                                       group c by new { c.ProductName, part.ABCInd, c.ItemType }
                                           into g
                                           select new
                                           {
                                               g.Key.ProductName,
                                               g.Key.ABCInd,
                                               g.Key.ItemType,
                                               //Count = g.Count()
                                               Count = g.Sum(p => p.Qty)
                                           }
                                        ).OrderByDescending(p => p.Count).Take(topCount).ToList();

                    if (allUserList.Count > 0)
                    {
                        foreach (var item in allUserList)
                        {
                            DashBoardReport p = new DashBoardReport();
                            p.DisplayPartno = item.ProductName;
                            p.ABCInd = item.ABCInd;
                            p.Type = item.ItemType;
                            p.ShowSource = item.Count.ToString();
                            //导出excel int类型默认显示0,用string
                            p.Country = "";
                            p.CountryName = "";
                            _dashProduct.Add(p);
                        }
                    }
                }
                else if (searchType == "Quotation")//查找Quotation 产品
                {
                    var allUserList = (from p in context.Quotations
                                       join c in context.CartItems on p.CartID equals c.CartID
                                       join part in context.Parts on c.Part equals part
                                       where POCOS.Quotation.SuccessfullyStatus.Contains(p.Status) && !p.UserID.ToLower().Contains("@advantech.")
                                           && !p.UserID.ToLower().Contains("@damez.com")
                                           && p.QuoteDate >= startdate && p.QuoteDate <= enddate
                                           && (storeId == "ALL" ? true : p.StoreID == storeId && c.StoreID == storeId)
                                       group c by new { c.ProductName, part.ABCInd, c.ItemType }
                                           into g
                                           select new
                                           {
                                               g.Key.ProductName,
                                               g.Key.ItemType,
                                               g.Key.ABCInd,
                                               Count = g.Sum(p => p.Qty)
                                               //Count = g.Count()
                                           }
                                        ).OrderByDescending(p => p.Count).Take(topCount).ToList();
                    if (allUserList.Count > 0)
                    {
                        foreach (var item in allUserList)
                        {
                            DashBoardReport p = new DashBoardReport();
                            p.DisplayPartno = item.ProductName;
                            p.ABCInd = item.ABCInd;
                            p.Type = item.ItemType;
                            p.ShowSource = item.Count.ToString();
                            //导出excel int类型默认显示0,用string
                            p.Country = "";
                            p.CountryName = "";
                            _dashProduct.Add(p);
                        }
                    }
                }
                else//根据用户日志查找
                {
                    var currentUserList = (from a in context.UserActivityLogs
                                           from b in context.Parts.OfType<Product>()
                                           where a.ProductID == b.SProductID
                                               && a.ProductID != null && a.CreatedDate >= startdate && a.CreatedDate <= enddate
                                               && (!a.UserId.ToLower().Contains("@advantech.") && !a.UserId.Contains("@damez.com") || a.UserId == null)
                                               && (storeId == "ALL" ? true : a.StoreID == storeId && b.StoreID == storeId)
                                               && a.CategoryType.ToUpper() != "SEARCH"
                                           select new { b.DisplayPartno, b.ABCInd, b.ProductType, a.ID }).Distinct().ToList();
                    var allUserList = (from a in currentUserList
                                       group a by new { a.DisplayPartno, a.ABCInd, a.ProductType } into g
                                       select new
                                       {
                                           g.Key.DisplayPartno,
                                           g.Key.ABCInd,
                                           g.Key.ProductType,
                                           Count = g.Count()
                                       }
                                ).OrderByDescending(p => p.Count).Take(topCount).ToList();
                    if (allUserList.Count > 0)
                    {
                        foreach (var item in allUserList)
                        {
                            DashBoardReport p = new DashBoardReport();
                            p.DisplayPartno = item.DisplayPartno;
                            p.ABCInd = item.ABCInd;
                            p.Type = item.ProductType;
                            p.ShowSource = item.Count.ToString();
                            _dashProduct.Add(p);
                        }
                    }
                }
                if (_dashProduct != null)
                    return _dashProduct;
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }

        /// <summary>
        /// get hot product detail
        /// </summary>
        /// <returns></returns>
        public List<DashBoardReport> getHotProductDetailReport(DateTime startdate, DateTime enddate, string storeId, string searchType, string sproductId)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> _dashProduct = new List<DashBoardReport>();
                if (searchType == "Order")
                {
                    var allUserList = (from p in context.Orders
                                       join c in context.CartItems on p.CartID equals c.CartID
                                       where POCOS.Order.SuccessfullyStatus.Contains(p.OrderStatus) && !p.UserID.ToLower().Contains("@advantech")
                                       && !p.UserID.ToLower().Contains("@damez.com")
                                       && p.OrderDate >= startdate && p.OrderDate <= enddate
                                       && c.ProductName == sproductId
                                       && (storeId == "ALL" ? true : p.StoreID == storeId && c.StoreID == storeId)
                                       select new
                                       {
                                           p.OrderNo,
                                           p.OrderDate,
                                           p.TotalAmount,
                                           c.Qty
                                       }).OrderByDescending(p => p.OrderDate).ToList();

                    if (allUserList.Count > 0)
                    {
                        foreach (var item in allUserList)
                        {
                            DashBoardReport p = new DashBoardReport();
                            p.Source = item.OrderNo;
                            p.YYYYM = item.OrderDate.Value.ToShortDateString();
                            p.LocalAmount = item.TotalAmount.Value;
                            p.Cnt = item.Qty;
                            //导出excel int类型默认显示0,用string
                            p.Country = p.LocalAmount.ToString();
                            p.CountryName = p.Cnt.ToString();
                            _dashProduct.Add(p);
                        }
                    }
                }
                else if (searchType == "Quotation")//查找Quotation 产品
                {
                    var allUserList = (from p in context.Quotations
                                       join c in context.CartItems on p.CartID equals c.CartID
                                       where POCOS.Quotation.SuccessfullyStatus.Contains(p.Status) && !p.UserID.ToLower().Contains("@advantech")
                                           && !p.UserID.ToLower().Contains("@damez.com")
                                           && p.QuoteDate >= startdate && p.QuoteDate <= enddate
                                           && c.ProductName == sproductId
                                           && (storeId == "ALL" ? true : p.StoreID == storeId && c.StoreID == storeId)
                                       select new
                                       {
                                           p.QuotationNumber,
                                           p.QuoteDate,
                                           p.TotalAmount,
                                           c.Qty
                                       }).OrderByDescending(p => p.QuoteDate).ToList();
                    if (allUserList.Count > 0)
                    {
                        foreach (var item in allUserList)
                        {
                            DashBoardReport p = new DashBoardReport();
                            p.Source = item.QuotationNumber;
                            p.YYYYM = item.QuoteDate.Value.ToShortDateString();
                            p.LocalAmount = item.TotalAmount.Value;
                            p.Cnt = item.Qty;
                            //导出excel int类型默认显示0,用string
                            p.Country = p.LocalAmount.ToString();
                            p.CountryName = p.Cnt.ToString();
                            _dashProduct.Add(p);
                        }
                    }
                }
                if (_dashProduct != null)
                    return _dashProduct;
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }

        /// <summary>
        /// search product Report by Name
        /// use in Report/Performance/ProductReport.aspx
        /// </summary>
        /// <returns></returns>
        public List<DashBoardReport> getProductReportByName(DateTime startdate, DateTime enddate, string storeId, string searchType, string productNo)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> _dashProduct = new List<DashBoardReport>(); ;
                if (searchType == "Order")
                {
                    if (storeId != "ALL")
                    {
                        var allUserList = (from p in context.Orders
                                  join c in context.CartItems on p.CartID equals c.CartID
                                  join cart in context.Carts on c.CartID equals cart.CartID
                                  join part in context.Parts on c.Part equals part
                                  where POCOS.Order.SuccessfullyStatus.Contains(p.OrderStatus)
                                  && !p.UserID.Contains("@advantech.")
                                  && !p.UserID.Contains("@damez.com")
                                  && p.OrderDate >= startdate && p.OrderDate <= enddate && p.StoreID == storeId
                               &&
                                  (
                                  string.IsNullOrEmpty(productNo) ||
                                  c.ProductName.StartsWith(productNo))
                                  group c by new { c.ProductName, part.ABCInd, cart.MiniSiteID}
                                      into g
                                      select new
                                      {
                                          g.Key.MiniSiteID,
                                          g.Key.ProductName,
                                          g.Key.ABCInd,
                                          Count = g.Sum(p => p.Qty)
                                      }).OrderByDescending(p => p.Count).ToList();

                        //var allUserList = (from p in context.Orders
                        //                   join c in context.CartItems on p.CartID equals c.CartID
                        //                   join part in context.Parts on c.Part equals part
                        //                   where POCOS.Order.SuccessfullyStatus.Contains(p.OrderStatus) && !p.UserID.Contains("@advantech.")
                        //                   && !p.UserID.Contains("@damez.com")
                        //                   && p.OrderDate >= startdate && p.OrderDate <= enddate && p.StoreID == storeId
                        //                   &&
                        //                   (
                        //                   string.IsNullOrEmpty(productNo) ||
                        //                   c.ProductName.StartsWith(productNo))
                        //                   group c by new { c.ProductName, part.ABCInd }
                        //                       into g
                        //                       select new
                        //                       {
                        //                           g.Key.ProductName,
                        //                           g.Key.ABCInd,
                        //                           Count = g.Sum(p => p.Qty)
                        //                           //Count = g.Count()
                        //                       }
                        //                ).OrderByDescending(p => p.Count).ToList();

                        if (allUserList.Count > 0)
                        {
                            foreach (var item in allUserList)
                            {
                                DashBoardReport p = new DashBoardReport();
                                if (item.MiniSiteID == null)
                                    p.Store = "eStore";
                                else
                                    p.Store = (from m in context.MiniSites
                                              where m.ID == item.MiniSiteID
                                              select m.SiteName).FirstOrDefault();
                                p.DisplayPartno = item.ProductName;
                                p.ABCInd = item.ABCInd;
                                p.ShowSource = item.Count.ToString();
                                _dashProduct.Add(p);
                            }
                        }
                    }
                    else
                    {
                       
                        var allUserList = (from p in context.Orders
                                           join c in context.CartItems on p.CartID equals c.CartID
                                           join cart in context.Carts on c.CartID equals cart.CartID
                                           join part in context.Parts on c.Part equals part
                                           where POCOS.Order.SuccessfullyStatus.Contains(p.OrderStatus)
                                           && !p.UserID.Contains("@advantech.")
                                           && !p.UserID.Contains("@damez.com")
                                           && p.OrderDate >= startdate && p.OrderDate <= enddate
                                        &&
                                           (
                                           string.IsNullOrEmpty(productNo) ||
                                           c.ProductName.StartsWith(productNo))
                                           group c by new { c.ProductName, part.ABCInd, cart.MiniSiteID }
                                               into g
                                               select new
                                               {
                                                   g.Key.MiniSiteID,
                                                   g.Key.ProductName,
                                                   g.Key.ABCInd,
                                                   Count = g.Sum(p => p.Qty)
                                               }).OrderByDescending(p => p.Count).ToList();
                        if (allUserList.Count > 0)
                        {
                            foreach (var item in allUserList)
                            {
                                DashBoardReport p = new DashBoardReport();
                                if (item.MiniSiteID == null)
                                    p.Store = "eStore";
                                else
                                    p.Store = (from m in context.MiniSites
                                               where m.ID == item.MiniSiteID
                                               select m.SiteName).FirstOrDefault();
                                p.DisplayPartno = item.ProductName;
                                p.ABCInd = item.ABCInd;
                                p.ShowSource = item.Count.ToString();
                                _dashProduct.Add(p);
                            }
                        }
                    }
                }
                else if (searchType == "Quotation")//查找Quotation 产品
                {
                    if (storeId != "ALL")
                    {
                        var allUserList = (from p in context.Quotations
                                           join c in context.CartItems on p.CartID equals c.CartID
                                           join cart in context.Carts on c.CartID equals cart.CartID
                                           join part in context.Parts on c.Part equals part
                                           where POCOS.Quotation.SuccessfullyStatus.Contains(p.Status) && !p.UserID.Contains("@advantech.")
                                           && !p.UserID.Contains("@damez.com") 
                                           && p.QuoteDate >= startdate && p.QuoteDate <= enddate && p.StoreID == storeId
                                          &&
                                           (
                                           string.IsNullOrEmpty(productNo) ||
                                           c.ProductName.StartsWith(productNo))
                                           group c by new { c.ProductName, part.ABCInd, cart.MiniSiteID }
                                               into g
                                               select new
                                               {
                                                   g.Key.MiniSiteID,
                                                   g.Key.ProductName,
                                                   g.Key.ABCInd,
                                                   Count = g.Sum(p => p.Qty)
                                                   //Count = g.Count()
                                               }
                                        ).OrderByDescending(p => p.Count).ToList();
                        if (allUserList.Count > 0)
                        {
                            foreach (var item in allUserList)
                            {
                                DashBoardReport p = new DashBoardReport();
                                if (item.MiniSiteID == null)
                                    p.Store = "eStore";
                                else
                                    p.Store = (from m in context.MiniSites
                                               where m.ID == item.MiniSiteID
                                               select m.SiteName).FirstOrDefault();
                                p.DisplayPartno = item.ProductName;
                                p.ABCInd = item.ABCInd;
                                p.ShowSource = item.Count.ToString();
                                _dashProduct.Add(p);
                            }
                        }
                    }
                    else
                    {
                        var allUserList = (from p in context.Quotations
                                           join c in context.CartItems on p.CartID equals c.CartID
                                           join cart in context.Carts on c.CartID equals cart.CartID
                                           join part in context.Parts on c.Part equals part
                                           where POCOS.Quotation.SuccessfullyStatus.Contains(p.Status) && !p.UserID.Contains("@advantech.")
                                           && !p.UserID.Contains("@damez.com") 
                                           && p.QuoteDate >= startdate && p.QuoteDate <= enddate
                                         &&
                                           (
                                           string.IsNullOrEmpty(productNo) ||
                                           c.ProductName.StartsWith(productNo))
                                           group c by new { c.ProductName, part.ABCInd, cart.MiniSiteID}
                                               into g
                                               select new
                                               {
                                                   g.Key.MiniSiteID,
                                                   g.Key.ProductName,
                                                   g.Key.ABCInd,
                                                   Count = g.Sum(p => p.Qty)
                                                   //Count = g.Count()
                                               }
                                        ).OrderByDescending(p => p.Count).ToList();
                        if (allUserList.Count > 0)
                        {
                            foreach (var item in allUserList)
                            {
                                DashBoardReport p = new DashBoardReport();
                                if (item.MiniSiteID == null)
                                    p.Store = "eStore";
                                else
                                    p.Store = (from m in context.MiniSites
                                               where m.ID == item.MiniSiteID
                                               select m.SiteName).FirstOrDefault();
                                p.DisplayPartno = item.ProductName;
                                p.ABCInd = item.ABCInd;
                                p.ShowSource = item.Count.ToString();
                                _dashProduct.Add(p);
                            }
                        }
                    }
                }
                else//根据用户日志查找
                {
                    if (storeId != "ALL")
                    {
                        var currentUserList = (from a in context.UserActivityLogs
                                               from b in context.Parts.OfType<Product>()
                                               where a.ProductID == b.SProductID
                                                   && a.ProductID != null && a.CreatedDate >= startdate && a.CreatedDate <= enddate && a.StoreID == b.StoreID
                                                   && (!a.UserId.ToLower().Contains("@advantech.") && !a.UserId.Contains("@damez.com") || a.UserId == null)
                                                   && (b.SProductID.StartsWith(productNo) || b.DisplayPartno.StartsWith(productNo)) && a.StoreID == storeId
                                                   && a.CategoryType.ToUpper() != "SEARCH"
                                               select new { b.DisplayPartno,b.ABCInd , a.ID }).Distinct().ToList();
                        var allUserList = (from a in currentUserList
                                           group a by new { a.DisplayPartno, a.ABCInd } into g
                                           select new
                                           {
                                               g.Key.DisplayPartno,
                                               g.Key.ABCInd,
                                               Count = g.Count()
                                           }
                                    ).OrderByDescending(p => p.Count).ToList();
                        //var allUserList = (from a in context.UserActivityLogs
                        //                   join b in context.Parts.OfType<Product>() on a.ProductID equals b.SProductID
                        //                   where a.ProductID != null && a.CreatedDate >= startdate && a.CreatedDate <= enddate
                        //                   && !a.UserId.Contains("@advantech.") && !a.UserId.Contains("@damez.com") 
                        //                   && a.StoreID == storeId
                        //                   && b.DisplayPartno.StartsWith(productNo)
                        //                   group b by b.DisplayPartno
                        //                       into g
                        //                       select new
                        //                       {
                        //                           g.Key,
                        //                           Count = g.Count()
                        //                       }
                        //                ).OrderByDescending(p => p.Count).ToList();
                        if (allUserList.Count > 0)
                        {
                            foreach (var item in allUserList)
                            {
                                DashBoardReport p = new DashBoardReport();
                                p.Store = "";
                                p.DisplayPartno = item.DisplayPartno;
                                p.ABCInd = item.ABCInd;
                                p.ShowSource = item.Count.ToString();
                                _dashProduct.Add(p);
                            }
                        }
                    }
                    else
                    {
                        //这个代码用 join 不用from 也行
                        var currentUserList = (from a in context.UserActivityLogs
                                             from b in context.Parts.OfType<Product>() where a.ProductID == b.SProductID
                                             && a.ProductID != null && a.CreatedDate >= startdate && a.CreatedDate <= enddate
                                             && (!a.UserId.ToLower().Contains("@advantech.") && !a.UserId.Contains("@damez.com") || a.UserId == null)
                                             && b.SProductID.StartsWith(productNo) && a.CategoryType.ToUpper() != "SEARCH"
                                             select new { b.DisplayPartno, a.ID}).Distinct().ToList();
                        var allUserList =(from a in currentUserList
                                    group a by a.DisplayPartno into g
                                        select new
                                        {
                                            g.Key,
                                            Count = g.Count()
                                        }
                                    ).OrderByDescending(p => p.Count).ToList();

                        #region 同样解决的代码
                        //解决product 中多个store 有重复现象
                        //var allUserLista = (from a in context.UserActivityLogs
                        //                    join b in context.Parts.OfType<Product>() on a.ProductID equals b.SProductID
                        //                    where a.ProductID != null && a.CreatedDate >= startdate && a.CreatedDate <= enddate
                        //                    && !a.UserId.Contains("@advantech.") && !a.UserId.Contains("@damez.com")
                        //                    && b.DisplayPartno.StartsWith(productNo)

                        //                    group b by new {a.ID,a.ProductID}
                        //                        into g

                        //                        select new
                        //                        {
                        //                            g.Key.ProductID,
                        //                            g.Key.ID,
                        //                            Count = g.Count(),
                        //                        }
                        //                ).OrderByDescending(p => p.Count).Distinct().ToList();
                        //var aaaa = (from a in allUserLista
                        //              group a by a.ProductID
                        //                  into g
                        //                  select new
                        //                  {
                        //                      g.Key,
                        //                      Count = g.Count()
                        //                  }).OrderByDescending(p => p.Count).ToList();

                        //product 中多个store 有重复现象
                        //var allUserList = (from a in context.UserActivityLogs
                        //                   join b in context.Parts.OfType<Product>() on a.ProductID equals b.SProductID
                        //                   where a.ProductID != null && a.CreatedDate >= startdate && a.CreatedDate <= enddate
                        //                   && !a.UserId.Contains("@advantech.") && !a.UserId.Contains("@damez.com") 
                        //                   && b.DisplayPartno.StartsWith(productNo)
                        //                   group b by b.DisplayPartno
                        //                       into g
                        //                       select new
                        //                       {
                        //                           g.Key,
                        //                           Count = g.Count()
                        //                       }
                        //                ).OrderByDescending(p => p.Count).ToList();
                        #endregion
                        
                        if (allUserList.Count > 0)
                        {
                            foreach (var item in allUserList)
                            {
                                DashBoardReport p = new DashBoardReport();
                                p.DisplayPartno = item.Key;
                                p.ShowSource = item.Count.ToString();
                                _dashProduct.Add(p);
                            }
                        }
                    }
                }
                if (_dashProduct != null)
                    return _dashProduct;
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }
        /// <summary>
        /// searct Register Conversion Report
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public List<sp_getRegisterConversion_Result> getRegisterConversion(DateTime startdate, DateTime enddate, string storeId, bool isEStore)
        {
            if (startdate == null && enddate == null)
                return new List<sp_getRegisterConversion_Result>();
            try
            {
                enddate = enddate.AddHours(24);
                List<sp_getRegisterConversion_Result> _dashPerformance = new List<sp_getRegisterConversion_Result>();
                _dashPerformance = (from r in context.sp_getRegisterConversion(startdate, enddate, storeId, isEStore) select r).ToList();

                if (_dashPerformance != null)
                    return _dashPerformance;
                else
                    return new List<sp_getRegisterConversion_Result>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<sp_getRegisterConversion_Result>();
            }
        }

        /// <summary>
        /// 获取具体 用户  1，3，6，12 下单
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="showType"></param>
        /// <param name="showTypeValue"></param>
        /// <param name="month"></param>
        /// <param name="isEStore"></param>
        /// <returns></returns>
        public List<sp_getConversionDetail_Result> getConversionDetail(DateTime startdate, DateTime enddate, string showType, string showTypeValue, int month, bool isEStore)
        {
            if (startdate == null && enddate == null)
                return new List<sp_getConversionDetail_Result>();
            try
            {
                enddate = enddate.AddHours(24);
                List<sp_getConversionDetail_Result> _dashPerformance = new List<sp_getConversionDetail_Result>();
                //_dashPerformance = (from r in context.sp_getRegisterConversion(startdate, enddate, showType, isEStore) select r).ToList();
                _dashPerformance = (from r in context.sp_getConversionDetail(startdate, enddate, showType, showTypeValue, month, isEStore) select r).ToList();

                if (_dashPerformance != null)
                    return _dashPerformance;
                else
                    return new List<sp_getConversionDetail_Result>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<sp_getConversionDetail_Result>();
            }
        }

        /// <summary>
        /// search promotion report
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="storeId"></param>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        public List<DashBoardReport> getPromotionReport(DateTime startdate, DateTime enddate, string storeId, string promotionCode, string type)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> _dashProduct = new List<DashBoardReport>();
                var allPromotion = (from p in context.PromotionAppliedLogs
                                    join c in context.Campaigns on p.CampaignID equals c.CampaignID
                                    where p.CreatedDate >= startdate && p.CreatedDate <= enddate
                                    && (type == "ALL" ? true : p.Type == type)
                                    && (storeId == "ALL" ? true : p.StoreID == storeId && c.StoreID == storeId)
                                    && (string.IsNullOrEmpty(promotionCode) ? true : p.PromotionCode == promotionCode)
                                    group c by new { c.CampaignID, c.PromotionCode, c.Description, p.Type }
                                        into g
                                        select new DashBoardReport
                                        {
                                            Cnt = g.Key.CampaignID,
                                            Source = g.Key.PromotionCode,
                                            ShowSource = g.Key.Description,
                                            Type = g.Key.Type,
                                            CustomersCount = g.Count()
                                        }).ToList();
                _dashProduct = allPromotion.ToList();
                if (_dashProduct != null)
                    return _dashProduct;
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }

        /// <summary>
        /// 获取 使用Promotion 的order和quotation
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="storeId"></param>
        /// <param name="campaignID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<DashBoardReport> getPromotionDetailReport(DateTime startdate, DateTime enddate, string storeId, int campaignID, string type)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> _dashProduct = new List<DashBoardReport>();
                var allPromotion = (from p in context.PromotionAppliedLogs
                                    where p.CreatedDate >= startdate && p.CreatedDate <= enddate
                                    && (type == "ALL" ? true : p.Type == type)
                                    && (storeId == "ALL" ? true : p.StoreID == storeId)
                                    && p.CampaignID == campaignID
                                    select new DashBoardReport()
                                    {
                                        Source = p.QuoteOrderNo,
                                        Type = p.Type,
                                        UserID = p.UserID,
                                        ReportDate = p.CreatedDate
                                    }).ToList();
                _dashProduct = allPromotion.ToList();
                if (_dashProduct != null)
                    return _dashProduct;
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }

        public List<DashBoardReport> getProductLineReport(DateTime startdate, DateTime enddate, string storeId = "ALL", Decimal currencyToUSDRate = 1)
        {
            if (startdate == null && enddate == null)
                return new List<DashBoardReport>();
            try
            {
                enddate = enddate.AddHours(24);
                List<DashBoardReport> dashBoardPerformance = new List<DashBoardReport>();
                //product
                #region 删除
                //dashBoardPerformance = (from a in context.Orders
                //                        join c in context.CartItems on a.CartID equals c.CartID
                //                        join d in context.Parts on c.SProductID equals d.SProductID
                //                        from f in context.vStoreExchangeRates
                //                        //where a.OrderStatus == "Confirmed" && !a.UserID.Contains("@advantech.")
                //                        where POCOS.Order.SuccessfullyStatus.Contains(a.OrderStatus) && !a.UserID.Contains("@advantech.")
                //                             && !a.UserID.Contains("@damez.com") && (storeId == "ALL" ? true : a.StoreID == storeId)
                //                             && a.StoreID == c.StoreID && a.StoreID == d.StoreID && a.StoreID == f.StoreID
                //                             && a.OrderDate >= startdate && a.OrderDate <= enddate && a.TotalAmount != null
                //                             && string.IsNullOrEmpty(c.BTOConfigID) //只取product 不取ctos
                //                             && !string.IsNullOrEmpty(d.VendorProductLine)
                //                        select new
                //                        {
                //                            DisplayPartno = d.VendorProductLine,
                //                            Cnt = c.Qty,
                //                            LocalAmount = Math.Round((c.AdjustedPrice * f.ToUSDRate.Value) / currencyToUSDRate, 2)
                //                        }).ToList().GroupBy(p => p.DisplayPartno).Select(p => new DashBoardReport() { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();
                #endregion

                dashBoardPerformance = (from a in context.Orders
                                        join b in context.Carts on a.CartID equals b.CartID
                                        join c in context.CartItems on b.CartID equals c.CartID
                                        join d in context.Parts on c.SProductID equals d.SProductID
                                        from f in context.vStoreExchangeRates
                                        where POCOS.Order.SuccessfullyStatus.Contains(a.OrderStatus) && !a.UserID.Contains("@advantech.")
                                             && !a.UserID.Contains("@damez.com") && (storeId == "ALL" ? true : a.StoreID == storeId)
                                             && a.StoreID == b.StoreID && a.StoreID == d.StoreID && a.StoreID == f.StoreID
                                             && a.OrderDate >= startdate && a.OrderDate <= enddate && a.TotalAmount != null
                                             && string.IsNullOrEmpty(c.BTOConfigID) //只取product 不取ctos
                                             && !string.IsNullOrEmpty(d.VendorProductLine)
                                        select new
                                        {
                                            DisplayPartno = d.VendorProductLine,
                                            Cnt = c.Qty,
                                            LocalAmount = Math.Round((c.AdjustedPrice * f.ToUSDRate.Value) / currencyToUSDRate, 2)
                                        }).ToList().GroupBy(p => p.DisplayPartno).Select(p => new DashBoardReport() { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();

                #region ctos
                //ctos 
                List<DashBoardReport> ctosDashBoardPerformance = new List<DashBoardReport>();

                #region 删除
                //ctosDashBoardPerformance = (from a in context.Orders
                //                            join c in context.CartItems on new { a.CartID, a.StoreID } equals new { c.CartID, c.StoreID }
                //                            join e in context.BTOSConfigs on c.BTOConfigID equals e.BTOConfigID
                //                            join f in context.BTOSConfigDetails on new { e.BTOConfigID, e.ConfigID } equals new { f.BTOConfigID, f.ConfigID }
                //                            join d in context.Parts on f.SProductID equals d.SProductID
                //                            from v in context.vStoreExchangeRates
                //                            where POCOS.Order.SuccessfullyStatus.Contains(a.OrderStatus) && !a.UserID.Contains("@advantech.")
                //                            && !a.UserID.Contains("@damez.com") && (storeId == "ALL" ? true : a.StoreID == storeId)
                //                            && a.StoreID == c.StoreID && a.StoreID == d.StoreID && a.StoreID == v.StoreID
                //                            && a.OrderDate >= startdate && a.OrderDate <= enddate && a.TotalAmount != null
                //                            && !string.IsNullOrEmpty(c.BTOConfigID) //只取ctos 不取product
                //                            && !string.IsNullOrEmpty(d.VendorProductLine)
                //                            select new
                //                            {
                //                                DisplayPartno = d.VendorProductLine,
                //                                Cnt = c.Qty * e.Qty * f.Qty,
                //                                LocalAmount = Math.Round((c.Qty * e.Qty * f.Qty * f.AdjustedPrice.Value * v.ToUSDRate.Value) / currencyToUSDRate, 2)
                //                            }).ToList().GroupBy(p => p.DisplayPartno).Select(p => new DashBoardReport() { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();
                #endregion

                ctosDashBoardPerformance = (from a in context.Orders
                                            join b in context.Carts on new { a.CartID, a.StoreID } equals new { b.CartID, b.StoreID }
                                            join c in context.CartItems on new { b.CartID, b.StoreID } equals new { c.CartID, c.StoreID }
                                            join h in context.BTOSystems on c.BTOConfigID equals h.BTOConfigID
                                            join e in context.BTOSConfigs on h.BTOConfigID equals e.BTOConfigID
                                            join f in context.BTOSConfigDetails on new { e.BTOConfigID, e.ConfigID } equals new { f.BTOConfigID, f.ConfigID }
                                            join d in context.Parts on f.SProductID equals d.SProductID
                                            from v in context.vStoreExchangeRates
                                            //where a.OrderStatus == "Confirmed" && !a.UserID.Contains("@advantech.")
                                            where POCOS.Order.SuccessfullyStatus.Contains(a.OrderStatus) && !a.UserID.Contains("@advantech.")
                                            && !a.UserID.Contains("@damez.com") && (storeId == "ALL" ? true : a.StoreID == storeId)
                                            && a.StoreID == b.StoreID && a.StoreID == c.StoreID && a.StoreID == d.StoreID && a.StoreID == v.StoreID
                                            && a.OrderDate >= startdate && a.OrderDate <= enddate && a.TotalAmount != null
                                            && !string.IsNullOrEmpty(c.BTOConfigID) //只取ctos 不取product
                                            && !string.IsNullOrEmpty(d.VendorProductLine)
                                            select new
                                            {
                                                DisplayPartno = d.VendorProductLine,
                                                Cnt = c.Qty * e.Qty * f.Qty,
                                                LocalAmount = Math.Round((c.Qty * e.Qty * f.Qty * f.AdjustedPrice.Value * v.ToUSDRate.Value) / currencyToUSDRate, 2)
                                            }).ToList().GroupBy(p => p.DisplayPartno).Select(p => new DashBoardReport() { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();

                dashBoardPerformance.AddRange(ctosDashBoardPerformance);

                dashBoardPerformance = dashBoardPerformance.GroupBy(p => p.DisplayPartno).Select(p => new DashBoardReport() { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();
                #endregion

                if (dashBoardPerformance != null)
                    return dashBoardPerformance.ToList();
                else
                    return new List<DashBoardReport>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<DashBoardReport>();
            }
        }
        #endregion

        #region Flash Reprot
        //生成FlashBoard模板
        private List<POCOS.DashBoardReport> genarateFlashDaskBoardList(DateTime thisTwoStartMonth, DateTime thisOneStartMonth, DateTime thisCurrentMonth, bool isYTM = true)
        {
            List<POCOS.DashBoardReport> registerDashBoardList = new List<POCOS.DashBoardReport>();
            POCOS.DashBoardReport excampleReport = new POCOS.DashBoardReport();
            excampleReport.DisplayPartno = "Last 2 month";
            excampleReport.ReportDate = thisTwoStartMonth;
            registerDashBoardList.Add(excampleReport);

            excampleReport = new POCOS.DashBoardReport();
            excampleReport.DisplayPartno = "Last 1 month";
            excampleReport.ReportDate = thisOneStartMonth;
            registerDashBoardList.Add(excampleReport);

            excampleReport = new POCOS.DashBoardReport();
            excampleReport.DisplayPartno = "Current Month (" + thisCurrentMonth.Month + ")";
            excampleReport.ReportDate = thisCurrentMonth;
            registerDashBoardList.Add(excampleReport);

            if (isYTM)
            {
                excampleReport = new POCOS.DashBoardReport();
                excampleReport.DisplayPartno = "YTM";
                excampleReport.ReportDate = thisCurrentMonth;
                registerDashBoardList.Add(excampleReport);
            }

            return registerDashBoardList;
        }
        /// <summary>
        /// get eStore Flash Reprots
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="successfullyStatus">order Success status</param>
        /// <param name="noPassStatus">order Pass Status</param>
        /// <returns></returns>
        public List<FlashReport> getFlashReport(string storeId, List<string> orderSuccessfullyStatus, List<string> orderNoPassStatus, DateTime currencyMotnh)
        {
            //本月 前1月 前2月 今年
            DateTime thisCurrentMonth = currencyMotnh;//DateTime.Parse(DateTime.Now.ToShortDateString());
            //thisCurrentMonth = thisCurrentMonth.AddDays(1 - thisCurrentMonth.Day);

            DateTime thisOneStartMonth = thisCurrentMonth.AddMonths(-1);
            DateTime thisOneEndMonth = thisOneStartMonth.AddMonths(1);
            DateTime thisTwoStartMonth = thisOneStartMonth.AddMonths(-1);
            DateTime thisTwoEndMonth = thisTwoStartMonth.AddMonths(1);
            //去年本月 去年前1月 去年前2月 去年
            DateTime lastCurrentMonth = thisCurrentMonth.AddYears(-1);
            DateTime lastOneStartMonth = thisOneStartMonth.AddYears(-1);
            DateTime lastOneEndMonth = thisOneEndMonth.AddYears(-1);
            DateTime lastTwoStartMonth = thisTwoStartMonth.AddYears(-1);
            DateTime lastTwoEndMonth = thisTwoEndMonth.AddYears(-1);

            List<POCOS.FlashReport> flashList = new List<POCOS.FlashReport>();

            //只取 eStore注册的.
            #region Rigisteants (last 3 months)
            POCOS.FlashReport flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Registrant (last 3 months)";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "# of Register";
            flashItem.HeaderNameThree = "YoY%";
            flashItem.HeaderNameFour = "Remarks";

            //currentMonth
            List<POCOS.DashBoardReport> registerDashBoardList = genarateFlashDaskBoardList(thisTwoStartMonth, thisOneStartMonth, thisCurrentMonth);
            flashItem.DashBoardList = registerDashBoardList;

            var currentRegisterFlash = (from p in context.Members
                                        join c in context.Countries on p.COUNTRY equals c.CountryName
                                        where ((p.DATE_REGISTERED >= thisOneStartMonth && p.DATE_REGISTERED < thisOneEndMonth) ||
                                        (p.DATE_REGISTERED >= thisTwoStartMonth && p.DATE_REGISTERED < thisTwoEndMonth)
                                        || (p.DATE_REGISTERED.Year == thisCurrentMonth.Year && p.DATE_REGISTERED.Month <= thisCurrentMonth.Month))
                                        && (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                                        && c.StoreID == storeId && p.SOURCE.StartsWith("eStore")
                                        && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                        group p by new { p.DATE_REGISTERED.Year, p.DATE_REGISTERED.Month }
                                            into g
                                            select new
                                            {
                                                Year = g.Key.Year,
                                                Month = g.Key.Month,
                                                Count = g.Count()
                                            }).Union(
                                            (from p in context.Members
                                             join c in context.Countries on p.COUNTRY equals c.CountryName
                                             where ((p.DATE_REGISTERED >= lastOneStartMonth && p.DATE_REGISTERED < lastOneEndMonth) ||
                                             (p.DATE_REGISTERED >= lastTwoStartMonth && p.DATE_REGISTERED < lastTwoEndMonth)
                                             || (p.DATE_REGISTERED.Year == lastCurrentMonth.Year && p.DATE_REGISTERED.Month <= lastCurrentMonth.Month))
                                             && (p.COUNTRY == c.CountryName || p.COUNTRY == c.Shorts)
                                             && c.StoreID == storeId && p.SOURCE.StartsWith("eStore")
                                             && !p.EMAIL_ADDR.Contains("@advantech.") && !p.EMAIL_ADDR.Contains("@damez.com")
                                             group p by new { p.DATE_REGISTERED.Year, p.DATE_REGISTERED.Month }
                                                 into g
                                                 select new
                                                 {
                                                     Year = g.Key.Year,
                                                     Month = g.Key.Month,
                                                     Count = g.Count()
                                                 })
                                        ).ToList();

            foreach (DashBoardReport item in registerDashBoardList)
            {
                int registerYoYCount = 0;
                if (item.DisplayPartno.Equals("YTM"))
                {
                    item.CustomersAmount = currentRegisterFlash.Where(p => p.Year == item.ReportDate.Year).Sum(p => p.Count);
                    registerYoYCount = currentRegisterFlash.Where(p => p.Year == item.ReportDate.AddYears(-1).Year).Sum(p => p.Count);
                }
                else
                {
                    var registerFlash = currentRegisterFlash.FirstOrDefault(p => p.Year == item.ReportDate.Year && p.Month == item.ReportDate.Month);
                    if (registerFlash != null)
                        item.CustomersAmount = registerFlash.Count;

                    DateTime lastDt = item.ReportDate.AddYears(-1);//去年
                    registerFlash = currentRegisterFlash.FirstOrDefault(p => p.Year == lastDt.Year && p.Month == lastDt.Month);
                    if (registerFlash != null)
                        registerYoYCount = registerFlash.Count;
                }

                if (registerYoYCount != 0)
                {
                    if (item.CustomersAmount == 0)
                        item.LocalAmount = decimal.Round((0 - registerYoYCount) * 100, 2);
                    else
                        item.LocalAmount = decimal.Round(((item.CustomersAmount / registerYoYCount) - 1) * 100, 2);
                }
                else
                {
                    if (item.CustomersAmount == 0)
                        item.LocalAmount = 0;
                    else
                        item.LocalAmount = decimal.Round(item.CustomersAmount * 100, 2);
                }

                item.FlashColumnTwo = item.CustomersAmount;
                item.FlashColumnThree = item.LocalAmount + "%";
                item.FlashColumnFour = "";
            }

            #endregion

            //只取 Confirm状态.
            #region Quotation(last 3 months)
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Quotation(last 3 months)";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "Quotation Amount";
            flashItem.HeaderNameThree = "# of Quotation";
            flashItem.HeaderNameFour = "Quotation Avg Amt";

            //currentMonth
            registerDashBoardList = genarateFlashDaskBoardList(thisTwoStartMonth, thisOneStartMonth, thisCurrentMonth);
            flashItem.DashBoardList = registerDashBoardList;

            var currentQuotationFlash = (from p in context.Quotations
                                         where ((p.QuoteDate >= thisOneStartMonth && p.QuoteDate < thisOneEndMonth) ||
                                                        (p.QuoteDate >= thisTwoStartMonth && p.QuoteDate < thisTwoEndMonth)
                                                        || (p.QuoteDate.Value.Year == thisCurrentMonth.Year && p.QuoteDate.Value.Month <= thisCurrentMonth.Month))
                                                        && p.StoreID == storeId && POCOS.Quotation.SuccessfullyStatus.Contains(p.Status)
                                                        && !p.UserID.Contains("@advantech.") && !p.UserID.Contains("@damez.com")
                                         group p by new { p.QuoteDate.Value.Year, p.QuoteDate.Value.Month }
                                             into g
                                             select new
                                             {
                                                 Year = g.Key.Year,
                                                 Month = g.Key.Month,
                                                 Count = g.Count(),
                                                 Amount = g.Sum(p => p.TotalAmount)
                                             }
                                        ).ToList();

            foreach (DashBoardReport item in registerDashBoardList)
            {
                if (item.DisplayPartno.Equals("YTM"))
                {
                    item.CustomersAmount = currentQuotationFlash.Where(p => p.Year == item.ReportDate.Year).Sum(p => p.Amount.Value);
                    item.LocalAmount = currentQuotationFlash.Where(p => p.Year == item.ReportDate.Year).Sum(p => p.Count);
                }
                else
                {
                    var quotationFlash = currentQuotationFlash.FirstOrDefault(p => p.Year == item.ReportDate.Year && p.Month == item.ReportDate.Month);
                    if (quotationFlash != null)
                    {
                        item.CustomersAmount = quotationFlash.Amount.Value;
                        item.LocalAmount = quotationFlash.Count;
                    }
                }
                item.CustomersAmount = decimal.Round(item.CustomersAmount, 2);
                item.Avg = decimal.Round(item.CustomersAmount / (item.LocalAmount == 0 ? 1 : item.LocalAmount), 2);

                item.FlashColumnTwo = item.CustomersAmount;
                item.FlashColumnThree = item.LocalAmount;
                item.FlashColumnFour = item.Avg;
            }
            #endregion

            ////排除 Confirm状态.
            #region Abandoned Quotation(last 3 months)
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Abandoned Quotation(last 3 months)";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "Quotation Amount";
            flashItem.HeaderNameThree = "# of Quotation";
            flashItem.HeaderNameFour = "Quotation Avg Amt";

            //currentMonth
            registerDashBoardList = genarateFlashDaskBoardList(thisTwoStartMonth, thisOneStartMonth, thisCurrentMonth);
            flashItem.DashBoardList = registerDashBoardList;

            var currentNoPassQuotationFlashOne = (from p in context.Quotations
                                                  join c in context.Carts on p.CartID equals c.CartID
                                                  where ((p.QuoteDate >= thisOneStartMonth && p.QuoteDate < thisOneEndMonth) ||
                                                            (p.QuoteDate >= thisTwoStartMonth && p.QuoteDate < thisTwoEndMonth)
                                                            || (p.QuoteDate.Value.Year == thisCurrentMonth.Year && p.QuoteDate.Value.Month <= thisCurrentMonth.Month))
                                                            && p.StoreID == storeId && !POCOS.Quotation.SuccessfullyStatus.Contains(p.Status) && c.BilltoID != null
                                                            && !p.UserID.Contains("@advantech.") && !p.UserID.Contains("@damez.com")
                                                  select new
                                                  {
                                                      Year = p.QuoteDate.Value.Year,
                                                      Month = p.QuoteDate.Value.Month,
                                                      TotalAmount = c.TotalAmount != null ? c.TotalAmount : 0,
                                                      Freight = p.Freight != null ? p.Freight : 0,
                                                      Tax = p.Tax != null ? p.Tax : 0,
                                                      DutyAndTax = p.DutyAndTax != null ? p.DutyAndTax : 0,
                                                      TotalDiscount = p.TotalDiscount != null ? p.TotalDiscount : 0
                                                  }
                                        ).ToList();

            //如果Quotation.TotalAmount 就走Cart.TotalAmount里面计算.
            var currentNoPassQuotationFlashTwo = (from p in currentNoPassQuotationFlashOne
                                                  select new
                                                  {
                                                      Year = p.Year,
                                                      Month = p.Month,
                                                      TotalAmount = p.TotalAmount + p.Freight + p.Tax + p.DutyAndTax - p.TotalDiscount
                                                  }).ToList();

            var currentNoPassQuotationFlash = (from p in currentNoPassQuotationFlashTwo
                                               group p by new { p.Year, p.Month }
                                                   into g
                                                   select new
                                                   {
                                                       Year = g.Key.Year,
                                                       Month = g.Key.Month,
                                                       Count = g.Count(),
                                                       Amount = g.Sum(p => p.TotalAmount)
                                                   }).ToList();

            //var currentNoPassQuotationFlash = (from p in context.Quotations
            //                             where ((p.QuoteDate >= thisOneStartMonth && p.QuoteDate < thisOneEndMonth) ||
            //                                            (p.QuoteDate >= thisTwoStartMonth && p.QuoteDate < thisTwoEndMonth)
            //                                            || p.QuoteDate.Value.Year == thisCurrentMonth.Year)
            //                                            && p.StoreID == storeId && !POCOS.Quotation.SuccessfullyStatus.Contains(p.Status) 
            //                                            && !p.UserID.Contains("@advantech.") && !p.UserID.Contains("@damez.com") 
            //                             group p by new { p.QuoteDate.Value.Year, p.QuoteDate.Value.Month }
            //                                 into g
            //                                 select new
            //                                 {
            //                                     Year = g.Key.Year,
            //                                     Month = g.Key.Month,
            //                                     Count = g.Count(),
            //                                     Amount = g.Sum(p => p.TotalAmount)
            //                                 }
            //                            ).ToList();

            foreach (DashBoardReport item in registerDashBoardList)
            {
                if (item.DisplayPartno.Equals("YTM"))
                {
                    item.CustomersAmount = currentNoPassQuotationFlash.Where(p => p.Year == item.ReportDate.Year).Sum(p => p.Amount.HasValue ? p.Amount.Value : 0);
                    item.LocalAmount = currentNoPassQuotationFlash.Where(p => p.Year == item.ReportDate.Year).Sum(p => p.Count);
                }
                else
                {
                    var quotationFlash = currentNoPassQuotationFlash.FirstOrDefault(p => p.Year == item.ReportDate.Year && p.Month == item.ReportDate.Month);
                    if (quotationFlash != null)
                    {
                        item.CustomersAmount = quotationFlash.Amount.HasValue ? quotationFlash.Amount.Value : 0;
                        item.LocalAmount = quotationFlash.Count;
                    }
                }
                item.CustomersAmount = decimal.Round(item.CustomersAmount, 2);
                item.Avg = decimal.Round(item.CustomersAmount / (item.LocalAmount == 0 ? 1 : item.LocalAmount), 2);

                item.FlashColumnTwo = item.CustomersAmount;
                item.FlashColumnThree = item.LocalAmount;
                item.FlashColumnFour = item.Avg;
            }
            #endregion

            //order 只取 success list状态
            #region Revenue  (last 3 months)
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Revenue  (last 3 months)";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "Order Amount";
            flashItem.HeaderNameThree = "# of Order";
            flashItem.HeaderNameFour = "Order Avg Amt";

            //currentMonth
            registerDashBoardList = genarateFlashDaskBoardList(thisTwoStartMonth, thisOneStartMonth, thisCurrentMonth);
            flashItem.DashBoardList = registerDashBoardList;

            var currentOrderFlash = (from p in context.Orders
                                     where ((p.OrderDate >= thisOneStartMonth && p.OrderDate < thisOneEndMonth) ||
                                                        (p.OrderDate >= thisTwoStartMonth && p.OrderDate < thisTwoEndMonth)
                                                        || (p.OrderDate.Value.Year == thisCurrentMonth.Year && p.OrderDate.Value.Month <= thisCurrentMonth.Month))
                                                        && p.StoreID == storeId && orderSuccessfullyStatus.Contains(p.OrderStatus)
                                                        && !p.UserID.Contains("@advantech.") && !p.UserID.Contains("@damez.com")
                                     group p by new { p.OrderDate.Value.Year, p.OrderDate.Value.Month }
                                         into g
                                         select new
                                         {
                                             Year = g.Key.Year,
                                             Month = g.Key.Month,
                                             Count = g.Count(),
                                             Amount = g.Sum(p => p.TotalAmount)
                                         }
                                        ).ToList();

            foreach (DashBoardReport item in registerDashBoardList)
            {
                if (item.DisplayPartno.Equals("YTM"))
                {
                    item.CustomersAmount = currentOrderFlash.Where(p => p.Year == item.ReportDate.Year).Sum(p => p.Amount.Value);
                    item.LocalAmount = currentOrderFlash.Where(p => p.Year == item.ReportDate.Year).Sum(p => p.Count);
                }
                else
                {
                    var orderFlash = currentOrderFlash.FirstOrDefault(p => p.Year == item.ReportDate.Year && p.Month == item.ReportDate.Month);
                    if (orderFlash != null)
                    {
                        item.CustomersAmount = orderFlash.Amount.HasValue ? orderFlash.Amount.Value : 0;
                        item.LocalAmount = orderFlash.Count;
                    }
                }
                item.CustomersAmount = decimal.Round(item.CustomersAmount, 2);
                item.Avg = decimal.Round(item.CustomersAmount / (item.LocalAmount == 0 ? 1 : item.LocalAmount), 2);

                item.FlashColumnTwo = item.CustomersAmount;
                item.FlashColumnThree = item.LocalAmount;
                item.FlashColumnFour = item.Avg;
            }
            #endregion

            //第一次下单的新用户 /  老用户
            #region Revenue first order / old order
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Revenue First-order/Non-first-order  (last 3 months)";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "First-order / Non-first-order %<br>of Order Amount";
            flashItem.HeaderNameThree = "First-order / Non-first-order %<br>of # of Order(%)";
            flashItem.HeaderNameFour = "Amt YoY%";

            //currentMonth
            registerDashBoardList = genarateFlashDaskBoardList(thisTwoStartMonth, thisOneStartMonth, thisCurrentMonth, false);
            flashItem.DashBoardList = registerDashBoardList;

            var firstNoneOrderList = (from o in context.Orders
                                      let c = context.Orders.Count(p => p.StoreID == storeId && orderSuccessfullyStatus.Contains(p.OrderStatus)
                                          && p.UserID == o.UserID && !p.UserID.Contains("@advantech.") && !p.UserID.Contains("@damez.com")
                                          && ((p.OrderDate.Value.Year == o.OrderDate.Value.Year && p.OrderDate.Value.Month < o.OrderDate.Value.Month) ||
                                                 p.OrderDate.Value.Year < o.OrderDate.Value.Year))
                                      where ((o.OrderDate.Value.Year == thisCurrentMonth.Year && o.OrderDate.Value.Month == thisCurrentMonth.Month) ||
                                            (o.OrderDate >= thisOneStartMonth && o.OrderDate < thisOneEndMonth) ||
                                            (o.OrderDate >= thisTwoStartMonth && o.OrderDate < thisTwoEndMonth))
                                            && o.StoreID == storeId && orderSuccessfullyStatus.Contains(o.OrderStatus)
                                            && !o.UserID.Contains("@advantech.") && !o.UserID.Contains("@damez.com")
                                      select new
                                      {
                                          UserId = o.UserID,
                                          Amount = o.TotalAmount.HasValue ? o.TotalAmount : 0,
                                          Date = o.OrderDate.HasValue ? o.OrderDate : thisCurrentMonth,
                                          isFirstOrder = c == 0
                                      }
                            ).Union(
                                from o in context.Orders
                                let c = context.Orders.Count(p => p.StoreID == storeId && orderSuccessfullyStatus.Contains(p.OrderStatus)
                                    && p.UserID == o.UserID && !p.UserID.Contains("@advantech.") && !p.UserID.Contains("@damez.com")
                                    && ((p.OrderDate.Value.Year == o.OrderDate.Value.Year && p.OrderDate.Value.Month < o.OrderDate.Value.Month) ||
                                           p.OrderDate.Value.Year < o.OrderDate.Value.Year))
                                where ((o.OrderDate.Value.Year == lastCurrentMonth.Year && o.OrderDate.Value.Month == lastCurrentMonth.Month) ||
                                      (o.OrderDate >= lastOneStartMonth && o.OrderDate < lastOneEndMonth) ||
                                      (o.OrderDate >= lastTwoStartMonth && o.OrderDate < lastTwoEndMonth))
                                      && o.StoreID == storeId && orderSuccessfullyStatus.Contains(o.OrderStatus)
                                      && !o.UserID.Contains("@advantech.") && !o.UserID.Contains("@damez.com")
                                select new
                                {
                                    UserId = o.UserID,
                                    Amount = o.TotalAmount.HasValue ? o.TotalAmount : 0,
                                    Date = o.OrderDate.HasValue ? o.OrderDate : thisCurrentMonth,
                                    isFirstOrder = c == 0
                                }
                            ).ToList();

            foreach (var item in registerDashBoardList)
            {
                if (firstNoneOrderList.Count > 0)
                {
                    //第一下单用户的总金额
                    var firstOrderAmountFlash = firstNoneOrderList.Where(p => p.Date.Value.Year == item.ReportDate.Year && p.Date.Value.Month == item.ReportDate.Month
                                            && p.isFirstOrder).Sum(p => p.Amount.Value);
                    //第二下单用户的总金额
                    var firstNonOrderAmountFlash = firstNoneOrderList.Where(p => p.Date.Value.Year == item.ReportDate.Year && p.Date.Value.Month == item.ReportDate.Month
                                            && !p.isFirstOrder).Sum(p => p.Amount.Value);
                    decimal firstAmountTotalAmount = firstOrderAmountFlash + firstNonOrderAmountFlash;
                    if (firstAmountTotalAmount > 0)
                        item.FlashColumnTwo = decimal.Round((firstOrderAmountFlash * 100 / firstAmountTotalAmount), 2) + "% / " + decimal.Round((firstNonOrderAmountFlash * 100 / firstAmountTotalAmount), 2) + "%";
                    else
                        item.FlashColumnTwo = "0% / 0%";

                    //第一次下单的用户数量
                    var firstOrderCountFlash = firstNoneOrderList.Where(p => p.Date.Value.Year == item.ReportDate.Year && p.Date.Value.Month == item.ReportDate.Month
                                            && p.isFirstOrder).Count();
                    //第二次下单的用户数量
                    var firstNonOrderCountFlash = firstNoneOrderList.Where(p => p.Date.Value.Year == item.ReportDate.Year && p.Date.Value.Month == item.ReportDate.Month
                                            && !p.isFirstOrder).Count();

                    int firstTotalCount = firstOrderCountFlash + firstNonOrderCountFlash;
                    if (firstTotalCount > 0)
                        item.FlashColumnThree = decimal.Round((firstOrderCountFlash * 100 / firstTotalCount), 2) + "% / " + decimal.Round((firstNonOrderCountFlash * 100 / firstTotalCount), 2) + "%";
                    else
                        item.FlashColumnThree = "0% / 0%";

                    DateTime lastDt = item.ReportDate.AddYears(-1);//去年
                    //去年第一次下单用户的总金额
                    var lastAmountTotalAmount = firstNoneOrderList.Where(p => p.Date.Value.Year == lastDt.Year && p.Date.Value.Month == lastDt.Month).Sum(p => p.Amount.Value);
                    if (firstAmountTotalAmount > 0 && lastAmountTotalAmount > 0)
                        item.FlashColumnFour = decimal.Round(((firstAmountTotalAmount / lastAmountTotalAmount) - 1) * 100, 2) + "%";
                    else
                        item.FlashColumnFour = "0%";
                }
                else
                {
                    item.FlashColumnTwo = "0% / 0%";
                    item.FlashColumnThree = "0% / 0%";
                    item.FlashColumnFour = "0%";
                }
            }
            #endregion

            //order 只取 failed list状态
            #region Abandoned Cart (last 3 months)
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Abandoned Cart (last 3 months)";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "Amount";
            flashItem.HeaderNameThree = "Qty";
            flashItem.HeaderNameFour = "Avg Amt";

            //currentMonth
            registerDashBoardList = genarateFlashDaskBoardList(thisTwoStartMonth, thisOneStartMonth, thisCurrentMonth, false);
            flashItem.DashBoardList = registerDashBoardList;
            //把Cart.totalAmount也带出来.  防止Order.TotalAmount没数据.
            var currentAbandonedFlashOne = (from p in context.Orders
                                            join c in context.Carts on p.CartID equals c.CartID
                                            where ((p.OrderDate >= thisOneStartMonth && p.OrderDate < thisOneEndMonth) ||
                                                               (p.OrderDate >= thisTwoStartMonth && p.OrderDate < thisTwoEndMonth)
                                                               || (p.OrderDate.Value.Year == thisCurrentMonth.Year && p.OrderDate.Value.Month == thisCurrentMonth.Month))
                                                               && p.StoreID == storeId && orderNoPassStatus.Contains(p.OrderStatus) && c.BilltoID != null
                                                               && !p.UserID.Contains("@advantech.") && !p.UserID.Contains("@damez.com")
                                            select new
                                            {
                                                Year = p.OrderDate.Value.Year,
                                                Month = p.OrderDate.Value.Month,
                                                TotalAmount = c.TotalAmount != null ? c.TotalAmount : 0,
                                                Freight = p.Freight != null ? p.Freight : 0,
                                                Tax = p.Tax != null ? p.Tax : 0,
                                                DutyAndTax = p.DutyAndTax != null ? p.DutyAndTax : 0,
                                                Surcharge = p.Surcharge != null ? p.Surcharge : 0,
                                                TotalDiscount = p.TotalDiscount != null ? p.TotalDiscount : 0
                                            }
                                        ).ToList();

            //如果Order.TotalAmount 就走Cart.TotalAmount里面计算.
            var currentAbandonedFlashTwo = (from p in currentAbandonedFlashOne
                                            select new
                                            {
                                                Year = p.Year,
                                                Month = p.Month,
                                                TotalAmount = p.TotalAmount + p.Freight + p.Tax + p.DutyAndTax + p.Surcharge - p.TotalDiscount
                                            }).ToList();

            var currentAbandonedFlash = (from p in currentAbandonedFlashTwo
                                         group p by new { p.Year, p.Month }
                                             into g
                                             select new
                                             {
                                                 Year = g.Key.Year,
                                                 Month = g.Key.Month,
                                                 Count = g.Count(),
                                                 Amount = g.Sum(p => p.TotalAmount)
                                             }).ToList();

            //var currentAbandonedFlash = (from p in context.Orders
            //                         where ((p.OrderDate >= thisOneStartMonth && p.OrderDate < thisOneEndMonth) ||
            //                                            (p.OrderDate >= thisTwoStartMonth && p.OrderDate < thisTwoEndMonth)
            //                                            || (p.OrderDate.Value.Year == thisCurrentMonth.Year && p.OrderDate.Value.Month == thisCurrentMonth.Month))
            //                                            && p.StoreID == storeId && orderNoPassStatus.Contains(p.OrderStatus)
            //                                            && !p.UserID.Contains("@advantech.") && !p.UserID.Contains("@damez.com") 
            //                         group p by new { p.OrderDate.Value.Year, p.OrderDate.Value.Month }
            //                             into g
            //                             select new
            //                             {
            //                                 Year = g.Key.Year,
            //                                 Month = g.Key.Month,
            //                                 Count = g.Count(),
            //                                 Amount = g.Sum(p => p.TotalAmount)
            //                             }
            //                            ).ToList();

            foreach (DashBoardReport item in registerDashBoardList)
            {
                var orderFlash = currentAbandonedFlash.FirstOrDefault(p => p.Year == item.ReportDate.Year && p.Month == item.ReportDate.Month);
                if (orderFlash != null)
                {
                    item.CustomersAmount = orderFlash.Amount.HasValue ? orderFlash.Amount.Value : 0;
                    item.LocalAmount = orderFlash.Count;
                }
                item.CustomersAmount = decimal.Round(item.CustomersAmount, 2);
                item.Avg = decimal.Round(item.CustomersAmount / (item.LocalAmount == 0 ? 1 : item.LocalAmount), 2);

                item.FlashColumnTwo = item.CustomersAmount;
                item.FlashColumnThree = item.LocalAmount;
                item.FlashColumnFour = item.Avg;
            }
            #endregion

            #region Conversion Rate  (last 3 months)
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Conversion Rate  (last 3 months)";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "Registration  Conversion  Rate%";
            flashItem.HeaderNameThree = "Qutation Conversion  Rate%";
            flashItem.HeaderNameFour = "Shopping cart Conversion  Rate%";

            //currentMonth
            registerDashBoardList = genarateFlashDaskBoardList(thisTwoStartMonth, thisOneStartMonth, thisCurrentMonth, false);
            flashItem.DashBoardList = registerDashBoardList;

            var convertRegisrerToOrder = (from o in context.Orders
                                          join m in context.Members on o.UserID equals m.EMAIL_ADDR
                                          where (((o.OrderDate >= thisOneStartMonth && o.OrderDate < thisOneEndMonth) && (m.DATE_REGISTERED >= thisOneStartMonth && m.DATE_REGISTERED < thisOneEndMonth))
                                          || ((o.OrderDate >= thisTwoStartMonth && o.OrderDate < thisTwoEndMonth) && (m.DATE_REGISTERED >= thisTwoStartMonth && m.DATE_REGISTERED < thisTwoEndMonth))
                                          || ((o.OrderDate.Value.Year == thisCurrentMonth.Year && o.OrderDate.Value.Month == thisCurrentMonth.Month) &&
                                             (m.DATE_REGISTERED.Year == thisCurrentMonth.Year && m.DATE_REGISTERED.Month == thisCurrentMonth.Month)))
                                          && o.StoreID == storeId && orderSuccessfullyStatus.Contains(o.OrderStatus)
                                          && m.SOURCE.StartsWith("eStore") //走eStore 注册的人.
                                          && !o.UserID.Contains("@advantech.") && !o.UserID.Contains("@damez.com")
                                          group o by new { o.OrderDate.Value.Year, o.OrderDate.Value.Month }
                                              into g
                                              select new
                                              {
                                                  Year = g.Key.Year,
                                                  Month = g.Key.Month,
                                                  Count = g.Count()
                                              }
                                        ).ToList();
            //elad@htsol.com
            var convertQuotationToOrder = (from o in context.Orders
                                           join q in context.Quotations on o.Source equals q.QuotationNumber
                                           where (((o.OrderDate >= thisOneStartMonth && o.OrderDate < thisOneEndMonth) && (q.QuoteDate >= thisOneStartMonth && q.QuoteDate < thisOneEndMonth))
                                                               || ((o.OrderDate >= thisTwoStartMonth && o.OrderDate < thisTwoEndMonth) && (q.QuoteDate >= thisTwoStartMonth && q.QuoteDate < thisTwoEndMonth))
                                                               || ((o.OrderDate.Value.Year == thisCurrentMonth.Year && o.OrderDate.Value.Month == thisCurrentMonth.Month) &&
                                                                  (q.QuoteDate.Value.Year == thisCurrentMonth.Year && q.QuoteDate.Value.Month == thisCurrentMonth.Month)))
                                           && o.StoreID == storeId && q.StoreID == storeId && orderSuccessfullyStatus.Contains(o.OrderStatus)
                                           && !o.UserID.Contains("@advantech.") && !o.UserID.Contains("@damez.com")
                                           group o by new { o.OrderDate.Value.Year, o.OrderDate.Value.Month }
                                               into g
                                               select new
                                               {
                                                   Year = g.Key.Year,
                                                   Month = g.Key.Month,
                                                   Count = g.Count()
                                               }
                                          ).ToList();
            //获取上面已经 拿到的注册用户数量
            POCOS.FlashReport registerFlashReprot = flashList.FirstOrDefault(p => p.Title == "Registrant (last 3 months)");
            POCOS.FlashReport quotationFlashReprot = flashList.FirstOrDefault(p => p.Title == "Quotation(last 3 months)");
            foreach (DashBoardReport item in registerDashBoardList)
            {
                //获取对应月份 注册数量
                if (registerFlashReprot != null)
                {
                    var registerFlash = registerFlashReprot.DashBoardList.FirstOrDefault(p => p.ReportDate.Year == item.ReportDate.Year && p.ReportDate.Month == item.ReportDate.Month);
                    if (registerFlash != null)
                        item.CustomersAmount = registerFlash.CustomersAmount;
                }
                if (quotationFlashReprot != null)
                {
                    var quotationFlash = quotationFlashReprot.DashBoardList.FirstOrDefault(p => p.ReportDate.Year == item.ReportDate.Year && p.ReportDate.Month == item.ReportDate.Month);
                    if (quotationFlash != null)
                        item.LocalAmount = quotationFlash.LocalAmount;
                }


                var convertOrder = convertRegisrerToOrder.FirstOrDefault(p => p.Year == item.ReportDate.Year && p.Month == item.ReportDate.Month);
                if (convertOrder != null)
                {
                    if (item.CustomersAmount != 0)
                        item.CustomersAmount = decimal.Round((convertOrder.Count * 100) / item.CustomersAmount, 2);
                }
                else
                    item.CustomersAmount = 0;

                var convertQuotationOrder = convertQuotationToOrder.FirstOrDefault(p => p.Year == item.ReportDate.Year && p.Month == item.ReportDate.Month);
                if (convertQuotationOrder != null)
                {
                    if (item.LocalAmount != 0)
                        item.LocalAmount = decimal.Round((convertQuotationOrder.Count * 100) / item.LocalAmount, 2);
                }
                else
                    item.LocalAmount = 0;

                item.FlashColumnTwo = item.CustomersAmount + "%";
                item.FlashColumnThree = item.LocalAmount + "%";
                item.FlashColumnFour = "0" + "%";
            }

            #endregion

            //当月 Abandoned Cart最多的3个产品
            #region Top 3 product in Abandoned Cart
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Top 3 product in Abandoned Cart (Month:" + thisCurrentMonth.Month + ")";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "Product P/N";
            flashItem.HeaderNameThree = "Qty";
            flashItem.HeaderNameFour = "Amount";

            registerDashBoardList = new List<DashBoardReport>();
            flashItem.DashBoardList = registerDashBoardList;

            var dashBoardPartToOrder = (from a in context.Orders
                                        join b in context.Carts on a.CartID equals b.CartID
                                        join c in context.CartItems on new { b.CartID, b.StoreID } equals new { c.CartID, c.StoreID }
                                        join d in context.Parts on c.SProductID equals d.SProductID
                                        where orderNoPassStatus.Contains(a.OrderStatus) && a.StoreID == storeId
                                             && a.StoreID == b.StoreID && b.StoreID == c.StoreID
                                             && c.StoreID == d.StoreID //&& a.TotalAmount != null
                                             && a.OrderDate.Value.Year == thisCurrentMonth.Year && a.OrderDate.Value.Month == thisCurrentMonth.Month
                                             && !a.UserID.Contains("@advantech.") && !a.UserID.Contains("@damez.com")
                                             && string.IsNullOrEmpty(c.BTOConfigID) //只取product 不取ctos
                                             && !d.SProductID.StartsWith("19") && !d.SProductID.StartsWith("20") && !d.SProductID.StartsWith("17")
                                             && !d.SProductID.StartsWith("OPTIONS") && !d.SProductID.StartsWith("AGS") && !d.SProductID.StartsWith("SQF")
                                        select new
                                        {
                                            DisplayPartno = d.SProductID,
                                            Cnt = c.Qty,
                                            LocalAmount = c.AdjustedPrice
                                        }).GroupBy(p => p.DisplayPartno).Select(p => new { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();

            var dashBoardCtosToOrder = (from a in context.Orders
                                        join b in context.Carts on a.CartID equals b.CartID
                                        join c in context.CartItems on new { b.CartID, b.StoreID } equals new { c.CartID, c.StoreID }
                                        join h in context.BTOSystems on c.BTOConfigID equals h.BTOConfigID
                                        join e in context.BTOSConfigs on h.BTOConfigID equals e.BTOConfigID
                                        join f in context.BTOSConfigDetails on new { e.BTOConfigID, e.ConfigID } equals new { f.BTOConfigID, f.ConfigID }
                                        join d in context.Parts on f.SProductID equals d.SProductID
                                        where orderNoPassStatus.Contains(a.OrderStatus)
                                            && a.StoreID == storeId && a.StoreID == b.StoreID
                                            && a.StoreID == d.StoreID //&& a.TotalAmount != null
                                            && a.OrderDate.Value.Year == thisCurrentMonth.Year && a.OrderDate.Value.Month == thisCurrentMonth.Month
                                            && !a.UserID.Contains("@advantech.") && !a.UserID.Contains("@damez.com")
                                            && !string.IsNullOrEmpty(c.BTOConfigID) //只取ctos 不取product
                                            && !d.SProductID.StartsWith("19") && !d.SProductID.StartsWith("20") && !d.SProductID.StartsWith("17")
                                            && !d.SProductID.StartsWith("OPTIONS") && !d.SProductID.StartsWith("AGS") && !d.SProductID.StartsWith("SQF")
                                        select new
                                        {
                                            DisplayPartno = d.SProductID,
                                            Cnt = c.Qty * e.Qty * f.Qty,
                                            LocalAmount = c.Qty * e.Qty * f.Qty * f.AdjustedPrice.Value
                                        }).GroupBy(p => p.DisplayPartno).Select(p => new { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList(); ;
            dashBoardPartToOrder.AddRange(dashBoardCtosToOrder);
            dashBoardPartToOrder = dashBoardPartToOrder.GroupBy(p => p.DisplayPartno).Select(p => new { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).OrderByDescending(p => p.LocalAmount).Take(3).ToList();
            //dashBoardPartToOrder = dashBoardPartToOrder.GroupBy(p => p.DisplayPartno).Select(p => new { DisplayPartno = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).OrderByDescending(p => p.LocalAmount).ToList();

            for (int i = 0; i < dashBoardPartToOrder.Count; i++)
            {
                POCOS.DashBoardReport excampleReport = new POCOS.DashBoardReport();
                excampleReport.DisplayPartno = "Top" + (i + 1);
                excampleReport.Source = dashBoardPartToOrder[i].DisplayPartno;
                excampleReport.LocalAmount = dashBoardPartToOrder[i].Cnt;
                excampleReport.Avg = decimal.Round(dashBoardPartToOrder[i].LocalAmount, 2);

                excampleReport.FlashColumnTwo = excampleReport.Source;
                excampleReport.FlashColumnThree = excampleReport.LocalAmount;
                excampleReport.FlashColumnFour = excampleReport.Avg;

                registerDashBoardList.Add(excampleReport);
            }

            #endregion

            //当月 内部用户 Quotation 金额最多的 3个人.  
            #region eStore Qutation by eSales
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "eStore Qutation by eSales (Month:" + thisCurrentMonth.Month + ")";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "eSales";
            flashItem.HeaderNameThree = "Qty";
            flashItem.HeaderNameFour = "Amount";

            registerDashBoardList = new List<DashBoardReport>();
            flashItem.DashBoardList = registerDashBoardList;

            var currentQuotationbySales = (from p in context.Quotations
                                           where p.QuoteDate.Value.Year == thisCurrentMonth.Year && p.QuoteDate.Value.Month == thisCurrentMonth.Month
                                           && p.StoreID == storeId && POCOS.Quotation.SuccessfullyStatus.Contains(p.Status)
                                                 && (p.UserID.Contains("@advantech.") || p.UserID.Contains("@damez.com"))
                                           group p by p.UserID
                                               into g
                                               select new
                                               {
                                                   UserId = g.Key,
                                                   Count = g.Count(),
                                                   Amount = g.Sum(p => p.TotalAmount)
                                               }
                                        ).OrderByDescending(p => p.Amount).Take(3).ToList();

            for (int i = 0; i < currentQuotationbySales.Count; i++)
            {
                POCOS.DashBoardReport excampleReport = new POCOS.DashBoardReport();
                excampleReport.DisplayPartno = "Top" + (i + 1);
                excampleReport.Source = currentQuotationbySales[i].UserId;
                excampleReport.LocalAmount = currentQuotationbySales[i].Count;
                excampleReport.Avg = currentQuotationbySales[i].Amount.HasValue ? decimal.Round(currentQuotationbySales[i].Amount.Value, 2) : 0;

                excampleReport.FlashColumnTwo = excampleReport.Source;
                excampleReport.FlashColumnThree = excampleReport.LocalAmount;
                excampleReport.FlashColumnFour = excampleReport.Avg;

                registerDashBoardList.Add(excampleReport);
            }
            #endregion

            //当月 内部用户 Order 金额最多的 3个人.  
            #region eStore Order by eSales
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "eStore Order by eSales (Month:" + thisCurrentMonth.Month + ")";
            flashItem.HeaderNameOne = "";
            flashItem.HeaderNameTwo = "eSales";
            flashItem.HeaderNameThree = "Qty";
            flashItem.HeaderNameFour = "Amount";

            registerDashBoardList = new List<DashBoardReport>();
            flashItem.DashBoardList = registerDashBoardList;

            var currentOrderbySales = (from p in context.Orders
                                       where p.OrderDate.Value.Year == thisCurrentMonth.Year && p.OrderDate.Value.Month == thisCurrentMonth.Month
                                            && p.StoreID == storeId && orderSuccessfullyStatus.Contains(p.OrderStatus)
                                            && (p.UserID.Contains("@advantech.") || p.UserID.Contains("@damez.com"))
                                       group p by p.UserID
                                           into g
                                           select new
                                           {
                                               UserId = g.Key,
                                               Count = g.Count(),
                                               Amount = g.Sum(p => p.TotalAmount)
                                           }
                                        ).OrderByDescending(p => p.Amount).Take(3).ToList();

            for (int i = 0; i < currentOrderbySales.Count; i++)
            {
                POCOS.DashBoardReport excampleReport = new POCOS.DashBoardReport();
                excampleReport.DisplayPartno = "Top" + (i + 1);
                excampleReport.Source = currentOrderbySales[i].UserId;
                excampleReport.LocalAmount = currentOrderbySales[i].Count;
                excampleReport.Avg = currentOrderbySales[i].Amount.HasValue ? decimal.Round(currentOrderbySales[i].Amount.Value, 2) : 0;

                excampleReport.FlashColumnTwo = excampleReport.Source;
                excampleReport.FlashColumnThree = excampleReport.LocalAmount;
                excampleReport.FlashColumnFour = excampleReport.Avg;

                registerDashBoardList.Add(excampleReport);
            }
            #endregion

            //今年 外部用户 下订单最多的10个人
            #region TOP 10 Customer
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "TOP 10 Customer (Year:" + thisCurrentMonth.Year + ")";
            flashItem.HeaderNameOne = "ID/Email";
            flashItem.HeaderNameTwo = "Company";
            flashItem.HeaderNameThree = "Revenue";
            flashItem.HeaderNameFour = "Remarks";

            registerDashBoardList = new List<DashBoardReport>();
            flashItem.DashBoardList = registerDashBoardList;

            var currentCustomerBoard = (from o in context.Orders
                                        join u in context.Users on o.UserID equals u.UserID
                                        where o.StoreID == storeId && (o.OrderDate.Value.Year == thisCurrentMonth.Year && o.OrderDate.Value.Month <= thisCurrentMonth.Month)
                                        && orderSuccessfullyStatus.Contains(o.OrderStatus)
                                        && !o.UserID.Contains("@advantech.") && !o.UserID.Contains("@damez.com")
                                        group o by new { o.UserID, u.CompanyName }
                                            into g
                                            select new
                                            {
                                                UserId = g.Key.UserID,
                                                CompanyName = g.Key.CompanyName,
                                                Amount = g.Sum(p => p.TotalAmount)
                                            }
                                        ).OrderByDescending(p => p.Amount).Take(10).ToList();

            for (int i = 0; i < currentCustomerBoard.Count; i++)
            {
                POCOS.DashBoardReport excampleReport = new POCOS.DashBoardReport();
                excampleReport.DisplayPartno = currentCustomerBoard[i].UserId;
                excampleReport.Source = currentCustomerBoard[i].CompanyName;
                excampleReport.LocalAmount = currentCustomerBoard[i].Amount.HasValue ? decimal.Round(currentCustomerBoard[i].Amount.Value, 2) : 0;
                excampleReport.Avg = 0;

                excampleReport.FlashColumnTwo = excampleReport.Source;
                excampleReport.FlashColumnThree = excampleReport.LocalAmount;
                excampleReport.FlashColumnFour = "";

                registerDashBoardList.Add(excampleReport);
            }
            #endregion

            #region Product Line Performance (Month)

            genarateProductLineToFlash(flashList, thisCurrentMonth, storeId, orderSuccessfullyStatus, false);

            #endregion

            #region Product Line Performance (Year)

            genarateProductLineToFlash(flashList, thisCurrentMonth, storeId, orderSuccessfullyStatus, true);

            #endregion


            return flashList;
        }

        private void genarateProductLineToFlash(List<POCOS.FlashReport> flashList, DateTime thisCurrentMonth, string storeId, List<string> orderSuccessfullyStatus, bool isYear)
        {
            #region Product Line Performance (Month/Year)
            POCOS.FlashReport flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            string dateTitle = isYear ? "Year:" : "Month:";
            int dateYear = isYear ? thisCurrentMonth.Year : thisCurrentMonth.Month;
            flashItem.Title = "Product Line Performance (" + dateTitle + dateYear + ")";
            flashItem.HeaderNameOne = "Product Line";
            flashItem.HeaderNameTwo = "Qty";
            flashItem.HeaderNameThree = "Amount";
            flashItem.HeaderNameFour = "Amount%";

            List<DashBoardReport> registerDashBoardList = new List<DashBoardReport>();
            flashItem.DashBoardList = registerDashBoardList;

            var productLineMonthPartToOrder = (from a in context.Orders
                                               join b in context.Carts on a.CartID equals b.CartID
                                               join c in context.CartItems on new { b.CartID, b.StoreID } equals new { c.CartID, c.StoreID }
                                               join d in context.Parts on c.SProductID equals d.SProductID
                                               where orderSuccessfullyStatus.Contains(a.OrderStatus) && a.StoreID == storeId
                                                    && a.StoreID == b.StoreID && b.StoreID == c.StoreID
                                                    && c.StoreID == d.StoreID && a.TotalAmount != null
                                                    && !a.UserID.Contains("@advantech.") && !a.UserID.Contains("@damez.com")
                                                    && (isYear ? a.OrderDate.Value.Year == thisCurrentMonth.Year : a.OrderDate.Value >= thisCurrentMonth)
                                                    && string.IsNullOrEmpty(c.BTOConfigID) //只取product 不取ctos
                                                    && !string.IsNullOrEmpty(d.VendorProductLine)
                                               select new
                                               {
                                                   DispalyLine = d.VendorProductLine,
                                                   DisplayPartno = d.SProductID,
                                                   Cnt = c.Qty,
                                                   LocalAmount = c.AdjustedPrice
                                               }).GroupBy(p => new { p.DispalyLine, p.DisplayPartno }).Select(p => new { DispalyLine = p.Key.DispalyLine, DisplayPartno = p.Key.DisplayPartno, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();

            var productLineMonthCtosToOrder = (from a in context.Orders
                                               join b in context.Carts on a.CartID equals b.CartID
                                               join c in context.CartItems on new { b.CartID, b.StoreID } equals new { c.CartID, c.StoreID }
                                               join h in context.BTOSystems on c.BTOConfigID equals h.BTOConfigID
                                               join e in context.BTOSConfigs on h.BTOConfigID equals e.BTOConfigID
                                               join f in context.BTOSConfigDetails on new { e.BTOConfigID, e.ConfigID } equals new { f.BTOConfigID, f.ConfigID }
                                               join d in context.Parts on f.SProductID equals d.SProductID
                                               where orderSuccessfullyStatus.Contains(a.OrderStatus)
                                                   && a.StoreID == storeId && a.StoreID == b.StoreID
                                                   && a.StoreID == d.StoreID && a.TotalAmount != null
                                                   && !a.UserID.Contains("@advantech.") && !a.UserID.Contains("@damez.com")
                                                   && (isYear ? a.OrderDate.Value.Year == thisCurrentMonth.Year : a.OrderDate.Value >= thisCurrentMonth)
                                                   && !string.IsNullOrEmpty(c.BTOConfigID) //只取ctos 不取product
                                               select new
                                               {
                                                   DispalyLine = d.VendorProductLine,
                                                   DisplayPartno = d.SProductID,
                                                   Cnt = c.Qty * e.Qty * f.Qty,
                                                   LocalAmount = c.Qty * e.Qty * f.Qty * f.AdjustedPrice.Value
                                               }).GroupBy(p => new { p.DispalyLine, p.DisplayPartno }).Select(p => new { DispalyLine = p.Key.DispalyLine, DisplayPartno = p.Key.DisplayPartno, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();
            productLineMonthPartToOrder.AddRange(productLineMonthCtosToOrder);
            productLineMonthPartToOrder = productLineMonthPartToOrder.GroupBy(p => new { p.DispalyLine, p.DisplayPartno }).Select(p => new { DispalyLine = p.Key.DispalyLine, DisplayPartno = p.Key.DisplayPartno, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).ToList();

            var productLineOrder = productLineMonthPartToOrder.GroupBy(p => p.DispalyLine).Select(p => new { DispalyLine = p.Key, Cnt = p.Sum(g => g.Cnt), LocalAmount = p.Sum(g => g.LocalAmount) }).OrderByDescending(p => p.LocalAmount).Take(10).ToList();

            if (productLineOrder.Count > 0)
            {
                decimal productLineTotalAmount = decimal.Round(productLineOrder.Sum(p => p.LocalAmount), 2);
                POCOS.DashBoardReport excampleReport = new POCOS.DashBoardReport();
                for (int i = 0; i < productLineOrder.Count; i++)
                {
                    excampleReport = new POCOS.DashBoardReport();
                    excampleReport.DisplayPartno = productLineOrder[i].DispalyLine;
                    excampleReport.CustomersAmount = productLineOrder[i].Cnt;
                    excampleReport.LocalAmount = decimal.Round(productLineOrder[i].LocalAmount, 2);
                    if (excampleReport.LocalAmount > 0)
                        excampleReport.Avg = decimal.Round((excampleReport.LocalAmount / productLineTotalAmount) * 100, 2);

                    excampleReport.FlashColumnTwo = excampleReport.CustomersAmount;
                    excampleReport.FlashColumnThree = excampleReport.LocalAmount;
                    excampleReport.FlashColumnFour = excampleReport.Avg + "%";

                    registerDashBoardList.Add(excampleReport);
                }

                excampleReport = new POCOS.DashBoardReport();
                excampleReport.DisplayPartno = "Total";
                excampleReport.CustomersAmount = productLineOrder.Sum(p => p.Cnt);
                excampleReport.LocalAmount = productLineTotalAmount;
                excampleReport.Avg = 100;

                excampleReport.FlashColumnTwo = excampleReport.CustomersAmount;
                excampleReport.FlashColumnThree = excampleReport.LocalAmount;
                excampleReport.FlashColumnFour = excampleReport.Avg + "%";

                registerDashBoardList.Add(excampleReport);
            }
            #endregion

            #region Top 3  products in Product Line (Month)
            flashItem = new POCOS.FlashReport();
            flashList.Add(flashItem);
            flashItem.Title = "Top 3  products in Product Line (" + dateTitle + dateYear + ")";
            flashItem.HeaderNameOne = "Product Line";
            flashItem.HeaderNameTwo = "Product P/N";
            flashItem.HeaderNameThree = "Qty";
            flashItem.HeaderNameFour = "Amount";

            registerDashBoardList = new List<DashBoardReport>();
            flashItem.DashBoardList = registerDashBoardList;
            //产品线大于数量 0
            if (productLineOrder.Count > 0)
            {
                for (int i = 0; i < productLineOrder.Count; i++)
                {
                    var partNoListByLine = productLineMonthPartToOrder.Where(p => p.DispalyLine == productLineOrder[i].DispalyLine).OrderByDescending(p => p.LocalAmount).Take(3).ToList();
                    foreach (var item in partNoListByLine)
                    {
                        POCOS.DashBoardReport excampleReport = new POCOS.DashBoardReport();
                        excampleReport.DisplayPartno = productLineOrder[i].DispalyLine;
                        excampleReport.Source = item.DisplayPartno;
                        excampleReport.CustomersAmount = item.Cnt;
                        excampleReport.LocalAmount = decimal.Round(item.LocalAmount, 2);

                        excampleReport.FlashColumnTwo = excampleReport.Source;
                        excampleReport.FlashColumnThree = excampleReport.CustomersAmount;
                        excampleReport.FlashColumnFour = excampleReport.LocalAmount;

                        registerDashBoardList.Add(excampleReport);
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Others
        private static string myclassname()
        {
            return typeof(UserActivityHelper).ToString();
        }
        #endregion
    }
}
