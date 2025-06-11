using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace _10xErp.Helpers
{
    public class DataHelper
    {
        //HanaConnection hCon;
        SqlConnection hCon;
        public DataHelper()
        {
            hCon = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("SqlCon").ToString());

        }
        public int ExecuteNonQuery(string sQry)
        {
            int i = 0;
            try
            {
                hCon.Open();
                //HanaCommand cmd = new HanaCommand(sQry, hCon);
                SqlCommand cmd = new SqlCommand(sQry, hCon);
                i = cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (hCon.State == System.Data.ConnectionState.Open) hCon.Close();
            }

            return i;
        }
        public object ExecuteScalar(string sQry)
        {
            object objVar = null;
            try
            {
                hCon.Open();
                //HanaCommand cmd = new HanaCommand(sQry, hCon);
                SqlCommand cmd = new SqlCommand(sQry, hCon);
                objVar = cmd.ExecuteScalar();


            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (hCon.State == System.Data.ConnectionState.Open) hCon.Close();
            }

            return objVar;
        }
        public DataSet getDataSet(string sQry)
        {
            DataSet dsData = null;
            try
            {

                //HanaCommand cmd = new HanaCommand(sQry, hCon);
                //HanaDataAdapter hda = new HanaDataAdapter(cmd);
                SqlCommand cmd = new SqlCommand(sQry, hCon);
                SqlDataAdapter hda = new SqlDataAdapter(cmd);

                dsData = new DataSet();
                hda.Fill(dsData, "ds");

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }

            return dsData;
        }
    }
}