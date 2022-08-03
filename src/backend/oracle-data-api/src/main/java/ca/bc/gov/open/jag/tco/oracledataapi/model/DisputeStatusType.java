package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.OneToMany;
import javax.persistence.Table;

import com.fasterxml.jackson.annotation.JsonManagedReference;

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
    private String disputeStatusTypeCd;
	
	/**
	 * The desciprtion of the status type
	 */
    @Enumerated(EnumType.STRING)
	private DisputeStatus disputeStatusTypeDescription;
    
    @JsonManagedReference
	@OneToMany(targetEntity = Dispute.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
	@JoinColumn(name = "status_id")
	private List<Dispute> disputes = new ArrayList<Dispute>();
    
    public void addDisputes(List<Dispute> disputes) {
		for (Dispute dispute : disputes) {
			dispute.setStatusType(this);
		}
		this.disputes.addAll(disputes);
	}

}
