import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'default',
  standalone: false,
})
export class DefaultPipe implements PipeTransform {
  transform(value: any, defaultValue: string = '-'): any {
    return (value) ? value : defaultValue;
  }
}
