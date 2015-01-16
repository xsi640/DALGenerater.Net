using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess.SQLiteDAL;

namespace DBAccess
{
    /// <summary>
    /// 数据库连接
    /// </summary>
    public static class Conn
    {
        #region 常量
        /// <summary>
        /// 数据库连接字符串，{0}数据库物理路径
        /// </summary>
        public const string DB_CONNECTIONSTRING = "Data Source={0};UTF8Encoding=True;BinaryGuid=False;";
        #endregion

        #region 变量
        private static string _ConnectionString = null;
        #endregion

        #region 属性
        /// <summary>
        /// 当前登录用户的数据库连接字符串
        /// 当更改当前数据库连接字符串后，程序会自动关闭、再次打开数据库连接。
        /// </summary>
        public static string ConnectionString
        {
            get { return Conn._ConnectionString; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && _ConnectionString != value)
                {
                    ConnectionClose();
                    Conn._ConnectionString = value;
                }
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 关闭SQLite数据库连接对象
        /// </summary>
        public static void ConnectionClose()
        {
            if (SQLiteHelper._SqLiteConnection != null)
            {
                if (SQLiteHelper._SqLiteConnection.State == System.Data.ConnectionState.Open)
                {
                    SQLiteHelper._SqLiteConnection.Close();
                }
                SQLiteHelper._SqLiteConnection = null;
            }
        }
        #endregion
    }
}
