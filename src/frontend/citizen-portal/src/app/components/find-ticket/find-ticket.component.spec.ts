import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { ConfigService } from '@config/config.service';

import { FindTicketComponent } from './find-ticket.component';

describe('FindTicketComponent', () => {
  let component: FindTicketComponent;
  let fixture: ComponentFixture<FindTicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        BrowserAnimationsModule,
        HttpClientTestingModule,
        FormsModule,
        ReactiveFormsModule,
        NgxMaterialModule,
        CdkAccordionModule,
        TranslateModule.forRoot(),
      ],
      declarations: [FindTicketComponent],
      providers: [
        {
          provide: ConfigService,
          useClass: MockConfigService,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FindTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
