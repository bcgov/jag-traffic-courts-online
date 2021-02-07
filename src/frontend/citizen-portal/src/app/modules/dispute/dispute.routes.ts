export class DisputeRoutes {
  public static DISPUTE = 'dispute';
  public static HOME = 'home';
  public static START = 'start';
  public static PART_A = 'part-a';
  public static PART_B = 'part-b';
  public static PART_C = 'part-c';
  public static PART_D = 'part-d';
  public static OVERVIEW = 'overview';

  public static MODULE_PATH = DisputeRoutes.DISPUTE;

  public static stepRoutes(): string[] {
    return [
      DisputeRoutes.HOME,
      DisputeRoutes.PART_A,
      DisputeRoutes.PART_B,
      DisputeRoutes.PART_C,
      DisputeRoutes.PART_D,
      DisputeRoutes.OVERVIEW,
      DisputeRoutes.START,
    ];
  }
  public static routePath(route: string): string {
    return `/${DisputeRoutes.MODULE_PATH}/${route}`;
  }
}
