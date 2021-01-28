export class SurveyJsRoutes {
  public static SURVEY = 'survey';
  public static HOME = 'home';

  public static MODULE_PATH = SurveyJsRoutes.SURVEY;

  /**
   * @description
   * Useful for redirecting to module root-level routes.
   */
  public static routePath(route: string): string {
    return `/${SurveyJsRoutes.MODULE_PATH}/${route}`;
  }
}
