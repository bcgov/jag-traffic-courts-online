import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { WindowRefService } from '@core/services/window-ref.service';
import { TranslateModule } from '@ngx-translate/core';
import { AppComponent } from './app.component';

// Added the declaration of BlankComponent to be used for routing
@Component({ selector: 'test-blank', template: `` })
class BlankComponent {}

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
        HttpClientTestingModule,
        TranslateModule.forRoot(),
      ],
      providers: [WindowRefService],
      declarations: [AppComponent, BlankComponent],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });
});
