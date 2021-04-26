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
import { FormatDatePipe } from './pipes/format-date.pipe';
import { PhonePipe } from './pipes/phone.pipe';
import { ReplacePipe } from './pipes/replace.pipe';
import { YesNoPipe } from './pipes/yes-no.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { ConfirmDialogComponent } from './dialogs/confirm-dialog/confirm-dialog.component';
import { DialogContentDirective } from './dialogs/dialog-content.directive';
import { PageComponent } from './components/page/page.component';
import { PageFooterComponent } from './components/page-footer/page-footer.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { AlertComponent } from './components/alert/alert.component';
import { ConfigModule } from 'app/config/config.module';
import { PostalPipe } from './pipes/postal.pipe';
import { TcoPageHeaderComponent } from './components/tco-page-header/tco-page-header.component';
import { HeaderComponent } from './components/header/header.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [
    CapitalizePipe,
    DefaultPipe,
    FormatDatePipe,
    PhonePipe,
    ReplacePipe,
    SafeHtmlPipe,
    YesNoPipe,
    PostalPipe,
    ConfirmDialogComponent,
    DialogContentDirective,
    PageComponent,
    PageFooterComponent,
    PageHeaderComponent,
    TcoPageHeaderComponent,
    DialogContentDirective,
    AlertComponent,
    HeaderComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    NgxBusyModule,
    NgxMaterialModule,
    NgxMaskModule.forRoot(),
    NgxProgressModule,
    ConfigModule,
    TranslateModule,
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
    FormatDatePipe,
    PhonePipe,
    ReplacePipe,
    SafeHtmlPipe,
    YesNoPipe,
    PostalPipe,
    DialogContentDirective,
    PageComponent,
    PageFooterComponent,
    PageHeaderComponent,
    TcoPageHeaderComponent,
    DialogContentDirective,
    AlertComponent,
    ConfigModule,
    HeaderComponent,
  ],
  entryComponents: [ConfirmDialogComponent],
})
export class SharedModule {}
