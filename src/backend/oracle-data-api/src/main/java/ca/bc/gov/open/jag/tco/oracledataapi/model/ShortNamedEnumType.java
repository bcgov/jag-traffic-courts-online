package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.io.Serializable;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import java.util.Properties;

import org.hibernate.HibernateException;
import org.hibernate.engine.spi.SharedSessionContractImplementor;
import org.hibernate.usertype.DynamicParameterizedType;
import org.hibernate.usertype.UserType;

public class ShortNamedEnumType implements DynamicParameterizedType, UserType {

	private Class<? extends Enum<?>> enumClass;

	@Override
	public int[] sqlTypes() {
		return new int[] { Types.VARCHAR };
	}

	@Override
	public Class<? extends Enum<?>> returnedClass() {
		return enumClass;
	}

	@Override
	public boolean equals(Object x, Object y) throws HibernateException {
		return x == y;
	}

	@Override
	public int hashCode(Object x) throws HibernateException {
		return x == null ? 0 : x.hashCode();
	}

	@Override
	public Object nullSafeGet(ResultSet rs, String[] names, SharedSessionContractImplementor session, Object owner)
			throws HibernateException, SQLException {
		String shortCode = rs.getString(names[0]);
		if (rs.wasNull()) {
			return null;
		}
		for (Enum<?> value : returnedClass().getEnumConstants()) {
			if (value instanceof ShortNamedEnum) {
				ShortNamedEnum shortEnum = (ShortNamedEnum) value;
				if (shortEnum.getShortName().equals(shortCode)) {
					return value;
				}
			}
		}
		throw new IllegalStateException("Unknown " + returnedClass().getSimpleName() + " label");
	}

	@Override
	public void nullSafeSet(PreparedStatement st, Object value, int index, SharedSessionContractImplementor session)
			throws HibernateException, SQLException {
		if (value == null) {
			st.setNull(index, Types.VARCHAR);
		}
		else {
			st.setString(index, ((ShortNamedEnum) value).getShortName());
		}
	}

	@Override
	public Object deepCopy(Object value) throws HibernateException {
		return value;
	}

	@Override
	public boolean isMutable() {
		return false;
	}

	@Override
	public Serializable disassemble(Object value) throws HibernateException {
		return (Serializable) value;
	}

	@Override
	public Object assemble(Serializable cached, Object owner) throws HibernateException {
		return cached;
	}

	@Override
	public Object replace(Object original, Object target, Object owner) throws HibernateException {
		return original;
	}

	@Override
	@SuppressWarnings("unchecked")
	public void setParameterValues(Properties parameters) {
		ParameterType params = (ParameterType) parameters.get(PARAMETER_TYPE);
		enumClass = params.getReturnedClass();
	}

}
