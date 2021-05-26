import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AppConfigService {

  public version: string;
  public useKeycloak: boolean;

  constructor(private http: HttpClient) {}

  load(): void  {
      const configFile = '/assets/app.config.json';
      console.log('Loading configuration from ' + configFile);
      const promise = this.http.get(configFile)
        .pipe(map(response => {
          Object.assign(this, response);
         }));
  }
}
