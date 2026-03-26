import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HearingInboxFilterService {
  filters = {
    appearanceDate: null as Date | null,
    courthouseLocation: '',
    appearanceRoomCode: ''
  };
}