package ca.bc.gov.open.jag.tco.oracledataapi.model;
import static javax.persistence.TemporalType.TIMESTAMP;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.EntityListeners;
import javax.persistence.MappedSuperclass;
import javax.persistence.Temporal;

import org.hibernate.annotations.TypeDef;
import org.hibernate.annotations.TypeDefs;
import org.springframework.data.annotation.CreatedBy;
import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.annotation.LastModifiedBy;
import org.springframework.data.annotation.LastModifiedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;

/**
 * An abstract Auditable class that auto-populates <code>createdBy</code>, <code>createdTs</code>, <code>modifiedBy</code>, and
 * <code>modifiedTs</code> fields. Classes need only to extend this class to add auditing fields to a model object.
 */
@TypeDefs({
	@TypeDef(name = "disputantUpdateStatus", defaultForType = DisputantUpdateRequestStatus.class, typeClass = ShortNamedEnumType.class),
	@TypeDef(name = "disputantUpdateType", defaultForType = DisputantUpdateRequestType.class, typeClass = ShortNamedEnumType.class),
	@TypeDef(name = "jjDisputeHearingType", defaultForType = JJDisputeHearingType.class, typeClass = ShortNamedEnumType.class)
})
@MappedSuperclass
@Getter
@Setter
@EntityListeners(AuditingEntityListener.class)
public abstract class Auditable<U> {

	/** The username of the individual (or system) who created this record. */
	@CreatedBy
	@Column(updatable = false)
	private U createdBy;

	/** The timestamp this record was created. This should always be in UTC date-time (ISO 8601) format */
	@CreatedDate
	@Temporal(TIMESTAMP)
	@Column(updatable = false)
	private Date createdTs;

	/** The username of the individual (or system) who modified this record. */
	@LastModifiedBy
	@Schema(nullable = true)
	private U modifiedBy;

	/** The timestamp this record was last modified. This should always be in UTC date-time (ISO 8601) format*/
	@LastModifiedDate
	@Temporal(TIMESTAMP)
	@Schema(nullable = true)
	private Date modifiedTs;

}
