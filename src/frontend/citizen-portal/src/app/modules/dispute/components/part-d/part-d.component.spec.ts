import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { PartDComponent } from './part-d.component';

describe('PartDComponent', () => {
  let component: PartDComponent;
  let fixture: ComponentFixture<PartDComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [PartDComponent],
      providers: [
        {
          provide: MockDisputeService,
          useClass: MockDisputeService,
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartDComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
