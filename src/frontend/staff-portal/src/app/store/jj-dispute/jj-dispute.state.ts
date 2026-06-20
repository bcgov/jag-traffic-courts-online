import { JJDispute } from 'app/services/jj-dispute.service';
import { RequestState, RequestStatus } from '../common-state';

export interface JJDisputeState extends RequestState<JJDispute[]> {
  selectedItem: JJDispute | undefined;
}

export const initialState: JJDisputeState = {
  status: RequestStatus.Idle,
  data: undefined,
  selectedItem: undefined,
};
