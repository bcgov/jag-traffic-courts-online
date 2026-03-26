import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HearingInboxFilterService {
  filters = {
    appearanceDate: new Date() as Date | null,
    courthouseLocation: '',
    appearanceRoomCode: ''
  };
}