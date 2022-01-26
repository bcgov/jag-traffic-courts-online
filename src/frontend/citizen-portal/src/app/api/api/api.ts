export * from './addressAutocomplete.service';
import { AddressAutocompleteService } from './addressAutocomplete.service';
export * from './disputes.service';
import { DisputesService } from './disputes.service';
export * from './lookup.service';
import { LookupService } from './lookup.service';
export * from './tickets.service';
import { TicketsService } from './tickets.service';
export const APIS = [AddressAutocompleteService, DisputesService, LookupService, TicketsService];
