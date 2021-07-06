import { Pipe, PipeTransform } from '@angular/core';

import { APP_DATE_FORMAT } from '@shared/modules/ngx-material/ngx-material.module';

@Pipe({
  name: 'formatDate',
})
export class FormatDatePipe implements PipeTransform {
  transform(date: string, format: string = APP_DATE_FORMAT): string {
    if (date) {
      const newDate = new Date(Date.parse(date));
      date = `${newDate.getDate()}
        ${newDate.toLocaleString('default', {
          month: 'short',
        })} ${newDate.getFullYear()}`;
    }

    return date;
  }
}
