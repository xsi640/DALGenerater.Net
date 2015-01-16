using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBAccess.IDAL
{
    /// <summary>
    /// 基础数据库访问接口
    /// </summary>
    /// <typeparam name="TKey">数据库主键</typeparam>
    /// <typeparam name="TObject">数据库对象</typeparam>
    public interface IBaseDAL<TKey, TObject>
    {
        int Insert(TObject obj);
        int Insert(IList<TObject> lists);
        int InsertOrUpdate(TObject obj);
        int InsertOrUpdate(IList<TObject> lists);
        int Update(TObject obj);
        int Delete(TKey key);
        int DeleteAll();
        int DeleteAll(string where);
        int Delete(IList<TKey> idLists);
        List<TObject> Select(string where, string order = "");
        List<TObject> SelectAll(string order = "");
        TObject Get(TKey key);
        int Count(string where);
        int CountAll();
    }
}
