export class AuthRoutes {
  public static AUTH = 'auth';
  public static LANDING = 'landing';

  public static MODULE_PATH = AuthRoutes.AUTH;

  /**
   * @description
   * Useful for redirecting to module root-level routes.
   */
  public static routePath(route: string): string {
    return `/${AuthRoutes.MODULE_PATH}/${route}`;
  }
}
