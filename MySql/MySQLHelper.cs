using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DBAccess
{
    public static class MySqlHelper
    {
        #region 变量
        internal static MySqlConnection _SqLiteConnection = null;
        #endregion

        #region 属性
        /// <summary>
        /// 返回一个打开状态的MySql连接对象
        /// 单例设计模式
        /// </summary>
        public static MySqlConnection MySqlConnection
        {
            get
            {
                if (MySqlHelper._SqLiteConnection == null)
                {
                    MySqlHelper._SqLiteConnection = new MySqlConnection();
                }
                if (string.IsNullOrWhiteSpace(MySqlHelper._SqLiteConnection.ConnectionString))
                {
                    MySqlHelper._SqLiteConnection.ConnectionString = Conn.ConnectionString;
                }
                if (MySqlHelper._SqLiteConnection.State != ConnectionState.Open)
                    MySqlHelper._SqLiteConnection.Open();
                return MySqlHelper._SqLiteConnection;
            }
            set { MySqlHelper._SqLiteConnection = value; }
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
            catch (MySqlException ex)
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
        public static List<T> ExecuteList<T>(string commandText, params MySqlParameter[] array)
        {
            List<T> lists = new List<T>();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(commandText, MySqlHelper.MySqlConnection))
                {
                    cmd.Parameters.AddRange(array);
                    using (MySqlDataReader dr = cmd.ExecuteReader())
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
            catch (MySqlException ex)
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
        public static T ExecuteEntity<T>(string commandText, params MySqlParameter[] array)
        {
            T obj = default(T);
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(commandText, MySqlHelper.MySqlConnection))
                {
                    cmd.Parameters.AddRange(array);
                    using (MySqlDataReader dr = cmd.ExecuteReader())
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
            catch (MySqlException ex)
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
        public static int ExecuteNonQuery(string commandText, params MySqlParameter[] array)
        {
            int rowCount = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(commandText, MySqlHelper.MySqlConnection))
                {
                    if (array != null && array.Length > 0)
                        cmd.Parameters.AddRange(array);
                    rowCount = cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
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
        public static int ExecuteNonQuery(MySqlTransaction transaction, string commandText, params MySqlParameter[] array)
        {
            int rowCount = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(commandText, MySqlHelper.MySqlConnection, transaction))
                {
                    if (array != null && array.Length > 0)
                        cmd.Parameters.AddRange(array);
                    rowCount = cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
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
        public static int ExecuteScalar(string commandText, params MySqlParameter[] array)
        {
            int result = 0;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(commandText, MySqlHelper.MySqlConnection))
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
            catch (MySqlException ex)
            {
                throw ex;
            }
            return result;
        }
        #endregion
    }
}
