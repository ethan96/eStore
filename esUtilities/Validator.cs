using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net.Mail;

namespace esUtilities
{
    /// <summary>
    /// This class is used to validate parameter as below - 
    /// 1. EMail format
    /// </summary>
    public class Validator
    {
        // Email pattern
        private const string emailPattern =  @"^(([^<>()[\]\\.,;:\s@\""]+"
                     + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                     + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                     + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                     + @"[a-zA-Z]{2,}))$";

        public static bool isValidEmail(string email)
        {
            bool result;
            email = email.Trim();
            result = Regex.IsMatch(email, emailPattern);

            return result;
        }

        public static bool areValidEmail(List<string> emails)
        {
            bool validFlag = false;
            if (emails == null)
                return validFlag;
            foreach(string em in emails)
            {
                string mail;
                mail = em.Trim();
                if (!Regex.IsMatch(mail, emailPattern))
                {
                    validFlag = false;
                }
                else
                {
                    validFlag = true;
                }
            }

            return validFlag;
        }

        public static List<string> getInvalidEmails(List<string> emails)
        {
            List<string> invalidEmails = new List<string>();
            foreach (string em in emails)
            {
                string email;
                email = em.Trim();
                if (!Regex.IsMatch(email, emailPattern))
                {
                    invalidEmails.Add(email);
                } 
            }

            return invalidEmails;
        }


        public static MailAddressCollection getValidEmails(List<string> emails)
        {
            MailAddressCollection validEmails = new MailAddressCollection();
            foreach (string em in emails)
            {
                string email;
                email = em.Trim();
                if (Regex.IsMatch(email, emailPattern))
                {
                    MailAddress m = new MailAddress(email);
                    validEmails.Add(m);
                }
            }

            return validEmails;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static bool isBitInclude(object item, int? coll)
        {
            if (item == null || coll == null)
                return false;
            int markSt = (int)Math.Pow(2, (int)item);
            var tt = (coll & markSt) == markSt;
            return tt;
        }


    }
}
