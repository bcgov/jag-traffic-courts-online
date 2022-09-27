package ca.bc.gov.open.jag.tco.oracledataapi.dto;

import java.sql.Date;

import com.fasterxml.jackson.annotation.JsonFormat;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class DisputeCountDTO {
	
	private String pleaCd;
	private String requestTimeToPayYn;
	private String requestReductionYn;
	private String requestCourtAppearanceYn;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date entDtm;
	private String entUserId;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date updDtm;
	private String updUserId;
}
