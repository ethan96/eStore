using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace esUtilities
{
    public interface IMailer
    {
        /// <summary>
        /// This function shall return mail physical file name. All email will save as a file in eStore.Utilities.MailRepository
        /// </summary>
        /// <param name="mailToAddress"></param>
        /// <param name="mailFromName"></param>
        /// <param name="mailFromAddress"></param>
        /// <param name="subject"></param>
        /// <param name="mailbody"></param>
        /// <returns></returns>
        EMailReponse sendMail(string mailToAddress, string mailFromName, string mailFromAddress, string subject, string mailbody, string storeId, String mailCC="", String mailBcc ="");


        

    }
}
