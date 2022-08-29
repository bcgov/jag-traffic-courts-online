export * from './dispute.service';
import { DisputeService } from './dispute.service';
export * from './jJ.service';
import { JJService } from './jJ.service';
export * from './keycloak.service';
import { KeycloakService } from './keycloak.service';
export * from './lookup.service';
import { LookupService } from './lookup.service';
export const APIS = [DisputeService, JJService, KeycloakService, LookupService];
