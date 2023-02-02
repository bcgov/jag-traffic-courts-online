import { DatePipe } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

import { ResolutionHeaderComponent } from './resolution-header.component';

describe('ResolutionHeaderComponent', () => {
  let component: ResolutionHeaderComponent;
  let fixture: ComponentFixture<ResolutionHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ResolutionHeaderComponent ],
      providers: [
        DatePipe,
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({
              ticketNumber: 123,
              time: '12:00',
            }),
            snapshot: {
              queryParams: {
                ticketNumber: 123,
                time: '12:00',
              },
            }
          },
        },
      ],
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResolutionHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
