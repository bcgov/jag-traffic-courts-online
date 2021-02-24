export class DisputeRoutes {
  public static DISPUTE = 'dispute';
  public static FIND = 'find';
  public static STEPPER = 'stepper';
  public static PART_A = 'part-a';
  public static PART_B = 'part-b';
  public static PART_C = 'part-c';
  public static PART_D = 'part-d';
  public static OVERVIEW = 'overview';
  public static REVIEW_TICKET = 'review';
  public static STEP_COUNT = 'count';
  public static STEP_OVERVIEW = 'overview';

  public static MODULE_PATH = DisputeRoutes.DISPUTE;

  public static stepRoutes(): string[] {
    return [
      DisputeRoutes.STEPPER,
      DisputeRoutes.OVERVIEW,
      DisputeRoutes.PART_A,
      DisputeRoutes.PART_B,
      DisputeRoutes.PART_C,
      DisputeRoutes.PART_D,
    ];
  }
  public static routePath(route: string): string {
    return `/${DisputeRoutes.MODULE_PATH}/${route}`;
  }
}
