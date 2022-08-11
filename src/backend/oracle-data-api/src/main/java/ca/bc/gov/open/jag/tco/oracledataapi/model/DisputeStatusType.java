package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
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
public class DisputeStatusType extends Auditable<String> {

	/**
     * Dispute Status Type Code primary key
     */
    @Id
    @Column(length = 10)
    private String disputeStatusTypeCode;
	
	/**
	 * The description of the status type
	 */
    @Column(length = 50)
    @Enumerated(EnumType.STRING)
    @Schema(nullable = false)
	private DisputeStatus disputeStatusTypeDescription;
    
    /**
     * Whether the dispute status is active or not Y/N
     */
    @Enumerated(EnumType.STRING)
    @Schema(nullable = false)
    private YesNo Active;
    
    @JsonManagedReference
	@OneToMany(targetEntity = Dispute.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
	@JoinColumn(name = "dispute_status_type_cd")
	private List<Dispute> disputes = new ArrayList<Dispute>();
    
    public void addDisputes(List<Dispute> disputes) {
		for (Dispute dispute : disputes) {
			dispute.setDisputeStatusType(this);
		}
		this.disputes.addAll(disputes);
	}

}
