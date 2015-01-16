using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Reflection;
using System.Data.Common;

namespace DBAccess.SQLiteDAL
{
    /// <summary>
    /// SQLite数据库访问辅助类
    /// </summary>
    public static class SQLiteHelper
    {
        #region 变量
        internal static SQLiteConnection _SqLiteConnection = null;
        #endregion

        #region 属性
        /// <summary>
        /// 返回一个打开状态的SQLite连接对象
        /// 单例设计模式
        /// </summary>
        public static SQLiteConnection SQLiteConnection
        {
            get
            {
                if (SQLiteHelper._SqLiteConnection == null)
                {
                    SQLiteHelper._SqLiteConnection = new SQLiteConnection();
                }
                if (string.IsNullOrWhiteSpace(SQLiteHelper._SqLiteConnection.ConnectionString))
                {
                    SQLiteHelper._SqLiteConnection.ConnectionString = Conn.ConnectionString;
                }
                if (SQLiteHelper._SqLiteConnection.State != ConnectionState.Open)
                    SQLiteHelper._SqLiteConnection.Open();
                return SQLiteHelper._SqLiteConnection;
            }
            set { SQLiteHelper._SqLiteConnection = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 将一条查询记录创建为一个实体类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static T ExecuteDataReader<T>(IDataReader dr)
        {
            T obj = default(T);
            try
            {
                obj = Activator.CreateInstance<T>();
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                int fieldCount = dr.FieldCount;
                foreach (PropertyInfo propertyInfo in properties)
                {
                    string propertyName = propertyInfo.Name;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        string fieldName = dr.GetName(i);
                        if (string.Compare(propertyName, fieldName, true) == 0)
                        {
                            object value = dr.GetValue(i);
                            if (value != null && value != DBNull.Value)
                            {
                                propertyInfo.SetValue(obj, value, null);
                            }
                            break;
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return obj;
        }
        /// <summary>
        /// 返回符合条件查询结果的泛型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<T> ExecuteList<T>(string commandText, params SQLiteParameter[] array)
        {
            List<T> lists = new List<T>();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(commandText, SQLiteHelper.SQLiteConnection))
                {
                    cmd.Parameters.AddRange(array);
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            while (dr.Read())
                            {
                                T obj = ExecuteDataReader<T>(dr);
                                if (obj != null)
                                {
                                    lists.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }

            return lists;
        }
        /// <summary>
        /// 返回符合条件查询结果的实体类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T ExecuteEntity<T>(string commandText, params SQLiteParameter[] array)
        {
            T obj = default(T);
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(commandText, SQLiteHelper.SQLiteConnection))
                {
                    cmd.Parameters.AddRange(array);
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            while (dr.Read())
                            {
                                obj = ExecuteDataReader<T>(dr);
                                break;
                            }
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return obj;
        }
        /// <summary>
        /// 执行不查询操作数据库命令
        /// 不支持事务操作
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string commandText, params SQLiteParameter[] array)
        {
            int rowCount = 0;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLiteHelper.SQLiteConnection))
                {
                    cmd.CommandText = commandText;
                    if (array != null && array.Length > 0)
                        cmd.Parameters.AddRange(array);
                    rowCount = cmd.ExecuteNonQuery();
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return rowCount;
        }
        /// <summary>
        /// 执行不查询操作数据库命令
        /// 支持事务操作
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SQLiteTransaction transaction, string commandText, params SQLiteParameter[] array)
        {
            int rowCount = 0;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(commandText, SQLiteHelper.SQLiteConnection, transaction))
                {
                    if (array != null && array.Length > 0)
                        cmd.Parameters.AddRange(array);
                    rowCount = cmd.ExecuteNonQuery();
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return rowCount;

        }
        /// <summary>
        /// 执行聚合函数查询
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int ExecuteScalar(string commandText, params SQLiteParameter[] array)
        {
            int result = 0;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(commandText, SQLiteHelper.SQLiteConnection))
                {
                    if (array != null && array.Length > 0)
                        cmd.Parameters.AddRange(array);
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        Int32.TryParse(obj.ToString(), out result);
                    }
                }
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }
        #endregion
    }
}
