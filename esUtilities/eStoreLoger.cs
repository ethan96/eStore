using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Net;
using System.Diagnostics;

namespace eStore.Utilities
{
    /// <summary>
    /// Singleton eStoreLogger
    /// </summary>
   public static class eStoreLoger
   {
       
       /// <summary>
       /// This is a static constructor and will be invoked for once per application server life time.
       /// </summary>
       static eStoreLoger()
       {
           log4net.Config.XmlConfigurator.Configure();
       }

       /// <summary>
       /// This method will return ILog using caller class name + invoking method name
       /// </summary>
       /// <returns></returns>
       private static ILog getLogger()
       {
           System.Diagnostics.StackFrame stackframe = new System.Diagnostics.StackFrame(2, true);
           String packageName = stackframe.GetMethod().ReflectedType.FullName + "." + stackframe.GetMethod().Name;

           return LogManager.GetLogger(packageName);
       }


       /// <summary>
       /// Log as Fatal error, will send out email after development stage
       /// </summary>
       /// <param name="message"></param>
       /// <param name="username"></param>
       /// <param name="userip"></param>
       /// <param name="storeid"></param>
       /// <param name="ex"></param>
       public static void Fatal( string message,string username="", string userip="",string storeid="", Exception ex = null) 
       {
           setUserDefined(message, username, storeid,userip);
           getLogger().Fatal(message, ex);
       }


        /// <summary>
        /// Save to table tblog4net as Critical, will send out email later.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="username">optional , Presentation layer can provide login user name</param>
        /// <param name="userip">optional , Presentation layer can provide login user ipd</param>
        /// <param name="storeid">optional , Presentation layer can provide current store id</param>
        /// <param name="ex">Exception object, will save stacks in logs</param>
        public static void Error(string message, string username = "", string userip = "", string storeid = "", Exception ex = null)
        {
            setUserDefined(message, username, storeid, userip);
            getLogger().Error(message, ex);
        }
       
       
       /// <summary>
       /// Save to table tblog4net as Warning, will send out email later.
       /// </summary>
       /// <param name="message"></param>
       /// <param name="username">optional , Presentation layer can provide login user name</param>
       /// <param name="userip">optional , Presentation layer can provide login user ipd</param>
       /// <param name="storeid">optional , Presentation layer can provide current store id</param>
       /// <param name="ex">Exception object, will save stacks in logs</param>
       public static void Warn(string message,string username="", string userip="",string storeid="", Exception ex = null) 
       {
           setUserDefined(message, username,storeid, userip);
           getLogger().Warn(message,ex);
       }

       /// <summary>
       /// Save to table tblog4net as information
       /// </summary>
       /// <param name="message"></param>
       /// <param name="username"></param>
       /// <param name="userip"></param>
       /// <param name="storeid"></param>
       /// <param name="ex"></param>
       public static void Info(string message,string username="", string userip="",string storeid="", Exception ex = null) 
       {
           setUserDefined(message, username, storeid,userip);
           getLogger().Info(message, ex);
       }

       /// <summary>
       /// auto add username if not provided
       /// auto add userip, now catch IPV6, will change to IPV4
       /// </summary>
       /// <param name="message"></param>
       /// <param name="username"></param>
       /// <param name="storeid"></param>
       /// <param name="userip"></param>
       private static void setUserDefined(string message, string username,string storeid, string userip) {

           if (string.IsNullOrEmpty(username))
           {
               
               //System.Security.Principal.WindowsPrincipal wp = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());
               
               if (System.Web.HttpContext.Current!=null)
               username = System.Web.HttpContext.Current.User.Identity.Name;
               
           }
    
           if (string.IsNullOrEmpty(userip)) {
               userip = getlocalIP();
           }

           MDC.Set("User", username);
           MDC.Set("store_id", storeid);
           MDC.Set("Userip", userip);
           MDC.Set("file", getCallername());
           if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session!=null)
           MDC.Set("Context", System.Web.HttpContext.Current.Session.SessionID);
      }

       /// <summary>
       /// get running machine ips
       /// </summary>
       /// <returns></returns>
       private static string getlocalIP() {
           if (System.Web.HttpContext.Current != null)
           {
                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                return !string.IsNullOrEmpty(ip) ? ip : System.Web.HttpContext.Current.Request.UserHostName;
           }
           else
           {
               IPAddress[] a = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

               foreach (IPAddress i in a)
               {
                   if (i.IsIPv6LinkLocal == false) {
                       return i.ToString();
                   }
               }

               return a[0].ToString();
           }
       }

       /// <summary>
       ///  Get callers function name from stack
       /// </summary>
       /// <returns>A string with all Caller's function name. </returns>
       private static string getCallername() {
           // get call stack
           StackTrace stackTrace = new StackTrace();
           StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)
           StringBuilder  stackcaller = new StringBuilder ( "");
           // write call stack method names
           foreach (StackFrame stackFrame in stackFrames)
           {
               stackcaller.Append (stackFrame.GetMethod().Name);   // write method name
               stackcaller.Append("<--");
           }
           return stackcaller.ToString();
       }
    }
}
