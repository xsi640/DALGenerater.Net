package com.poreader.dao.util;

import java.beans.IntrospectionException;
import java.beans.PropertyDescriptor;
import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.sql.Types;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class DBUtils {

	public static <T> T appendObject(ResultSet rs, Class<T> clazz)
			throws InstantiationException, IllegalAccessException,
			SQLException, IntrospectionException, IllegalArgumentException,
			InvocationTargetException {
		T result = clazz.newInstance();
		Field[] fields = clazz.getDeclaredFields();
		for (Field field : fields) {
			String propertyName = field.getName();
			Object paramVal = null;
			Class<?> clazzField = field.getType();
			if (clazzField == String.class) {
				paramVal = rs.getString(propertyName);
			} else if (clazzField == short.class || clazzField == Short.class) {
				paramVal = rs.getShort(propertyName);
			} else if (clazzField == int.class || clazzField == Integer.class) {
				paramVal = rs.getInt(propertyName);
			} else if (clazzField == long.class || clazzField == Long.class) {
				paramVal = rs.getLong(propertyName);
			} else if (clazzField == float.class || clazzField == Float.class) {
				paramVal = rs.getFloat(propertyName);
			} else if (clazzField == double.class || clazzField == Double.class) {
				paramVal = rs.getDouble(propertyName);
			} else if (clazzField == boolean.class
					|| clazzField == Boolean.class) {
				paramVal = rs.getBoolean(propertyName);
			} else if (clazzField == byte.class || clazzField == Byte.class) {
				paramVal = rs.getByte(propertyName);
			} else if (clazzField == char.class
					|| clazzField == Character.class) {
				paramVal = rs.getCharacterStream(propertyName);
			} else if (clazzField == Date.class) {
				paramVal = rs.getTimestamp(propertyName);
			} else if (clazzField.isArray()) {
				paramVal = rs.getString(propertyName).split(","); // 以逗号分隔的字符串
			}
			PropertyDescriptor pd = new PropertyDescriptor(propertyName, clazz);
			pd.getWriteMethod().invoke(result, paramVal);
		}
		return result;
	}

	public static void setPreparedStatement(PreparedStatement ps,
			Object[] values) throws SQLException {
		if (values == null)
			return;
		for (int i = 1; i <= values.length; i++) {
			Object fieldValue = values[i - 1];
			Class<?> clazzValue = fieldValue.getClass();
			if (clazzValue == String.class) {
				ps.setString(i, (String) fieldValue);
			} else if (clazzValue == boolean.class
					|| clazzValue == Boolean.class) {
				ps.setBoolean(i, (Boolean) fieldValue);
			} else if (clazzValue == byte.class || clazzValue == Byte.class) {
				ps.setByte(i, (Byte) fieldValue);
			} else if (clazzValue == char.class
					|| clazzValue == Character.class) {
				ps.setObject(i, fieldValue, Types.CHAR);
			} else if (clazzValue == Date.class) {
				ps.setTimestamp(i, new Timestamp(((Date) fieldValue).getTime()));
			} else if (clazzValue.isArray()) {
				Object[] arrayValue = (Object[]) fieldValue;
				StringBuffer sb = new StringBuffer();
				for (int j = 0; j < arrayValue.length; j++) {
					sb.append(arrayValue[j]).append("、");
				}
				ps.setString(i, sb.deleteCharAt(sb.length() - 1).toString());
			} else {
				ps.setObject(i, fieldValue, Types.NUMERIC);
			}
		}
	}

	public static int ExecuteNonQuery(Connection conn, String sql,
			Object[] values) throws SQLException {
		int result = 0;
		PreparedStatement statement = conn.prepareStatement(sql);
		setPreparedStatement(statement, values);
		result = statement.executeUpdate();
		return result;
	}

	public static int ExecuteNonQuery(Connection conn, String sql)
			throws SQLException {
		return ExecuteNonQuery(conn, sql, null);
	}

	public static int ExecuteNonQuery(String sql, Object[] values)
			throws SQLException {
		int result = 0;
		PreparedStatement statement = Conn.getConnection()
				.prepareStatement(sql);
		setPreparedStatement(statement, values);
		result = statement.executeUpdate();
		return result;
	}

	public static int ExecuteNonQuery(String sql) throws SQLException {
		return ExecuteNonQuery(sql, null);
	}

	@SuppressWarnings("unchecked")
	public static <T> T ExecuteEntity(String sql, Object[] values,
			Class<?> clazz) throws InstantiationException,
			IllegalAccessException, IllegalArgumentException,
			InvocationTargetException, SQLException, IntrospectionException {
		T result = null;
		PreparedStatement statement = Conn.getConnection()
				.prepareStatement(sql);
		setPreparedStatement(statement, values);
		ResultSet rs = statement.executeQuery();
		if (rs.next()) {
			result = (T) appendObject(rs, clazz);
		}
		return result;
	}

	public static <T> T ExecuteEntity(String sql, Class<?> clazz)
			throws InstantiationException, IllegalAccessException,
			IllegalArgumentException, InvocationTargetException, SQLException,
			IntrospectionException {
		return ExecuteEntity(sql, null, clazz);
	}

	@SuppressWarnings("unchecked")
	public static <T> List<T> ExecuteList(String sql, Object[] values,
			Class<?> clazz) throws InstantiationException,
			IllegalAccessException, IllegalArgumentException,
			InvocationTargetException, SQLException, IntrospectionException {
		List<T> result = new ArrayList<T>();
		PreparedStatement statement = Conn.getConnection()
				.prepareStatement(sql);
		setPreparedStatement(statement, values);
		ResultSet rs = statement.executeQuery();
		while (rs.next()) {
			T t = (T) appendObject(rs, clazz);
			result.add(t);
		}
		return result;
	}

	public static <T> List<T> ExecuteList(String sql, Class<?> clazz)
			throws InstantiationException, IllegalAccessException,
			IllegalArgumentException, InvocationTargetException, SQLException,
			IntrospectionException {
		return ExecuteList(sql, null, clazz);
	}

	public static int ExecuteScalar(String sql) throws SQLException {
		int result = 0;
		Statement statement = Conn.getConnection().createStatement();
		ResultSet rs = statement.executeQuery(sql);
		if (rs.next()) {
			result = rs.getInt(1);
		}
		return result;
	}
}
