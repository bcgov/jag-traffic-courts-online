package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.List;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * Holds response from ORDS Get Statutes List
 * 
 * @author 237563
 *
 */
@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
public final class GetStatutesListServiceResponse {
	
	private List<Statute> statuteCodeValues = null;
}
