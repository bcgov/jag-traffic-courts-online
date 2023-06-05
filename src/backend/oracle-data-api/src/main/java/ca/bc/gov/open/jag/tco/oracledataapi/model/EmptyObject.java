package ca.bc.gov.open.jag.tco.oracledataapi.model;

import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * Empty object used send empty body
 */
@Getter
@Setter
@NoArgsConstructor
@JsonInclude(JsonInclude.Include.NON_NULL)
public class EmptyObject {
    private String field;
    public static final EmptyObject instance = new EmptyObject();
}
