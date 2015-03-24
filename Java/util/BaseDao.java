package com.poreader.dao.util;

import java.util.List;

public interface BaseDao<TKey, TObject> {
	int Insert(TObject obj);

	int Insert(List<TObject> lists);

	int InsertOrUpdate(TObject obj);

	int InsertOrUpdate(List<TObject> lists);

	int Update(TObject obj);

	int Delete(TKey key);

	int DeleteAll();

	int DeleteAll(String where);

	int Delete(List<TKey> idLists);

	List<TObject> Select();

	List<TObject> Select(String where);
	
	List<TObject> Select(String where, String order);

	List<TObject> Select(int offset,int limit);
	
	List<TObject> Select(String where, int offset,int limit);

	List<TObject> Select(String where, String order, int offset, int limit);

	TObject Get(TKey key);

	int Count(String where);

	int Count();
}
