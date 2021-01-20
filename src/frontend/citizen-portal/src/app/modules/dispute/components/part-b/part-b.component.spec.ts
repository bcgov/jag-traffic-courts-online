import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { PartBComponent } from './part-b.component';

describe('PartBComponent', () => {
  let component: PartBComponent;
  let fixture: ComponentFixture<PartBComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [PartBComponent],
      providers: [
        {
          provide: MockDisputeService,
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
