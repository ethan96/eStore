using System;
using System.Net;
using System.Web;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text ;
using System.Runtime.Serialization.Formatters;
using eStore.Utilities;

namespace eStore.BusinessModules
{

	class WebPostRequest
	{
		WebRequest theRequest;
		HttpWebResponse theResponse;
		ArrayList  theQueryData;

		public WebPostRequest(string url)
		{
			theRequest = WebRequest.Create(url);
			theRequest.Method = "POST";
			theQueryData = new ArrayList();
		}

		public void Add(string key, string value)
		{
			theQueryData.Add(String.Format("{0}={1}",key,HttpUtility.UrlEncode(value,Encoding.UTF8)));
		}

        public ArrayList getQueryData()
        {
            return theQueryData;
        }

		public string GetResponse()
		{
            try
            {
                // Set the encoding type
                theRequest.ContentType = "application/x-www-form-urlencoded";

                // Build a string containing all the parameters
                string Parameters = String.Join("&", (String[])theQueryData.ToArray(typeof(string)));
                theRequest.ContentLength = Parameters.Length;

                // We write the parameters into the request
                StreamWriter sw = new StreamWriter(theRequest.GetRequestStream());
                sw.Write(Parameters);
                sw.Flush();
                sw.Close();

                // Execute the query
                theResponse = (HttpWebResponse)theRequest.GetResponse();
                StreamReader sr = new StreamReader(theResponse.GetResponseStream());
                return sr.ReadToEnd();

            }catch(Exception e){
                eStoreLoger.Fatal("Failed AEU Direct Payment", "", "", "",e);
                throw e;
            }
		}

        public void setTimeout(int millisecond) {
            theRequest.Timeout = millisecond;
        }

	}
     
}
	 



 