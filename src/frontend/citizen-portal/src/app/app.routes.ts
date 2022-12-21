export class AppRoutes {
  // Ticket
  public static TICKET = 'ticket';
  public static FIND = 'find';
  public static SCAN = 'scan';
  public static SUMMARY = 'summary';
  public static STEPPER = 'stepper';

  public static TICKET_MODULE_PATH = AppRoutes.TICKET;

  public static ticketPath(route: string): string {
    return `/${AppRoutes.TICKET_MODULE_PATH}/${route}`;
  }

  // Dispute
  public static DISPUTE = 'dispute';
  public static FIND_DISPUTE = 'find';
  public static UPDATE_DISPUTE = 'manage';
  public static UPDATE_DISPUTE_AUTH = 'auth';
  public static UPDATE_DISPUTE_CONTACT = 'update/address';

  public static DISPUTE_MODULE_PATH = AppRoutes.DISPUTE;

  // Base
  public static EMAILVERIFICATIONREQUIRED = 'email/verification';
  public static EMAILVERIFICATION = 'email/verify';
  public static SUBMIT_SUCCESS = 'submitted';

  public static disputePath(route: string): string {
    return `/${AppRoutes.DISPUTE_MODULE_PATH}/${route}`;
  }
}
