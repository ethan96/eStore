<%@ WebHandler Language="C#" Class="captcha" %>

using System;
using System.Web;
using System.Drawing;
using System.Web.SessionState;

public class captcha : IHttpHandler, IRequiresSessionState
{

   
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "image/jpeg";
        CatpchaImage captcha = new CatpchaImage();
        string str = captcha.DrawNumbers(5);
        eStore.Presentation.eStoreContext.Current.verificationCode = str;
        Bitmap bmp = captcha.Result;
        bmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
    }
 
    
    
    public bool IsReusable {
        get {
            return true;
        }
    }

}