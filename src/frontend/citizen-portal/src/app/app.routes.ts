export class AppRoutes {
  // Ticket
  public static FIND = 'find';
  public static PAYMENT_COMPLETE = 'paymentComplete';
  public static SUBMIT_SUCCESS = 'submitSuccess';
  public static SCAN = 'scan';
  public static SUMMARY = 'summary';
  public static TICKET = 'ticket';
  public static PAYMENT = 'payment';
  public static STEPPER = 'stepper';
  public static PHOTO = 'photo';
  public static EMAILVERIFICATIONREQUIRED = 'emailVerificationRequired';
  public static EMAILVERIFICATION = 'email/verify';

  public static TICKET_MODULE_PATH = AppRoutes.TICKET;

  public static ticketPath(route: string): string {
    return `/${AppRoutes.TICKET_MODULE_PATH}/${route}`;
  }

  // Dispute
  public static DISPUTE = 'dispute';
  public static FIND_DISPUTE = 'find';
  public static UPDATE_DISPUTE = 'manage';
  public static UPDATE_DISPUTE_AUTH = 'auth';

  public static DISPUTE_MODULE_PATH = AppRoutes.DISPUTE;

  public static disputePath(route: string): string {
    return `/${AppRoutes.DISPUTE_MODULE_PATH}/${route}`;
  }
}
