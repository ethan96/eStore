using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.PocoX;
using System.Data;

namespace eStore.POCOS.DAL
{
    public class ChannelPartnerHelper
    {

        public static List<ChannelPartner> getAllChannelPartner(string usercountry)
        {
            List<ChannelPartner> allpartners = new List<ChannelPartner>();
            allpartners = getChannelPartners(usercountry, "EA");

            allpartners.AddRange(getChannelPartners(usercountry, "EP"));

            return allpartners;


        }

        public static List<ChannelPartner> getChannelPartners(string usercountry, string productgroup)
        {
            List<ChannelPartner> cps = new List<ChannelPartner>();
            try
            {
                AdvantechPartner.AdvantechWebService cpws = new AdvantechPartner.AdvantechWebService();
                DataTable dt = cpws.getChannelPartnerList(productgroup, usercountry).Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    ChannelPartner cp = new ChannelPartner();
                    cp.Channelid = int.Parse(dr[0].ToString());
                    cp.Company = dr[2].ToString();
                    cp.Address = dr[20].ToString();
                    cp.City = dr[21].ToString();
                    cp.Zip = dr[24].ToString();
                    cp.Email = dr["secondary_email"].ToString();
                    cp.Phone = dr[8].ToString();
                    cp.Country = dr[25].ToString();
                    cp.CPSalesemail = dr["ChannelSalesEmail"].ToString();

                    cps.Add(cp);
                }
            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Error("Channel Partners service error", "", "", "", ex);
            }

            return cps;
        }

        public static ChannelPartner getChannelPartner(int ID)
        {
            ChannelPartner cp = null;
            if (ID > 0)
            {
                try
                {
                    AdvantechPartner.AdvantechWebService cpws = new AdvantechPartner.AdvantechWebService();
                    DataSet ds = cpws.getPartnerInfo(ID.ToString());
                    if (ds != null && ds.Tables != null && ds.Tables[0] != null)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            cp = new ChannelPartner();
                            cp.Channelid = int.Parse(dr[0].ToString());
                            cp.Company = dr[2].ToString();
                            cp.Address = dr[20].ToString();
                            cp.City = dr[21].ToString();
                            cp.Zip = dr[24].ToString();
                            cp.Email = dr["secondary_email"].ToString();
                            cp.Phone = dr[8].ToString();
                            cp.Country = dr[25].ToString();
                            cp.CPSalesemail = dr["ChannelSalesEmail"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    eStore.Utilities.eStoreLoger.Error("get Channel Partner error", "", "", "", ex);
                }
            }
            return cp;
        }
    }
}
