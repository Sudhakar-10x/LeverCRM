using System;
using System.Data;
using Sap.Data.Hana;
using System.Configuration;
public class HanaDataHelper
{
    public enum HanaCmdType
    {
        SqlText = 1,
        StoredProcedure = 2
    }

    private HanaConnection hanaCon;

    public HanaDataHelper()
    {
        string connectionString = ConfigurationManager.AppSettings["HanaCon"].ToString();
        hanaCon = new HanaConnection(connectionString);
    }

    public void OpenDBCon()
    {
        if (hanaCon.State == ConnectionState.Closed)
            hanaCon.Open();
    }

    public void CloseDBCon()
    {
        if (hanaCon.State == ConnectionState.Open)
            hanaCon.Close();
    }

    public HanaConnection GetConnection()
    {
        return hanaCon;
    }

    public DataSet GetDataset(string strSql, HanaCmdType cmdType, HanaParameter[] hanaParams = null, HanaTransaction objHanaTrans = null)
    {
        var dsData = new DataSet();
        try
        {
            using (var cmd = new HanaCommand(strSql, hanaCon))
            {
                if (objHanaTrans != null)
                    cmd.Transaction = objHanaTrans;
                else
                    OpenDBCon();

                SetCommandType(cmd, cmdType);
                if (hanaParams != null)
                    AddParameters(cmd, hanaParams);

                using (var da = new HanaDataAdapter(cmd))
                {
                    da.Fill(dsData);
                }
            }
            return dsData;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public HanaDataReader ExecuteReader(string strSql, HanaCmdType cmdType, HanaParameter[] hanaParams = null)
    {
        var cmd = new HanaCommand(strSql, hanaCon);
        SetCommandType(cmd, cmdType);
        if (hanaParams != null)
            AddParameters(cmd, hanaParams);

        OpenDBCon();
        return cmd.ExecuteReader(); // caller must dispose reader
    }

    public int ExecuteNonQuery(string strSql, HanaCmdType cmdType, HanaParameter[] hanaParams = null, HanaTransaction objHanaTrans = null)
    {
        try
        {
            using (var cmd = new HanaCommand(strSql, hanaCon))
            {
                if (objHanaTrans != null)
                    cmd.Transaction = objHanaTrans;
                else
                    OpenDBCon();

                SetCommandType(cmd, cmdType);
                if (hanaParams != null)
                    AddParameters(cmd, hanaParams);

                return cmd.ExecuteNonQuery();
            }
        }
        finally
        {
            if (objHanaTrans == null)
                CloseDBCon();
        }
    }

    public object ExecuteScalar(string strSql, HanaCmdType cmdType, HanaParameter[] hanaParams = null, HanaTransaction objHanaTrans = null)
    {
        try
        {
            using (var cmd = new HanaCommand(strSql, hanaCon))
            {
                if (objHanaTrans != null)
                    cmd.Transaction = objHanaTrans;
                else
                    OpenDBCon();

                SetCommandType(cmd, cmdType);
                if (hanaParams != null)
                    AddParameters(cmd, hanaParams);

                return cmd.ExecuteScalar();
            }
        }
        finally
        {
            if (objHanaTrans == null)
                CloseDBCon();
        }
    }

    private void SetCommandType(HanaCommand cmd, HanaCmdType cmdType)
    {
        cmd.CommandType = (cmdType == HanaCmdType.StoredProcedure)
            ? CommandType.StoredProcedure
            : CommandType.Text;
    }

    private void AddParameters(HanaCommand cmd, HanaParameter[] hanaParams)
    {
        foreach (var p in hanaParams)
        {
            cmd.Parameters.Add(p);
        }
    }
}
