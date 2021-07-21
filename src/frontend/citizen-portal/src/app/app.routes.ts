export class AppRoutes {
  public static FIND = 'find';
  public static SUCCESS = 'success';
  public static SHELL = 'shell';
  public static SUMMARY = 'summary';
  public static TICKET = 'ticket';
  public static STEPPER = 'stepper';
  public static ALL_STEPPER = 'dispute-all-stepper';
  public static PHOTO = 'photo';

  public static MODULE_PATH = AppRoutes.TICKET;

  public static disputePath(route: string): string {
    return `/${AppRoutes.MODULE_PATH}/${route}`;
  }

  public static routePath(route: string): string {
    return `/${route}`;
  }
}
