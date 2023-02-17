package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.OneToMany;
import javax.persistence.Table;

import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class JJDisputeImageData {

	/**
	 * create Date
	 */
	@Column
	@Schema(nullable = true)
	private Date createDate;
	
	/**
	 * version
	 */
	@Column
	@Schema(nullable = true)
	private String version;
	
	/**
	 * All the documents for this request
	 */
	@JsonManagedReference
	@OneToMany(targetEntity = JJDisputeImageDocument.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true, mappedBy = "jjDisputeImageDocument")
	public List<JJDisputeImageDocument> documents = new ArrayList<JJDisputeImageDocument>();
}
