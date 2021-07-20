import { Pipe, PipeTransform } from '@angular/core';

import { APP_DATE_FORMAT } from '@shared/modules/ngx-material/ngx-material.module';

@Pipe({
  name: 'formatDate',
})
export class FormatDatePipe implements PipeTransform {
  transform(date: string, format: string = APP_DATE_FORMAT): string {
    if (date) {
      let parts = date.split('-');
      let newDate;

      if (parts.length != 3) {
        parts = date.split(' ');
      }

      if (parts.length === 3) {
        newDate = new Date(
          parseInt(parts[0], 10),
          parseInt(parts[1], 10) - 1,
          parseInt(parts[2], 10)
        );
      } else {
        newDate = new Date(date);
      }

      // If not a date, then return the input date as is.
      if (isNaN(newDate)) {
        return date;
      }

      date = `${newDate.getDate()} ${newDate.toLocaleString('default', {
        month: 'short',
      })} ${newDate.getFullYear()}`;
    }

    return date;
  }
}
