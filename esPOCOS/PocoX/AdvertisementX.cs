using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace eStore.POCOS
{
    public partial class Advertisement : TaggingEnabled<Advertisement>
    {
        #region Tagging
        public override object EntityKey
        {
            get
            {
                return this.id;
            }

        }
        public override string EntityStoreId
        {
            get
            {
                return this.StoreID;
            }
        }

        #endregion
        public enum AdvertisementType {
             AllianceAdv,
             CenterPopup,
             EducationColumn, 
             HomeBanner, 
             SliderBanner,
             Reference, 
             RelatedLink, 
             SeeWhatsNew, 
             SpecialAdv,
             StoreAds,
             TodayHighLight, 
             TodaysDeals,
             TopSliderDown,
             NotSpecified,
             Floating,
             Expanding, 
             Resources,
             GoldenEggs,
             Rate,
             FullColumnBanner,
            LiveEngageSection
        };

         /// <summary>
         /// This property allows use to get and set last order status
         /// </summary>
         public virtual AdvertisementType segmentType
         {
             get
             {
                 if (!String.IsNullOrEmpty(this.AdType) && stringDictionary.ContainsKey(this.AdType))
                     return (AdvertisementType)stringDictionary[this.AdType];
                 else
                     return AdvertisementType.NotSpecified;
             }

             set { this.AdType = enumDictionary[value]; }
         }

         /*
          * private AdvertisementType _segmentType = AdvertisementType.NotSpecified;
          public AdvertisementType segmentType
          {
              get
              {
                  if (_segmentType == AdvertisementType.NotAvailable)
                  {
                      switch (AdType)
                      {
                          case "StoreAds":
                              _segmentType = AdvertisementType.StoreAds;
                              break;
                          case "AllianceAdv":
                              _segmentType = AdvertisementType.AllianceAdv;
                              break;
                          case "EducationColumn":
                              _segmentType = AdvertisementType.EducationColumn;
                              break;
                          case "HomeBanner":
                              _segmentType = AdvertisementType.HomeBanner;
                              break;
                          case "Reference":
                              _segmentType = AdvertisementType.Reference;
                              break;
                          case "RelatedLink":
                              _segmentType = AdvertisementType.RelatedLink;
                              break;
                          case "SeeWhatsNew":
                              _segmentType = AdvertisementType.SeeWhatsNew;
                              break;
                          case "SpecialAdv":
                              _segmentType = AdvertisementType.SpecialAdv;
                              break;
                          case "TodayHighLight":
                              _segmentType = AdvertisementType.TodayHighLight;
                              break;
                          case "TodaysDeals":
                              _segmentType = AdvertisementType.TodaysDeals;
                              break;
                          case "CenterPopup":
                              _segmentType = AdvertisementType.CenterPopup;
                              break;
                          case "TopSliderDown":
                              _segmentType = AdvertisementType.TopSliderDown;
                              break;

                      }
                  }

                  return _segmentType;
              }
          }
          */

         /// <summary>
        /// Before we can come out a better weighting algorithm, the segment weights will be predefined here.
        /// Today's deals : 100
        /// 
        /// </summary>
         public Int32 segmentWeight
         {
             get
             {
                 if (_segmentWeight == 0)
                 {
                     switch (segmentType)
                     {  case AdvertisementType.LiveEngageSection:
                         case AdvertisementType.Resources:
                             _segmentWeight = 1100;
                             break;
                         case AdvertisementType.StoreAds:
                             _segmentWeight = 1000;
                             break;
                         case AdvertisementType.TodaysDeals:
                             _segmentWeight = 900;
                             break;
                         case AdvertisementType.TodayHighLight:
                             _segmentWeight = 800;
                             break;
                         case AdvertisementType.SpecialAdv:
                             _segmentWeight = 700;
                             break;
                         case AdvertisementType.Floating:
                         case AdvertisementType.Expanding:
                         case AdvertisementType.CenterPopup:
                         case AdvertisementType.TopSliderDown:
                             _segmentWeight = 500;
                             break;
                         default:
                             _segmentWeight = -1000;    //to disqualify this type of adBanner
                             break;
                     }
                 }

                 return _segmentWeight;
             }
         }
         private Int32 _segmentWeight = 0;

        /// <summary>
        /// This property provides ranking reference.  The bigger the weight is, the higher priority the banner has.
        /// </summary>
         public Int32 weight
         {
             get
             {
                 if (_weight == 0)
                     _weight = (segmentWeight + (Int32)Weight.GetValueOrDefault());

                 return _weight;
             }

             set { _weight = value; }
         }
         private Int32 _weight = 0;


         private static Dictionary<String, AdvertisementType> _stringDictionary = null;
         private static Dictionary<AdvertisementType, String> _enumDictionary = null;
         private Dictionary<AdvertisementType, String> enumDictionary
         {
             get
             {
                 if (_enumDictionary == null)
                     initDictionary();

                 return _enumDictionary;
             }
         }

         public bool isInActivity
         {
             get
             {
                 return Publish && StartDate <= DateTime.Now && EndDate >= DateTime.Now;
             }
         }

         private Dictionary<String, AdvertisementType> stringDictionary
         {
             get
             {
                 if (_stringDictionary == null)
                     initDictionary();

                 return _stringDictionary;
             }
         }

         private void initDictionary()
         {
             _enumDictionary = new Dictionary<AdvertisementType, string>();
             _stringDictionary = new Dictionary<string, AdvertisementType>();

             foreach (int value in Enum.GetValues(typeof(AdvertisementType)))
             {
                 AdvertisementType status = (AdvertisementType)value;
                 String name = Enum.GetName(typeof(AdvertisementType), status);
                 _enumDictionary.Add(status, name);
                 _stringDictionary.Add(name, status);
             }
         }

         private string _imageFileHost;
         public string imageFileHost
        {
            get
            {
                if (_imageFileHost == null)
                {
                    if (!string.IsNullOrEmpty(Imagefile))
                        _imageFileHost = Imagefile.StartsWith("http", true, null) ? 
                                        Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), Imagefile);
                    else
                        _imageFileHost = "";
                }
                return _imageFileHost;
            }
        }

         private string _imagefileX;
         public string imagefileX
         {
             get
             {
                 if (_imagefileX == null)
                 {
                     if (!string.IsNullOrEmpty(Imagefile))
                         _imagefileX = Imagefile.StartsWith("http", true, null) ?
                                         Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), Imagefile);
                     else
                         _imagefileX = "";
                 }
                 return _imagefileX;
             }
         }
         public virtual string smallImagefileX
         {
             get
             {
                 if (this.segmentType == AdvertisementType.FullColumnBanner)
                 {
                     if (!string.IsNullOrEmpty(imagefileX) && imagefileX.Contains("."))
                     {
                         return imagefileX.Insert(imagefileX.LastIndexOf('.'), "_S");
                     }
                     else
                         return string.Empty;
                 }
                 else
                 { 
                     return imagefileX;
                 }
              
             }
         }

         private string _htmlContentX;
         public string htmlContentX
         {
             get
             {
                 if (_htmlContentX == null)
                 {
                     _htmlContentX = "";
                     if (complexAdvertisementContent != null)
                        _htmlContentX = complexAdvertisementContent.getHtmlContent();
                     else
                         _htmlContentX = HtmlContent;
                 }
                 return _htmlContentX;
             }
            
         }
         ComplexAdvertisementContent _complexAdvertisementContent;
         public ComplexAdvertisementContent complexAdvertisementContent
         {
             get {
                 if (_complexAdvertisementContent == null)
                 {
                     switch (this.AdType)
                     {
                         case "Floating": break;
                         case "Expanding":
                             _complexAdvertisementContent = new ExpandingContent(this.StoreID);
                             reloadcomplexAdvertisementContent();
                             break;
                        case "Resources":
                            _complexAdvertisementContent = new ResourcesContent(this.StoreID);
                            reloadcomplexAdvertisementContent();
                            break;
                        case "LiveEngageSection":
                            _complexAdvertisementContent = new LiveEngageSection(this.StoreID);
                            reloadcomplexAdvertisementContent();
                            break;
                        default:
                            _complexAdvertisementContent = null;
                            break;
                    }
                }
                 return _complexAdvertisementContent;
             }
             set {
                 if (value != null)
                 {
                     _complexAdvertisementContent = value;
                     uploadcomplexAdvertisementContent();
                 }
             }
         }

         public void reloadcomplexAdvertisementContent()
         {
             if (_complexAdvertisementContent != null && string.IsNullOrEmpty(HtmlContent) == false)
             {
                 _complexAdvertisementContent = _complexAdvertisementContent.deserialize(HtmlContent);
                 _complexAdvertisementContent.Advid = this.id.ToString();
             }
         }
         public void uploadcomplexAdvertisementContent()
         {
             if (_complexAdvertisementContent != null )
             {
                 HtmlContent = _complexAdvertisementContent.serialize();
             }
         }

         public void cacheToMemory()
         {
             CachePool.getInstance().cacheAdvertisement(this);
         }

         public Advertisement getAdvertisementFromCache()
         {
             var adv = CachePool.getInstance().getAdvertisement(this.StoreID, this.id);
             if (adv != null)
                 return adv;
             else
                 return null;
         }

         public  abstract class ComplexAdvertisementContent
         {
             public string StoreID { get; set; }
             private POCOS.Store _store;
             protected POCOS.Store Store
             {
                 get
                 {
                     if (_store == null)
                     {
                         if (!string.IsNullOrEmpty(StoreID))
                             _store = (new DAL.StoreHelper()).getStorebyStoreid(this.StoreID);

                     }
                     else if (!string.IsNullOrEmpty(StoreID) && _store.StoreID != StoreID)
                     {
                         _store = (new DAL.StoreHelper()).getStorebyStoreid(this.StoreID);
                     }
                     return _store;
                 }
             }

             public ComplexAdvertisementContent() { }
             public ComplexAdvertisementContent(string storeid)
             {
                 this.StoreID = storeid;
             }

             public string htmlContext = string.Empty;

             public virtual string getHtmlContent() {
                 return string.Empty;
             }

             public virtual List<int> getSiteMapContent()
             {
                 return new List<int>();
             }

             public string Advid {get;set;}

             public string serialize()
             {
                 StringWriter stringWriter = new StringWriter();
                 XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
                 xmlSerializer.Serialize(stringWriter, this);
                 string serializedXML = stringWriter.ToString();
                 return serializedXML;
             }
             public ComplexAdvertisementContent deserialize(string xml)
             {
                 XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
                 StringReader rd = new StringReader(xml);
                 object ad = xmlSerializer.Deserialize(rd);
                 if (ad is ComplexAdvertisementContent)
                     return (ComplexAdvertisementContent)ad;
                 else
                     return null;
             
             }
           
         }
        public class LiveEngageSection : ComplexAdvertisementContent
        {
            public LiveEngageSection()
                : base()
            { }
            public LiveEngageSection(string storeid)
                : base(storeid)
            {

            }

            public List<string> Sections { get; set; }

          

        }
         public class ResourcesContent:ComplexAdvertisementContent 
         {
             public ResourcesContent()
                 : base()
             { }
             public ResourcesContent( string storeid)
                 : base(storeid)
             {

             }
             private string _businessApArea;
             public string BusinessApArea
             {
                 get
                 {
                     return _businessApArea;
                 }
                 set
                 {
                     _businessApArea = value;
                 }
             }
             private List<string> _CMSResourceTypes;
             public List<string> CMSResourceTypes
             {
                 get {
                     if (_CMSResourceTypes == null)
                        _CMSResourceTypes=new List<string>();
                     return _CMSResourceTypes;
                 }
             }
             private List<bool> _CMSResourceTypesIsShow;
             public List<bool> CMSResourceTypesIsShow
             {
                 get
                 {
                     if (_CMSResourceTypesIsShow == null)
                         _CMSResourceTypesIsShow = new List<bool>();
                     return _CMSResourceTypesIsShow;
                 }
                 set
                 {
                     _CMSResourceTypesIsShow = value;
                 }
             }

             private List<string> _pisCategorys = new List<string>();
             public List<string> PisCategorys
             {
                 get { return _pisCategorys; }
                 set { _pisCategorys = value; }
             }

             public bool addCMSResourceType(string type,bool isNeedLogin = false)
             {
                 if (CMSResourceTypes.Contains(type) == false)
                 {
                     CMSResourceTypes.Add(type);
                     CMSResourceTypesIsShow.Add(isNeedLogin);
                     return true;
                 }
                 else
                     return false;
             }

             public List<string> getValidCMSResourceTypes()
             {
                 List<string> valid = new List<string>();
                 foreach (string t in CMSResourceTypes)
                 {
                     try
                     {
                         eStore.POCOS.DAL.CMSType cmstype = DAL.CMSType.NA;
                         Enum.TryParse(t, true, out cmstype);
                         if (cmstype != DAL.CMSType.NA)
                         {
                             eStore.POCOS.DAL.CMSHelper cmsHelper = new eStore.POCOS.DAL.CMSHelper(cmstype, "Customer Zone", this.Store);
                             if (this.PisCategorys != null && this.PisCategorys.Count > 0)
                             {
                                 if (cmsHelper.hasCMS4PisCategory(t, this.PisCategorys))
                                     valid.Add(t);
                             }
                             else
                             {
                                 if (cmsHelper.hasCMS4AllBaa(t, this.BusinessApArea))
                                     valid.Add(t);
                             }
                         }
                     }
                     catch (Exception ex)
                     {
                        eStore.Utilities.eStoreLoger.Error("getValidCMSResourceTypes()", "", "", "", ex);
                     }
                 }
                 return valid;
             }

             public override string getHtmlContent()
             {
                
                 System.Text.StringBuilder sbContent = new System.Text.StringBuilder();
                 List<string> validtype = getValidCMSResourceTypes();
                 if (validtype.Count > 0)
                 {
                     sbContent.AppendFormat("<div class=\"titlebar  ui-helper-reset ui-corner-all\">Resources</div><ul class=\"eStoreList\">");

                     for (int i = 0; i < CMSResourceTypes.Count; i++)
                     {
                         if(validtype.Contains(CMSResourceTypes[i]))
                         {
                             sbContent.AppendFormat("<li><a href=\"/Cms/CMSpager.aspx?advid={1}&item={2}\" target=\"_blank\" {3}>{0}</a></li>"
                                  , CMSResourceTypes[i]
                                  , Advid
                                  ,i
                                  , ""
                                  );
                         }
                     }

                     sbContent.AppendLine("</ul>");

                 }
                 return sbContent.ToString();
             }

             public override List<int> getSiteMapContent()
             {
                 List<int> cmsMapList = new List<int>();

                 List<string> validtype = getValidCMSResourceTypes();
                 if (validtype.Count > 0)
                 {
                     for (int i = 0; i < validtype.Count; i++)
                     {
                         cmsMapList.Add(i);
                     }
                 }
                 return cmsMapList;
             }
         }

         public class ExpandingContent : ComplexAdvertisementContent
         {
             public ExpandingContent() 
                 : base()
             { }

             public ExpandingContent(string storeId)
                 : base(storeId)
             { }

             private List<string> imgList = new List<string>();
             public List<string> ImgList
             {
                 get { return imgList; }
                 set { imgList = value; }
             }

             private List<int> widthList = new List<int>();
             public List<int> WidthList
             {
                 get { return widthList; }
                 set { widthList = value; }
             }

             private List<string> urlList = new List<string>();
             public List<string> UrlList
             {
                 get { return urlList; }
                 set { urlList = value; }
             }

             private List<string> targetList = new List<string>();
             public List<string> TargetList
             {
                 get { return targetList; }
                 set { targetList = value; }
             }

             public void addImage(string imgUrl, int width, string url = "#", string target = "_self")
             {
                 imgList.Add(imgUrl);
                 widthList.Add(width);
                 urlList.Add(url);
                 targetList.Add(target);
             }



             public override string getHtmlContent()
             {
                 System.Text.StringBuilder sb = new System.Text.StringBuilder();
                 sb.Append("<div id='sideads' class='sideads leftsideads'>");
                 sb.Append("<div class='sideadstitle'>");
                 sb.Append(string.Format("<img src='/resource{0}' dwidth='{1}' alt='' /></div>", imgList[0], WidthList[0]));
                 sb.Append("<div class='sideadsstaging1'>");
                 sb.Append(string.Format("<a href='{0}' target='{1}'>",urlList[1],targetList[1]));
                 sb.Append(string.Format("<img src='/resource{0}' dwidth='{1}' alt='' /></a></div>", imgList[1], WidthList[1]));
                 sb.Append("<div class='sideadsstaging2'>");
                 sb.Append(string.Format("<a href='{0}' target='{1}'>", urlList[2], targetList[2]));
                 sb.Append(string.Format("<img src='/resource{0}' dwidth='{1}' alt='' /></a></div>", imgList[2], WidthList[2]));
                 sb.Append("<div id='sideadsstagingclose' class='hiddenitem'><img src='/App_Themes/Default/Wrong.jpg' width='26' height='26' alt='a' /></div></div>");
                 return sb.ToString();
                 
             }

         }

        public Advertisement IsGEOMatched(string countryCode)
        {
            Advertisement adv = null;
            if (this != null)
            {
                switch (countryCode)
                {
                    case "MY":
                        if (string.IsNullOrEmpty(this.Map) || this.Map.ToUpper() == "AMY")
                            adv = this;
                        break;
                    case "ID":
                        if (string.IsNullOrEmpty(this.Map) || this.Map.ToUpper() == "AID")
                            adv = this;
                        break;
                    case "SG":
                        if (string.IsNullOrEmpty(this.Map) || this.Map.ToUpper() == "ASG" || this.Map.ToUpper() == "ROA")
                            adv = this;
                        break;
                    default:
                        if (string.IsNullOrEmpty(this.Map) || this.Map.ToUpper() == "ROA")
                            adv = this;
                        break;
                }
            }
            return adv;
        }

#region OMRegion
         public enum AdvertisementOMType
         {
             StoreAds, FullColumnBanner, HomeBanner, SliderBanner, TodayHighLight,
             CenterPopup, TopSliderDown, EducationColumn,TodaysDeals , Floating, Expanding , Resources, LiveEngageSection, video
         };

         public AdvertisementOMType segmentOMType
         {
             get
             {
                 AdvertisementOMType adType = AdvertisementOMType.StoreAds;
                 switch(AdType)
                 {
                     case "HomeBanner":
                         adType = AdvertisementOMType.HomeBanner;
                         break;
                     case "CenterPopup":
                         adType = AdvertisementOMType.CenterPopup;
                         break;
                     case "TopSliderDown":
                         adType = AdvertisementOMType.TopSliderDown;
                         break;
                     case "EducationColumn":
                         adType = AdvertisementOMType.EducationColumn;
                         break;
                     case "TodayHighLight":
                         adType = AdvertisementOMType.TodayHighLight;
                         break;
                     case "TodaysDeals":
                         adType = AdvertisementOMType.TodaysDeals;
                         break;
                     case "SliderBanner":
                         adType = AdvertisementOMType.SliderBanner;
                         break;
                     case "video":
                         adType = AdvertisementOMType.video;
                         break;

                     case "Floating": //暂时归结在StoreAds中，UI跑 StoreAds 逻辑
                     default:
                         adType = AdvertisementOMType.StoreAds;
                         break;
                 }
                 return adType;
             }
         }
#endregion

    }
}
