import { Pipe, PipeTransform } from '@angular/core';

import { APP_DATE_FORMAT } from '@shared/modules/ngx-material/ngx-material.module';

@Pipe({
  name: 'formatDate',
})
export class FormatDatePipe implements PipeTransform {
  transform(date: string, format: string = APP_DATE_FORMAT): string {
    if (date) {
      const parts = date.split('-');
      let newDate;
      if (parts.length === 3) {
        newDate = new Date(
          parseInt(parts[0]),
          parseInt(parts[1]) - 1,
          parseInt(parts[2])
        );
      } else {
        newDate = new Date(date);
      }

      date = `${newDate.getDate()} ${newDate.toLocaleString('default', {
        month: 'short',
      })} ${newDate.getFullYear()}`;
    }

    return date;
  }
}
