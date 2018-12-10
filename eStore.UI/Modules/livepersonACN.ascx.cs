using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class livepersonACN : livepersonBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RectangleHotSpot rhsQQ = new RectangleHotSpot();
            RectangleHotSpot rhsCall= new RectangleHotSpot();
            RectangleHotSpot rhsQuotation = new RectangleHotSpot();

            rhsQQ.NavigateUrl = "javascript:showQQAPI();";

            rhsCall.NavigateUrl = ResolveUrl( "~/ContactUS.aspx");
            rhsQuotation.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=sales");
            if (UserLargerImage)
            {
                imglivepersonACN.ImageUrl = ResolveUrl("~/Images/livepersonACN.jpg");
                imgwebaccess.ImageUrl = ResolveUrl("~/Images/webaccess.jpg");
                imglivepersonACN.Height = 180;
                imglivepersonACN.Width = 208;
                rhsQQ.Top = 60;
                rhsQQ.Left = 12;
                rhsQQ.Right = 198;
                rhsQQ.Bottom = 93;

                rhsCall.Top = 100;
                rhsCall.Left = 12;
                rhsCall.Right = 198;
                rhsCall.Bottom = 133;

                rhsQuotation.Top =140 ;
                rhsQuotation.Left = 12;
                rhsQuotation.Right = 198;
                rhsQuotation.Bottom = 173;
            }
            else
            {
                imglivepersonACN.ImageUrl = ResolveUrl("~/Images/livepersonACN-s.jpg");
                imgwebaccess.ImageUrl = ResolveUrl("~/Images/webaccess-s.jpg");
                imglivepersonACN.Height = 164;
                imglivepersonACN.Width = 190;
                rhsQQ.Top = 57;
                rhsQQ.Left = 11;
                rhsQQ.Right = 182;
                rhsQQ.Bottom = 85;

                rhsCall.Top = 94;
                rhsCall.Left = 11;
                rhsCall.Right = 182;
                rhsCall.Bottom = 121;

                rhsQuotation.Top = 128;
                rhsQuotation.Left = 11;
                rhsQuotation.Right = 182;
                rhsQuotation.Bottom = 157;
            }
            imglivepersonACN.HotSpots.Clear();
            imglivepersonACN.HotSpots.Add(rhsQQ);
            imglivepersonACN.HotSpots.Add(rhsCall);
            imglivepersonACN.HotSpots.Add(rhsQuotation);
            imglivepersonACN.HotSpotMode = HotSpotMode.Navigate;
        }
    }
}