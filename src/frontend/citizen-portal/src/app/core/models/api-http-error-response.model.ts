export class ApiHttpErrorResponse {
  readonly status: number;
  readonly errors: any;
  readonly message?: string;

  constructor(status: number, error: any, message?: string) {
    this.status = status;
    this.errors = error.errors ? error.errors : error;
    this.message = message;
  }
}
