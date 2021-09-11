import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ConfigModule } from 'app/config/config.module';
import { NgxMaskModule } from 'ngx-mask';
import { AlertComponent } from './components/alert/alert.component';
import { FooterComponent } from './components/footer/footer.component';
import { HeaderComponent } from './components/header/header.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { PageComponent } from './components/page/page.component';
import { ResolutionFooterComponent } from './components/resolution-footer/resolution-footer.component';
import { ResolutionHeaderComponent } from './components/resolution-header/resolution-header.component';
import { StepperFooterComponent } from './components/stepper-footer/stepper-footer.component';
import { StepperPageComponent } from './components/stepper-page/stepper-page.component';
import { TcoPageHeaderComponent } from './components/tco-page-header/tco-page-header.component';
import { ConfirmDialogComponent } from './dialogs/confirm-dialog/confirm-dialog.component';
import { DialogContentDirective } from './dialogs/dialog-content.directive';
import { TicketExampleDialogComponent } from './dialogs/ticket-example-dialog/ticket-example-dialog.component';
import { FeatureFlagDirective } from './directives/feature-flag.directive';
import { NgxBusyModule } from './modules/ngx-busy/ngx-busy.module';
import { NgxMaterialModule } from './modules/ngx-material/ngx-material.module';
import { NgxProgressModule } from './modules/ngx-progress/ngx-progress.module';
import { CapitalizePipe } from './pipes/capitalize.pipe';
import { DefaultPipe } from './pipes/default.pipe';
import { FormatDatePipe } from './pipes/format-date.pipe';
import { PhonePipe } from './pipes/phone.pipe';
import { PostalPipe } from './pipes/postal.pipe';
import { ReplacePipe } from './pipes/replace.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { YesNoPipe } from './pipes/yes-no.pipe';

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
    StepperFooterComponent,
    PageHeaderComponent,
    TcoPageHeaderComponent,
    DialogContentDirective,
    AlertComponent,
    HeaderComponent,
    FooterComponent,
    FeatureFlagDirective,
    TicketExampleDialogComponent,
    StepperPageComponent,
    ResolutionFooterComponent,
    ResolutionHeaderComponent,
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
    StepperFooterComponent,
    PageHeaderComponent,
    TcoPageHeaderComponent,
    DialogContentDirective,
    AlertComponent,
    ConfigModule,
    HeaderComponent,
    FooterComponent,
    ResolutionFooterComponent,
    ResolutionHeaderComponent,
    FeatureFlagDirective,
    StepperPageComponent,
  ],
  entryComponents: [ConfirmDialogComponent],
})
export class SharedModule { }
