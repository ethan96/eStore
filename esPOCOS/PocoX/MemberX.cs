using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.PocoX;

namespace eStore.POCOS
{
    /// <summary>
    /// In this file, It defines extended runtime member profile prooperties and methods
    /// </summary>
    public partial class Member : FollowUpable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Member()
        {
        }

        /// <summary>
        /// New Column in Membership Report,and mark the intested products with "EA","EP" or "EmbCore"
        /// </summary>
        private Dictionary<string, LoBType> inProductList = new Dictionary<string, LoBType>() 
        {
            {"Communication and Networking Platforms",LoBType.EP},
            {"Digital Signage Solutions",LoBType.EP},
            {"Display Panels and Panel Computers",LoBType.EP},
            {"Embedded Fanless Computers",LoBType.EP},
            {"Industrial Computers",LoBType.EP},
            {"Medical Computers",LoBType.EP},
            {"Mobile Computing",LoBType.EP},
            {"Automation Controllers and Software",LoBType.EA},
            {"Data Acquisition (DAQ) and Control",LoBType.EA},
            {"Digital Video Card and Software",LoBType.EA},
            {"Digital Video Recording Platforms",LoBType.EA},
            {"Embedded Automation Computers",LoBType.EA},
            {"Human Machine Interface and HMI Software",LoBType.EA},
            {"Industrial Communication",LoBType.EA},
            {"Remote I/O",LoBType.EA},
            {"Smart Home Appliance",LoBType.EA},
            {"Embedded Single Board Computers",LoBType.EmbCore},
            {"Embedded Software Customization Service",LoBType.EmbCore}
        };

        //将'and'与'&'统一,没有出现在Dictionary<string, LoBType>的数据,全部标记成'EP'
        private LoBType _LoB;
        public LoBType LoB 
        {
            get 
            {
                if (!string.IsNullOrEmpty(this.IN_PRODUCT))
                {
                    //统一大小写、空格数目不同、大小写的问题
                    KeyValuePair<string, LoBType> cc = inProductList.FirstOrDefault(c => this.IN_PRODUCT.ToLower().Replace(" ", "").Replace("&", "and").Contains(c.Key.ToLower().Replace(" ", "")));
                    if (cc.Key!=null)
                        return cc.Value;
                    else
                        return LoBType.EP;
                }
                else
                    _LoB = LoBType.EP;
                return _LoB; 
            }
            set { _LoB = value; }
        }

        private List<string> _intestProduct;

        public List<string> IntestProduct
        {
            get 
            {
                List<string> cc = inProductList.Select(p=>p.Key).ToList();
                _intestProduct = cc;
                return _intestProduct; 
            }
            set { _intestProduct = value; }
        }

        public enum LoBType { EA, EP, EmbCore }//marks:EA,EP,EmbCore

        private string _activityType;
        public string ActivityType
        {
            get
            {
                if (string.IsNullOrEmpty(_activityType))
                {
                    POCOS.DAL.UserHelper helper = new DAL.UserHelper();
                    helper.getMemberActiveType(this);
                }
                return _activityType;
            }
            set { _activityType = value; }
        }

        private string _activityDesc;
        public string ActivityDesc
        {
            get
            {
                if (string.IsNullOrEmpty(_activityDesc))
                {
                    POCOS.DAL.UserHelper helper = new DAL.UserHelper();
                    helper.getMemberActiveType(this);
                }
                return _activityDesc;
            }
            set { _activityDesc = value; }
        }

        public string StoreID { get; set; }

        public virtual string IS_NEW_CONTACT
        {
            get;
            set;
        }

        public virtual string SECTOR_BU
        {
            get;
            set;
        }

#region OM
        //The following methods are for FollowUp used in OM
        protected override String getInstanceID()     {  return this.EMAIL_ADDR;      }
        protected override String getInstanceStoreID() { return this.StoreID; }
        protected override String getInstanceOwner() { return this.EMAIL_ADDR; }
        protected override TrackingLog.TrackType getTrackType() { return TrackingLog.TrackType.LEADS; }
#endregion OM
    }

    public class MemberLog
    {
        public string EMAIL_ADDR { get; set; }

        public string ACTIVITY_TYPE { get; set; }

        public string ACTIVITY_DESC3 { get; set; }
    }
}
