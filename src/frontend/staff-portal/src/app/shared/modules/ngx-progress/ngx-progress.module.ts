import { NgModule } from '@angular/core';

import { NgProgressbar } from 'ngx-progressbar';
import { NgProgressHttp } from 'ngx-progressbar/http';

@NgModule({
  imports: [NgProgressbar, NgProgressHttp],
  exports: [NgProgressbar, NgProgressHttp],
})
export class NgxProgressModule {}
