using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{
    public class DateHelper    {
            

        /// <summary>
        /// 获取多少天后的日期(去除休息日和节假日)
        /// </summary>
        /// <param name="today">当前日期</param>
        /// <param name="days">相隔天数</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime getAvailableDate(DateTime today, int days, string storeid)
        {
            DateTime endDate = new DateTime(today.Year,today.Month ,today.Day);
            int _addDate = days; //增加天数
            
          
            
            int i = 1;
            List<DateTime> dateList = getHolidays(storeid);

            while (1 == 1)
            {
                endDate = endDate.AddDays(1);
                int parthIndex = dateList.IndexOf(endDate);
                if (parthIndex != -1)
                {
                    _addDate = _addDate + 1;
                }
                else
                {
                     
                    if (endDate.DayOfWeek == DayOfWeek.Saturday   || endDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        _addDate = _addDate + 1;
                    }
                }
                if (i >= _addDate)
                {
                    break; // TODO: might not be correct. Was : Exit Do
                }
                i = i + 1;
            }
            return endDate;
        }

        /// <summary>
        /// Get Holidays by Store
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        private List<DateTime> getHolidays(string storeid)
        {
            eStore.POCOS.eStore3Entities6 context = new eStore.POCOS.eStore3Entities6();

            List<DateTime> _holidays = (from h in context.Holidays
                                        where h.StoreID == storeid
                                        select h.HolidayDate).ToList();

            return _holidays;
        }

    }
}
