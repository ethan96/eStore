using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.PocoX;

namespace eStore.POCOS
{
    public partial class DashBoardReport
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DashBoardReport()
        {
        }

        #region Register Properties

        public string DisplayPartno { get; set; }
        public string ABCInd { get; set; }
        public string ShowSource
        {
            get;
            set;
        }

        public Nullable<int> RegisterCount
        {
            get;
            set;
        }
        public string State { get; set; }

        private string _StateX;
        public string StateX 
        { 
            get
            {
                if (string.IsNullOrEmpty(_StateX))
                    _StateX = State ?? string.Empty;
                return _StateX.Trim();
            }
            set
            {
                _StateX = value;
            }
        }
      
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        #endregion

        #region Primitive Properties

        public string Type
        {
            get;
            set;
        }

        public string StoreID
        {
            get;
            set;
        }

        public string Store
        {
            get;
            set;
        }

        public string YYYYM
        {
            get;
            set;
        }

        public decimal LocalAmount
        {
            get;
            set;
        }

        public string CountryName
        {
            get;
            set;
        }

        public string Country
        {
            get;
            set;
        }

        public string DMF
        {
            get;
            set;
        }

        public string Source
        {
            get;
            set;
        }

        public string LOB
        {
            get;
            set;
        }

        public string Assignee
        {
            get;
            set;
        }

        public string UserID
        {
            get;
            set;
        }

        public DateTime ReportDate { get; set; }

        #endregion

        #region Primitive Properties
        //sum count 总数量
        public int Cnt { get; set; }
        //Avg   平均值
        public decimal Avg { get; set; }

        //Order
        //from Quotation Count
        public int FromQuotationCnt { get; set; }

        //Quotation
        //Inside Sales Amount 内部销售金额
        public decimal InsideSalesAmount { get; set; }
        //Customers Amount 用户销售金额
        public decimal CustomersAmount { get; set; }
        //内部销售数量
        public int InsideSalesCount { get; set; }
        //用户销售数量
        public int CustomersCount { get; set; }
        #endregion

        #region Flash Report
        public object FlashColumnTwo { get; set; }

        public object FlashColumnThree { get; set; }

        public object FlashColumnFour { get; set; }
        #endregion
    }

    public class FlashReport
    {
        #region Default
        public string Title { get; set; }

        public string HeaderNameOne { get; set; }

        public string HeaderNameTwo { get; set; }

        public string HeaderNameThree { get; set; }

        public string HeaderNameFour { get; set; }
        #endregion

        public List<DashBoardReport> DashBoardList { get; set; }
        
    }
}
