import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { DisputeService } from '@dispute/services/dispute.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { PartBComponent } from './part-b.component';

describe('PartBComponent', () => {
  let component: PartBComponent;
  let fixture: ComponentFixture<PartBComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        ReactiveFormsModule,
        RouterModule.forRoot([]),
        BrowserAnimationsModule,
        NgxMaterialModule,
      ],
      declarations: [PartBComponent],
      providers: [
        {
          provide: DisputeService,
          useClass: MockDisputeService,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartBComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
