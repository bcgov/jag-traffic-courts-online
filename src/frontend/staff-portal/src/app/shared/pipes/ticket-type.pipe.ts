import { Pipe, PipeTransform } from '@angular/core';
import { TicketTypes } from '@shared/enums/ticket-type.enum';

@Pipe({
  name: 'ticketType',
})
export class TicketTypePipe implements PipeTransform {
  transform(value: any, explicit: string = null): string {
    // Null check to allow for default pipe chaining, but allow
    // for an explicit yes or no if required
    if (value === 'S') {
      return TicketTypes.CAMERA_TICKET;
    } else if (value === 'E') {
      return TicketTypes.ELECTRONIC_TICKET;
    } else if (value === 'A') {
      return TicketTypes.HANDWRITTEN_TICKET;
    }
    return null;
  }
}
