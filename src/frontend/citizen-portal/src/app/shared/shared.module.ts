import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { NgxMaskModule } from 'ngx-mask';
import { NgxBusyModule } from './modules/ngx-busy/ngx-busy.module';
import { NgxMaterialModule } from './modules/ngx-material/ngx-material.module';
import { NgxProgressModule } from './modules/ngx-progress/ngx-progress.module';
import { CapitalizePipe } from './pipes/capitalize.pipe';
import { DefaultPipe } from './pipes/default.pipe';
import { FirstKeyPipe } from './pipes/first-key.pipe';
import { FormatDatePipe } from './pipes/format-date.pipe';
import { PhonePipe } from './pipes/phone.pipe';
import { PostalPipe } from './pipes/postal.pipe';
import { ReplacePipe } from './pipes/replace.pipe';
import { YesNoPipe } from './pipes/yes-no.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { ConfirmDialogComponent } from './dialogs/confirm-dialog/confirm-dialog.component';
import { DialogContentDirective } from './dialogs/dialog-content.directive';

@NgModule({
  declarations: [
    CapitalizePipe,
    DefaultPipe,
    FirstKeyPipe,
    FormatDatePipe,
    PhonePipe,
    PostalPipe,
    ReplacePipe,
    SafeHtmlPipe,
    YesNoPipe,
    ConfirmDialogComponent,
    DialogContentDirective,
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    NgxBusyModule,
    NgxMaterialModule,
    NgxMaskModule.forRoot(),
    NgxProgressModule,
  ],
  exports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    NgxBusyModule,
    NgxMaterialModule,
    NgxMaskModule,
    NgxProgressModule,
    CapitalizePipe,
    DefaultPipe,
    FirstKeyPipe,
    FormatDatePipe,
    PhonePipe,
    PostalPipe,
    ReplacePipe,
    SafeHtmlPipe,
    YesNoPipe,
    DialogContentDirective,
  ],
  entryComponents: [ConfirmDialogComponent],
})
export class SharedModule {}
