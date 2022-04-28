export class AppRoutes {
  public static FIND = 'find';
  public static PAYMENT_COMPLETE = 'paymentComplete';
  public static SUBMIT_SUCCESS = 'submitSuccess';
  public static SHELL = 'shell';
  public static SUMMARY = 'summary';
  public static TICKET = 'ticket';
  public static PAYMENT = 'payment';
  public static STEPPER = 'stepper';
  public static PHOTO = 'photo';

  public static routePath(route: string): string {
    return `/${route}`;
  }
}
