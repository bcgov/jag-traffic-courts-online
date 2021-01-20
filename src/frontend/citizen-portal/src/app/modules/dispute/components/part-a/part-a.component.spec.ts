import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { PartAComponent } from './part-a.component';

describe('PartAComponent', () => {
  let component: PartAComponent;
  let fixture: ComponentFixture<PartAComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [PartAComponent],
      providers: [
        {
          provide: MockDisputeService,
          useClass: MockDisputeService,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartAComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
