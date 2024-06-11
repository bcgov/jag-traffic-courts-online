package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of file history (audit log) types that represents a specific description for the log
 * @author 237563
 */
public enum AuditLogEntryType {
	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("UNKNOWN"),
	
	/** Auto reconciliation failed in Justin VT Inbox **/
	ARFL("OCCAM"),

	/** Citizen adds an interpreter request to their hybrid/hearing dispute **/
	CAIN("OCCAM"),

	/** Citizen adds witnesses to their hybrid/hearing dispute **/
	CAWT("OCCAM"),

	/** Citizen has canceled the dispute, status of dispute changed to Cancelled **/
	CCAN("OCCAM"),

	/** Citizen has submitted an update to their contact information **/
	CCON("OCCAM"),

	/** Citizen has changed their Hearing dispute to Written Reasons, all counts **/
	CCWR("OCCAM"),

	/** Citizen adds legal representation information to their hybrid/hearing dispute **/
	CLEG("OCCAM"),

	/** Citizen changes the email associated with their dispute **/
	CUEM("OCCAM"),

	/** Citizen's updated email address has been verified **/
	CUEV("OCCAM"),

	/** Citizen changes the interpreter request in their hybrid/hearing dispute **/
	CUIN("OCCAM"),

	/** Citizen modifies their legal representation information to their hybrid/hearing dispute **/
	CULG("OCCAM"),

	/** Citizen updates their contact information (mailing address, phone number, contact name, contact type) **/
	CUPD("OCCAM"),

	/** Citizen has updated/modified their Written Reasons **/
	CUWR("OCCAM"),

	/** Citizen changes # of witnesses in their hybrid/hearing dispute **/
	CUWT("OCCAM"),

	/** Dispute update request accepted **/
	DURA("OCCAM"),

	/** Dispute update request rejected **/
	DURR("OCCAM"),

	/** Automated cancellation email sent to citizen **/
	EMCA("OCCAM"),

	/** Automated confirmation of submission email sent to citizen **/
	EMCF("OCCAM"),

	/** Automated notification email sent to citizen  **/
	EMCR("OCCAM"),

	/** Automated decision email sent to citizen (includes ROP/decision details) **/
	EMDC("OCCAM"),

	/** Email to citizen was unsuccessful **/
	EMFD("OCCAM"),

	/** Automated processing email sent to citizen (after staff submits to ARC) **/
	EMPR("OCCAM"),

	/** Automated rejection email sent to citizen **/
	EMRJ("OCCAM"),

	/** Citizen has been sent a notification (via email) to prompt them to verify their updated email address **/
	EMRV("OCCAM"),

	/** Citizen/user has been sent a notification (via email) to prompt them to verify their email address **/
	EMST("OCCAM"),

	/** Automated notification sent to citizen to verify the updates/changes to their dispute **/
	EMUP("OCCAM"),

	/** Citizen's email address has been verified **/
	EMVF("OCCAM"),

	/** Email sent to notify Disputant regarding their update request(s) received **/
	ESUR("OCCAM"),

	/** File was deleted by disputant **/
	FDLD("OCCAM"),

	/** File was deleted by staff **/
	FDLS("OCCAM"),

	/** File was uploaded by disputant **/
	FUPD("OCCAM"),

	/** File was uploaded by staff **/
	FUPS("OCCAM"),
	
	/** VTC staff has added a file remark for saving or updating a dispute in Ticket Validation **/
	FRMK("OCCAM"),

	/** Citizen/user submits their ticket resolution request / notice of dispute via TCO **/
	INIT("OCCAM"),

	/** Admin JJ has assigned a dispute to an individual JJ for actioning **/
	JASG("TCO"),

	/** JJ has entered their dispute decision, status changed to Confirmed **/
	JCNF("TCO"),

	/** JJ has diverted the dispute to a court appearance, status changed to JJ Divert to Court Appearance **/
	JDIV("TCO"),

	/** JJ has chosen to save and finish later, status changed to In Progress **/
	JPRG("TCO"),

	/** JUSTIN count not found in OCCAM - Counts not added to TCO **/
	OCNT("OCCAM"),
	
	/** JJ has chosen to recall and open the dispute, status changed to Review **/
	RCLD("TCO"),

	/** Data reconciliation complete, dispute is visible in JJ workbench, status is New **/
	RECN("OCCAM"),

	/** Dispute updated by Staff Admin Support **/
	SADM("TCO"),

	/** Staff have cancelled the dispute, status of dispute changed to Cancelled **/
	SCAN("OCCAM"),

	/** Staff has submitted the dispute to ARC, status of dispute changed to Processing **/
	SPRC("OCCAM"),

	/** Staff has rejected the dispute, status of dispute changed to Rejected **/
	SREJ("OCCAM"),

	/** Citizen has submitted the dispute via the Citizen Portal, status has been changed to New **/
	SUB("OCCAM"),

	/** Staff user scans and uploads a paper document to JUSTIN **/
	SUPL("TCO"),

	/** Staff has validated the OCR details, status of dispute changed to Validated **/
	SVAL("OCCAM"),

	/** Update request(s) submitted for staff review **/
	URSR("OCCAM"),

	/** VTC staff have sent a dispute decision back to a JJ for review, status changed to Review Required **/
	VREV("TCO"),

	/** VTC staff have reviewed a dispute decision and chosen to submit it to JUSTIN, status changed to Accepted **/
	VSUB("TCO");
	
	private final String schema;
	
	AuditLogEntryType(String schema) {
		this.schema = schema;
	}
	
	public String getSchema() {
		return schema;
	}
}
