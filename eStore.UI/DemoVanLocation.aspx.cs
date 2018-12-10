using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Artem.Google.UI;
using System.Xml;
using System.IO;

namespace eStore.UI
{
    public partial class DemoVanLocation : eStore.Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GoogleMap1.Address = "台湾台北市";

                string xmlAddress = System.Configuration.ConfigurationManager.AppSettings.Get("UserCertificateFiles") + "/Map/GoogleMap.xml";
                XmlDocument doc = new XmlDocument();
                #region 加载xml
                if (File.Exists(xmlAddress))
                {
                    try
                    {
                        doc.Load(xmlAddress);
                        if (doc.HasChildNodes)
                        {
                            XmlNode rootNode = doc.SelectSingleNode("Root");
                            if (rootNode != null && rootNode.HasChildNodes)
                            {
                                XmlNode mapTypeNode = rootNode.SelectSingleNode("MapType");
                                if (mapTypeNode != null)
                                    GoogleMap1.MapType = (MapType)Enum.Parse(typeof(MapType), mapTypeNode.InnerText);

                                XmlNode latitudeNode = rootNode.SelectSingleNode("Latitude");
                                XmlNode longitudeNode = rootNode.SelectSingleNode("Longitude");
                                XmlNode addressNode = rootNode.SelectSingleNode("Address");
                                if (latitudeNode != null && longitudeNode != null 
                                        && !string.IsNullOrEmpty(latitudeNode.InnerText) && !string.IsNullOrEmpty(longitudeNode.InnerText))
                                {
                                    GoogleMap1.Center.Latitude = double.Parse(latitudeNode.InnerText);
                                    GoogleMap1.Center.Longitude = double.Parse(longitudeNode.InnerText);
                                }
                                else if (addressNode != null && !string.IsNullOrEmpty(addressNode.InnerText))
                                {
                                    GoogleMap1.Center = null;
                                    GoogleMap1.Address = addressNode.InnerText;
                                }
                                //GoogleMap1.EnableStreetViewControl = true;
                                XmlNode titleNode = rootNode.SelectSingleNode("Title");
                                if (titleNode != null)
                                    GoogleMap1.ToolTip = titleNode.InnerText;

                                XmlNode zoomNode = rootNode.SelectSingleNode("Zoom");
                                if (zoomNode != null && !string.IsNullOrEmpty(zoomNode.InnerText))
                                    GoogleMap1.Zoom = int.Parse(zoomNode.InnerText);

                                XmlNode enableScrollZoomNode = rootNode.SelectSingleNode("EnableScrollZoom");
                                if (enableScrollZoomNode != null && !string.IsNullOrEmpty(enableScrollZoomNode.InnerText))
                                    GoogleMap1.EnableScrollWheelZoom = bool.Parse(enableScrollZoomNode.InnerText);

                                XmlNode markerNode = rootNode.SelectSingleNode("Marker");
                                if (markerNode != null && markerNode.HasChildNodes)
                                {
                                    //节点清除
                                    GoogleMap1.Markers.Clear();
                                    XmlNodeList markerList = markerNode.ChildNodes;
                                    List<GooleMapClass> googleList = new List<GooleMapClass>();
                                    foreach (XmlNode item in markerList)
                                    {
                                        if (!string.IsNullOrEmpty(item.InnerText))
                                        {
                                            string[] pair = item.InnerText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                            if (pair.Length == 2)
                                            {
                                                GooleMapClass gm = new GooleMapClass();
                                                gm.Marker = item.InnerText;
                                                gm.StartDate = item.Attributes["StartDate"] == null ? DateTime.Now : DateTime.Parse(item.Attributes["StartDate"].Value);
                                                gm.EndDate = item.Attributes["EndDate"] == null ? DateTime.Now : DateTime.Parse(item.Attributes["EndDate"].Value);
                                                gm.MarkerAddress = item.Attributes["Address"].Value;
                                                gm.MarkerInfo = item.Attributes["Info"].Value;
                                                googleList.Add(gm);
                                            }
                                        }
                                    }

                                    googleList = googleList.OrderBy(p=> p.StartDate).ToList();

                                    GooleMapClass currentMap = googleList.FirstOrDefault(p=>p.StartDate<=DateTime.Now && p.EndDate >= DateTime.Now);
                                    List<LatLng> latLngList = new List<LatLng>();
                                    GooglePolyline gp = new GooglePolyline();
                                    foreach (GooleMapClass item in googleList)
                                    {
                                        string[] pair = item.Marker.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (pair.Length == 2)
                                        {
                                            Marker newMarker = new Marker();
                                            newMarker.Position.Latitude = double.Parse(pair[0]);
                                            newMarker.Position.Longitude = double.Parse(pair[1]);
                                            newMarker.Title = item.MarkerAddress;
                                            newMarker.Info = item.MarkerInfo;
                                            newMarker.Icon = new MarkerImage().Url = "/images/google-red-dot.png";
                                            if (currentMap != null && currentMap.Marker == item.Marker)
                                            {
                                                newMarker.Animation = MarkerAnimation.Bounce;
                                                newMarker.Icon = new MarkerImage().Url = "/images/google-marker_greenA.png";
                                                newMarker.AutoOpen = true;
                                            }
                                            GoogleMap1.Markers.Add(newMarker);

                                            latLngList.Add(new LatLng(newMarker.Position.Latitude,newMarker.Position.Longitude));
                                        }
                                    }
                                    if (latLngList.Count > 0)
                                    {
                                        gp.Path = latLngList;
                                        gp.StrokeWeight = 10;
                                        gp.StrokeColor = System.Drawing.Color.Red;
                                        GoogleMap1.Polylines.Add(gp);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {}
                }
                #endregion
            }
        }
    }

    public class GooleMapClass
    {
        public string Marker { get; set; }

        public string MarkerAddress { get; set; }

        public string MarkerInfo { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}