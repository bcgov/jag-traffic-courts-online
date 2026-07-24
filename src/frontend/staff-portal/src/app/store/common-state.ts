export interface RequestState<T> {
    status: RequestStatus,
    data: T | undefined;
}

export enum RequestStatus {
  Idle = 'idle',
  Loading = 'loading',
  Success = 'success',
  Error = 'error',
}
