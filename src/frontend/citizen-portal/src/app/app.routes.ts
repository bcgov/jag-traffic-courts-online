export class AppRoutes {
  public static LANDING = 'landing';
  public static FIND = 'find';
  public static SUCCESS = 'success';
  public static LIST = 'list';
  public static SUMMARY = 'summary';
  public static DISPUTE = 'dispute';
  public static STEPPER = 'stepper';
  public static ALL_STEPPER = 'dispute-all-stepper';
  public static PHOTO = 'photo';

  public static MODULE_PATH = AppRoutes.DISPUTE;

  public static disputePath(route: string): string {
    return `/${AppRoutes.MODULE_PATH}/${route}`;
  }

  public static routePath(route: string): string {
    return `/${route}`;
  }
}
