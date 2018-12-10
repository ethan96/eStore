using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace eStore.POCOS.DAL
{
    public class ZTSD_106Helper : Helper
    {
        public int Truncate106A()
        {
            try
            {
                context.ExecuteStoreCommand("TRUNCATE TABLE ZTSD_106A");
                return 0;
            }
            catch
            {
                return -5000;
            }
        }

        public int Truncate106C()
        {
            try
            {
                context.ExecuteStoreCommand("TRUNCATE TABLE ZTSD_106C");
                return 0;
            }
            catch
            {
                return -5000;
            }
        }

        public int BulkInsert106A(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["eStoreProductionConnectionString"].ConnectionString))
                    {
                        con.Open();

                        if (con.State == System.Data.ConnectionState.Closed)
                            con.Open();

                        SqlBulkCopy bc = new SqlBulkCopy(con);
                        bc.DestinationTableName = "ZTSD_106A";
                        bc.ColumnMappings.Add("Company_ID", "Company_ID");
                        bc.ColumnMappings.Add("Org_ID", "Org_ID");
                        bc.WriteToServer(dt);
                    }
                }
                catch
                {
                    return -5000;
                }
            }
            return 0;
        }

        public int BulkInsert106C(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["eStoreProductionConnectionString"].ConnectionString))
                    {
                        con.Open();

                        if (con.State == System.Data.ConnectionState.Closed)
                            con.Open();

                        SqlBulkCopy bc = new SqlBulkCopy(con);
                        bc.DestinationTableName = "ZTSD_106C";
                        bc.ColumnMappings.Add("Company_ID", "Company_ID");
                        bc.ColumnMappings.Add("Org_ID", "Org_ID");
                        bc.WriteToServer(dt);
                    }
                }
                catch
                {
                    return -5000;
                }
            }
            return 0;
        }
    }
}
