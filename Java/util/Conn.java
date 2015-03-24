package com.poreader.dao.util;

import java.sql.Connection;
import java.sql.SQLException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.alibaba.druid.pool.DruidDataSource;
import com.alibaba.druid.pool.DruidDataSourceFactory;

public class Conn {
	private static final Logger logger = LoggerFactory.getLogger(Conn.class);
	private static DruidDataSource druidDataSource = null;

	public static DruidDataSource getDruidDataSource() {
		if (druidDataSource == null) {
			try {
				druidDataSource = (DruidDataSource) DruidDataSourceFactory
						.createDataSource(DBConfig.getProperties());
			} catch (Exception e) {
				logger.error(e.getMessage());
				e.printStackTrace();
			}
		}
		return druidDataSource;
	}

	public static Connection getConnection() {
		try {
			return getDruidDataSource().getConnection();
		} catch (SQLException e) {
			logger.error(e.getMessage());
			e.printStackTrace();
		}
		return null;
	}
}
