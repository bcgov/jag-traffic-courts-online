export class DisputeRoutes {
  public static DISPUTE = 'dispute';
  public static FIND = 'find';
  public static STEPPER = 'stepper';
  public static ALL_STEPPER = 'dispute-all-stepper';
  public static SUCCESS = 'success';
  public static LIST = 'list';
  public static SUMMARY = 'summary';

  public static MODULE_PATH = DisputeRoutes.DISPUTE;

  public static routePath(route: string): string {
    return `/${DisputeRoutes.MODULE_PATH}/${route}`;
  }
}
