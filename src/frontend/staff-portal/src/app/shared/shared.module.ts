import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ConfigModule } from 'app/config/config.module';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import { AlertComponent } from './components/alert/alert.component';
import { HeaderComponent } from './components/header/header.component';
import { PageHeaderComponent } from './components/page-header/page-header.component';
import { PageComponent } from './components/page/page.component';
import { ConfirmDialogComponent } from './dialogs/confirm-dialog/confirm-dialog.component';
import { MoreOptionsDialogComponent } from './dialogs/more-options-dialog/more-options-dialog.component';
import { ConfirmReasonDialogComponent } from './dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { DialogContentDirective } from './dialogs/dialog-content.directive';
import { TicketImageDialogComponent } from './dialogs/ticket-image-dialog/ticket-image-dialog.component';
import { FeatureFlagDirective } from './directives/feature-flag.directive';
import { NgxBusyModule } from './modules/ngx-busy/ngx-busy.module';
import { NgxMaterialModule } from './modules/ngx-material/ngx-material.module';
import { NgxProgressModule } from './modules/ngx-progress/ngx-progress.module';
import { CapitalizePipe } from './pipes/capitalize.pipe';
import { DefaultPipe } from './pipes/default.pipe';
import { PhonePipe } from './pipes/phone.pipe';
import { PostalPipe } from './pipes/postal.pipe';
import { ReplacePipe } from './pipes/replace.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { YesNoPipe } from './pipes/yes-no.pipe';
import { CustomDatePipe } from './pipes/custom-date.pipe';
import { FlexLayoutModule } from '@angular/flex-layout';
import { TicketImageContainerComponent } from './dialogs/ticket-image-container/ticket-image-container.component';

@NgModule({
    declarations: [
        CapitalizePipe,
        DefaultPipe,
        PhonePipe,
        CustomDatePipe,
        ReplacePipe,
        SafeHtmlPipe,
        YesNoPipe,
        PostalPipe,
        ConfirmDialogComponent,
        MoreOptionsDialogComponent,
        ConfirmReasonDialogComponent,
        DialogContentDirective,
        PageComponent,
        PageHeaderComponent,
        DialogContentDirective,
        AlertComponent,
        HeaderComponent,
        FeatureFlagDirective,
        TicketImageDialogComponent,
        TicketImageContainerComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        ReactiveFormsModule,
        NgxBusyModule,
        NgxMaterialModule,
        NgxMaskDirective, 
        NgxMaskPipe,
        NgxProgressModule,
        ConfigModule,
        TranslateModule,
        FlexLayoutModule
    ],
    exports: [
        CommonModule,
        RouterModule,
        ReactiveFormsModule,
        NgxBusyModule,
        NgxMaterialModule,
        NgxMaskDirective, 
        NgxMaskPipe,
        NgxProgressModule,
        CapitalizePipe,
        DefaultPipe,
        PhonePipe,
        CustomDatePipe,
        ReplacePipe,
        SafeHtmlPipe,
        YesNoPipe,
        PostalPipe,
        DialogContentDirective,
        PageComponent,
        PageHeaderComponent,
        DialogContentDirective,
        AlertComponent,
        ConfigModule,
        HeaderComponent,
        FeatureFlagDirective,
        FlexLayoutModule,
        TicketImageContainerComponent
    ],
    providers: [provideNgxMask()]
})
export class SharedModule { }
