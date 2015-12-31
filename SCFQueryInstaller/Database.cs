using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace SCFQueryInstaller
{
    class Database
    {
        private string IpAdress;
        private OdbcConnection OdbcCon;
        private string ConFormat;
        public Exception ExError;
        private string m_DNS;

        public Database(string IP)
        {
            OdbcCon = new OdbcConnection();
            IpAdress = IP;
        }

        public bool ReConnect()
        {
            try
            {
                OdbcCon.ConnectionString = ConFormat;
                OdbcCon.Open();
                OdbcCon.Close();
            }
            catch (Exception x)
            {
                ExError = x;
                return false;
            }
            finally
            {
                if (OdbcCon.State != ConnectionState.Closed)
                    OdbcCon.Close();
            }
            return true;
        }

        public bool Connect(string DNS, string Login, string Password)
        {
            try
            {
                ConFormat = "Server=" + IpAdress + ";DSN=" + DNS + ";UID=" + Login + ";PWD=" + Password + ";";//";//UID=sa;PWD=654321;
                OdbcCon.ConnectionString = ConFormat;
                m_DNS = DNS;
                OdbcCon.Open();
                OdbcCon.Close();
            }
            catch (Exception x)
            {
                ExError = x;
                return false;
            }
            finally
            {
                if (OdbcCon.State != ConnectionState.Closed)
                    OdbcCon.Close();
            }
            return true;
        }

        public bool Exec(string Query)
        {
            try
            {
                ExError = null;
                OdbcCon.Open();
                OdbcCommand cmd = new OdbcCommand(Query, OdbcCon);
                cmd.ExecuteNonQuery();
                OdbcCon.Close();
                return true;
            }
            catch (Exception x)
            {
                ExError = x;
                return false;
            }
            finally
            {
                if (OdbcCon.State != ConnectionState.Closed)
                   OdbcCon.Close();
            }
        }

    }
}
