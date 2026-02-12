import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JjDisputeUpdatesComponent } from './jj-dispute-updates.component';
import { AuthService } from 'app/services/auth.service';
import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';

describe('JjDisputeUpdatesComponent', () => {
  let component: JjDisputeUpdatesComponent;
  let fixture: ComponentFixture<JjDisputeUpdatesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [JjDisputeUpdatesComponent],
      imports: [],
      providers: [
        {
          provide: AuthService,
          useValue: {
            token: 'TOKEN',
          },
        },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
      ],
    });
    fixture = TestBed.createComponent(JjDisputeUpdatesComponent);
    fixture.componentInstance.disputeId = 999;
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
