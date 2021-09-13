export * from './disputeAPI.service';
import { DisputeAPIService } from './disputeAPI.service';
export * from './lookupAPI.service';
import { LookupAPIService } from './lookupAPI.service';
export * from './ticketAPI.service';
import { TicketAPIService } from './ticketAPI.service';
export const APIS = [DisputeAPIService, LookupAPIService, TicketAPIService];
