package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.AttributeConverter;
import javax.persistence.Converter;

/**
 * This class maps <i>Boolean</i> to <code>'Y'</code> or <code>'N'</code> characters (or null if Boolean is null).<br/>
 * <br/>
 * Note: autoApply is set to <code>true</code> here, meaning this is affects *all* Boolean when serialized/deserialized from the database
 */
@Converter(autoApply = true)
public class BooleanConverter implements AttributeConverter<Boolean, Character> {

	@Override
	public Character convertToDatabaseColumn(Boolean attribute) {
		if (attribute != null) {
			if (attribute) {
				return 'Y';
			} else {
				return 'N';
			}
		}
		return null;
	}

	@Override
	public Boolean convertToEntityAttribute(Character dbData) {
		if (dbData != null) {
			return dbData.equals('Y');
		}
		return null;
	}

}
