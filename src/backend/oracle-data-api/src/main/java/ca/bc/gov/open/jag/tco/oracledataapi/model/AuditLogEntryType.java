package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of file history (audit log) types that represents a specific description for the log
 * @author 237563
 */
public enum AuditLogEntryType {
	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN,

	/** Auto reconciliation failed in Justin VT Inbox **/
	ARFL,

	/** Citizen adds an interpreter request to their hybrid/hearing dispute **/
	CAIN,

	/** Citizen adds witnesses to their hybrid/hearing dispute **/
	CAWT,

	/** Citizen has canceled the dispute, status of dispute changed to Cancelled **/
	CCAN,

	/** Citizen has submitted an update to their contact information **/
	CCON,

	/** Citizen has changed their Hearing dispute to Written Reasons, all counts **/
	CCWR,

	/** Citizen adds legal representation information to their hybrid/hearing dispute **/
	CLEG,

	/** Citizen changes the email associated with their dispute **/
	CUEM,

	/** Citizen's updated email address has been verified **/
	CUEV,

	/** Citizen changes the interpreter request in their hybrid/hearing dispute **/
	CUIN,

	/** Citizen modifies their legal representation information to their hybrid/hearing dispute **/
	CULG,

	/** Citizen updates their contact information (mailing address, phone number, contact name, contact type) **/
	CUPD,

	/** Citizen has updated/modified their Written Reasons **/
	CUWR,

	/** Citizen changes # of witnesses in their hybrid/hearing dispute **/
	CUWT,

	/** Dispute update request accepted **/
	DURA,

	/** Dispute update request rejected **/
	DURR,

	/** Automated cancellation email sent to citizen **/
	EMCA,

	/** Automated confirmation of submission email sent to citizen **/
	EMCF,

	/** Automated notification email sent to citizen  **/
	EMCR,

	/** Automated decision email sent to citizen (includes ROP/decision details) **/
	EMDC,

	/** Email to citizen was unsuccessful **/
	EMFD,

	/** Automated processing email sent to citizen (after staff submits to ARC) **/
	EMPR,

	/** Automated rejection email sent to citizen **/
	EMRJ,

	/** Citizen has been sent a notification (via email) to prompt them to verify their updated email address **/
	EMRV,

	/** Citizen/user has been sent a notification (via email) to prompt them to verify their email address **/
	EMST,

	/** Automated notification sent to citizen to verify the updates/changes to their dispute **/
	EMUP,

	/** Citizen's email address has been verified **/
	EMVF,

	/** Email sent to notify Disputant regarding their update request(s) received **/
	ESUR,

	/** File was deleted by disputant **/
	FDLD,

	/** File was deleted by staff **/
	FDLS,

	/** File was uploaded by disputant **/
	FUPD,

	/** File was uploaded by staff **/
	FUPS,
	
	/** VTC staff has added a file remark for saving or updating a dispute in Ticket Validation **/
	FRMK,

	/** Citizen/user submits their ticket resolution request / notice of dispute via TCO **/
	INIT,

	/** Admin JJ has assigned a dispute to an individual JJ for actioning **/
	JASG,

	/** JJ has entered their dispute decision, status changed to Confirmed **/
	JCNF,

	/** JJ has diverted the dispute to a court appearance, status changed to JJ Divert to Court Appearance **/
	JDIV,

	/** JJ has chosen to save and finish later, status changed to In Progress **/
	JPRG,

	/** JUSTIN count not found in OCCAM - Counts not added to TCO **/
	OCNT,
	
	/** JJ has chosen to recall and open the dispute, status changed to Review **/
	RCLD,

	/** Data reconciliation complete, dispute is visible in JJ workbench, status is New **/
	RECN,

	/** Dispute updated by Staff Admin Support **/
	SADM,

	/** Staff have cancelled the dispute, status of dispute changed to Cancelled **/
	SCAN,

	/** Staff has submitted the dispute to ARC, status of dispute changed to Processing **/
	SPRC,

	/** Staff has rejected the dispute, status of dispute changed to Rejected **/
	SREJ,

	/** Citizen has submitted the dispute via the Citizen Portal, status has been changed to New **/
	SUB,

	/** Staff user scans and uploads a paper document to JUSTIN **/
	SUPL,

	/** Staff has validated the OCR details, status of dispute changed to Validated **/
	SVAL,

	/** Update request(s) submitted for staff review **/
	URSR,

	/** VTC staff have sent a dispute decision back to a JJ for review, status changed to Review Required **/
	VREV,

	/** VTC staff have reviewed a dispute decision and chosen to submit it to JUSTIN, status changed to Accepted **/
	VSUB
}
