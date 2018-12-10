using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eStore.Presentation.UrlRewriting
{
    public class RewriteBaseModule : System.Web.IHttpModule
    {
       public void Init(HttpApplication app)
        {
            app.AuthorizeRequest += this.BaseModuleRewriter_AuthorizeRequest;
        }

        public void Dispose()
        {
        }
        public void BaseModuleRewriter_AuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            Rewrite(app.Request.Path, app);
        }
        public virtual void Rewrite(string requestedPath, HttpApplication app)
        {
        }

    }
}
