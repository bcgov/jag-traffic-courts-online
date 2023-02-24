export * from './disputes.service';
import { DisputesService } from './disputes.service';
export * from './document.service';
import { DocumentService } from './document.service';
export * from './lookup.service';
import { LookupService } from './lookup.service';
export * from './tickets.service';
import { TicketsService } from './tickets.service';
export const APIS = [DisputesService, DocumentService, LookupService, TicketsService];
