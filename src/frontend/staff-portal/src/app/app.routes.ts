export class AppRoutes {
  public static LANDING = "";
  public static UNAUTHORIZED = "unauthorized";

  // TCO staff
  public static STAFF = "tco";
  // public static STAFF_WORKBENCH = "workbench";

  public static STAFF_MODULE_PATH = AppRoutes.STAFF;

  public static staffPath(route: string): string {
    return `/${AppRoutes.STAFF_MODULE_PATH}/${route}`;
  }

  // JJ
  public static JJ = "jj";
  // public static JJ_WORKBENCH = "workbench";

  public static JJ_MODULE_PATH = AppRoutes.JJ;

  public static jjPath(route: string): string {
    return `/${AppRoutes.JJ_MODULE_PATH}/${route}`;
  }
}
