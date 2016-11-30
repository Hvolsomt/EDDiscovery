﻿using EDDiscovery2.DB;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace EDDiscovery.DB
{
    [Flags]
    public enum EDDSqlDbSelection
    {
        None = 0,
        EDDiscovery = 1,
        EDDUser = 2,
        EDDSystem = 4
    }

    public static partial class SQLiteDBClass
    {
        #region Private properties / fields
        private static Object lockDBInit = new Object();                    // lock to sequence construction
        #endregion

        #region Transitional properties
        public static EDDSqlDbSelection DefaultMainDatabase { get { return  EDDSqlDbSelection.EDDUser; } }
        public static EDDSqlDbSelection UserDatabase { get { return EDDSqlDbSelection.EDDUser; } }
        public static EDDSqlDbSelection SystemDatabase { get { return EDDSqlDbSelection.EDDSystem;  } }
        #endregion

        #region Database Initialization
        public static void ExecuteQuery(SQLiteConnectionED conn, string query)
        {
            using (DbCommand command = conn.CreateCommand(query))
                command.ExecuteNonQuery();
        }

        public static void PerformUpgrade(SQLiteConnectionED conn, int newVersion, bool catchErrors, bool backupDbFile, string[] queries, Action doAfterQueries = null)
        {
            if (backupDbFile)
            {
                string dbfile = conn.DBFile;

                try
                {
                    File.Copy(dbfile, dbfile.Replace(".sqlite", $"{newVersion - 1}.sqlite"));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Exception: " + ex.Message);
                    System.Diagnostics.Trace.WriteLine("Trace: " + ex.StackTrace);
                }
            }

            try
            {
                foreach (var query in queries)
                {
                    ExecuteQuery(conn, query);
                }
            }
            catch (Exception ex)
            {
                if (!catchErrors)
                    throw;

                System.Diagnostics.Trace.WriteLine("Exception: " + ex.Message);
                System.Diagnostics.Trace.WriteLine("Trace: " + ex.StackTrace);
                MessageBox.Show($"UpgradeDB{newVersion} error: " + ex.Message);
            }

            doAfterQueries?.Invoke();

            conn.PutSettingIntCN("DBVer", newVersion);
        }





        #endregion

        #region Database access
        ///----------------------------
        /// STATIC code helpers for other DB classes

        public static DataSet SQLQueryText(SQLiteConnectionED cn, DbCommand cmd)  
        {
            try
            {
                DataSet ds = new DataSet();
                DbDataAdapter da = cn.CreateDataAdapter(cmd);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SqlQuery Exception: " + ex.Message);
                throw;
            }
        }

        static public int SQLNonQueryText(SQLiteConnectionED cn, DbCommand cmd)   
        {
            int rows = 0;

            try
            {
                rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SqlNonQueryText Exception: " + ex.Message);
                throw;
            }
        }

        static public object SQLScalar(SQLiteConnectionED cn, DbCommand cmd)      
        {
            object ret = null;

            try
            {
                ret = cmd.ExecuteScalar();
                return ret;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SqlNonQuery Exception: " + ex.Message);
                throw;
            }
        }
        #endregion

        #region Settings
        ///----------------------------
        /// STATIC functions for discrete values

        static public bool keyExists(string sKey)
        {
            return SQLiteConnectionUser.keyExists(sKey);
        }

        static public int GetSettingInt(string key, int defaultvalue)
        {
            return SQLiteConnectionUser.GetSettingInt(key, defaultvalue);
        }

        static public bool PutSettingInt(string key, int intvalue)
        {
            return SQLiteConnectionUser.PutSettingInt(key, intvalue);
        }

        static public double GetSettingDouble(string key, double defaultvalue)
        {
            return SQLiteConnectionUser.GetSettingDouble(key, defaultvalue);
        }

        static public bool PutSettingDouble(string key, double doublevalue)
        {
            return SQLiteConnectionUser.PutSettingDouble(key, doublevalue);
        }

        static public bool GetSettingBool(string key, bool defaultvalue)
        {
            return SQLiteConnectionUser.GetSettingBool(key, defaultvalue);
        }

        static public bool PutSettingBool(string key, bool boolvalue)
        {
            return SQLiteConnectionUser.PutSettingBool(key, boolvalue);
        }

        static public string GetSettingString(string key, string defaultvalue)
        {
            return SQLiteConnectionUser.GetSettingString(key, defaultvalue);
        }

        static public bool PutSettingString(string key, string strvalue)
        {
            return SQLiteConnectionUser.PutSettingString(key, strvalue);
        }
        #endregion
    }
}
