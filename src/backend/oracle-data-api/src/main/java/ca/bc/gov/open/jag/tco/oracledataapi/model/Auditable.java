package ca.bc.gov.open.jag.tco.oracledataapi.model;

import static javax.persistence.TemporalType.TIMESTAMP;

import java.util.Date;

import javax.persistence.EntityListeners;
import javax.persistence.MappedSuperclass;
import javax.persistence.Temporal;

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
@MappedSuperclass
@Getter
@Setter
@EntityListeners(AuditingEntityListener.class)
public abstract class Auditable<U> {

	/** The username of the individual (or system) who created this record. */
	@CreatedBy
	@Schema(nullable = false)
	private U enteredUserId;

	/** The timestamp this record was created. */
	@CreatedDate
	@Schema(nullable = false)
	@Temporal(TIMESTAMP)
	private Date enteredTs;

	/** The username of the individual (or system) who modified this record. */
	@LastModifiedBy
	@Schema(nullable = true)
	private U updatedUserId;

	/** The timestamp this record was last modified. */
	@LastModifiedDate
	@Schema(nullable = true)
	@Temporal(TIMESTAMP)
	private Date updatedTs;

}
