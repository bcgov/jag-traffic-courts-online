import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HearingInboxFilterService {
  filters = {
    appearanceDate: new Date(new Date().setHours(0, 0, 0, 0)) as Date | null,
    courthouseLocation: '',
    appearanceRoomCode: ''
  };
}