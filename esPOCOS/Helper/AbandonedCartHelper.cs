using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class AbandonedCartHelper : Helper
    {
        public DataTable GetDataTable(string sqlcmd)
        {
            System.Data.EntityClient.EntityConnection ec = context.Connection as System.Data.EntityClient.EntityConnection;
            if (string.IsNullOrEmpty(ec.StoreConnection.ConnectionString))
                return null;

            SqlConnection conn = new SqlConnection(ec.StoreConnection.ConnectionString);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sqlcmd, conn);
            da.SelectCommand.CommandTimeout = 5 * 60;

            try
            {
                da.Fill(dt);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
            }
            return dt;
        }
    }
}
