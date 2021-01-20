import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCheckbox, MatCheckboxModule } from '@angular/material/checkbox';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { PartCComponent } from './part-c.component';

describe('PartCComponent', () => {
  let component: PartCComponent;
  let fixture: ComponentFixture<PartCComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, MatCheckboxModule],
      declarations: [PartCComponent],
      providers: [
        {
          provide: MockDisputeService,
          useClass: MockDisputeService,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
