package ca.bc.gov.open.cto;

public class Constants {

    //drivers
    public static final String CHROME_DRIVER = "CHROME_DRIVER";
    public static final String GHOST_DRIVER = "GHOST_DRIVER";
    public static final String FIREFOX_DRIVER = "FIREFOX_DRIVER";
    public static final String IE_DRIVER = "IE_DRIVER";
    public static final String EDGE_DRIVER = "EDGE_DRIVER";

    //enviroments
    public static final String DEV = "DEV";
    public static final String TST = "TST";


    //others
    public static final String NOT_SET = "NOT_SET";
    public static final String JDBCURL = "JDBCURL";

    // xpath
    public static final String COURT_1_FINE_REDUCTION_XPATH = "//strong[contains(text(), 'Count 1')]/../../../../../..//span[contains(text(), 'request a fine reduction')]";
    public static final String COURT_2_TIME_TO_PAY_XPATH = "//strong[contains(text(), 'Count 2')]/../../../../../..//span[contains(text(), 'request time to pay')]";
    public static final String COURT_3_SKIP_XPATH = "//strong[contains(text(), 'Count 3')]/../../../../../..//span[contains(text(), 'Skip this count')]";
	public static final String DISPUTANT_SURNAME_XPATH = "//input[@formcontrolname='disputant_surname']";
	public static final String DISPUTANT_NAME_XPATH = "//input[@formcontrolname='disputant_given_names']";
	public static final String ADDRESS_XPATH = "//input[@formcontrolname='address']";
	public static final String CITY_XPATH = "//input[@formcontrolname='address_city']";
	public static final String PROVINCE_XPATH = "//input[@formcontrolname='address_province']";
	public static final String POSTAL_CODE_XPATH = "//input[@formcontrolname='postal_code']";
	public static final String EMAIL_XPATH = "//input[@formcontrolname='email_address']";
	public static final String PHONE_XPATH = "//input[@formcontrolname='home_phone_number']";
	public static final String LICENSE_XPATH = "//input[@formcontrolname='drivers_licence_number']";
	public static final String CONTACT_LAW_FIRM_NAME_XPATH = "//input[@formcontrolname='contact_law_firm_name']";
	public static final String LAW_FIRM_NAME_XPATH = "//input[@formcontrolname='law_firm_name']";
	public static final String LAWYER_FULL_NAME_XPATH = "//input[@formcontrolname='lawyer_full_name']";
	public static final String LAWYER_ADDRESS_XPATH = "//input[@formcontrolname='lawyer_address']";
	public static final String LAWYER_PHONE_XPATH = "//input[@formcontrolname='lawyer_phone_number']";
	public static final String LAWYER_EMAIL_XPATH = "//input[@formcontrolname='lawyer_email']";
	public static final String CONTACT_NAME_XPATH = "//input[@formcontrolname='contact_given_names']";
	public static final String CONTACT_SURNAME_XPATH = "//input[@formcontrolname='contact_surname']";
	public static final String FINE_REDUCTION_REASON_XPATH = "//input[@formcontrolname='fine_reduction_reason']";
	public static final String TIME_TO_PAY_REASON_XPATH = "//input[@formcontrolname='time_to_pay_reason']";
}
