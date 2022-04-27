import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FileUtilsService {
  constructor() {
  }

  public readFileAsDataURL(blob: Blob): Observable<string> {
    return Observable.create(obs => {
      const reader = new FileReader();
  
      reader.onerror = err => obs.error(err);
      reader.onabort = err => obs.error(err);
      reader.onload = () => obs.next(reader.result);
      reader.onloadend = () => obs.complete();
  
      return reader.readAsDataURL(blob);
    });
  }
}
