export class AppRoutes {
  // Ticket
  public static TICKET = 'ticket';
  public static FIND = 'find';
  public static SCAN = 'scan';
  public static SUMMARY = 'summary';
  public static STEPPER = 'dispute';

  public static TICKET_MODULE_PATH = AppRoutes.TICKET;

  public static ticketPath(route: string): string {
    return `/${AppRoutes.TICKET_MODULE_PATH}/${route}`;
  }

  // Dispute
  public static DISPUTE = 'dispute';
  public static FIND_DISPUTE = 'find';
  public static UPDATE_DISPUTE_LANDING = 'manage';
  public static UPDATE_DISPUTE_CONTACT = 'update/contact';
  public static UPDATE_DISPUTE = 'update';

  public static DISPUTE_MODULE_PATH = AppRoutes.DISPUTE;

  // Base
  public static EMAILVERIFICATIONREQUIRED = 'email/verification';
  public static EMAILVERIFICATION = 'email/verify/:token'; //start from external, better use angular route params instead of query string
  public static SUBMIT_SUCCESS = 'submitted';
  
  public static disputePath(route: string): string {
    return `/${AppRoutes.DISPUTE_MODULE_PATH}/${route}`;
  }
}
