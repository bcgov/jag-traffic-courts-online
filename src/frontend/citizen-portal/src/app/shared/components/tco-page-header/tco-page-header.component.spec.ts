import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { Component } from '@angular/core';

import { TcoPageHeaderComponent } from './tco-page-header.component';
@Component({ selector: 'test-blank', template: `` })
class BlankComponent {}

describe('TcoPageHeaderComponent', () => {
  let component: TcoPageHeaderComponent;
  let fixture: ComponentFixture<TcoPageHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
        TranslateModule.forRoot(),
      ],
      declarations: [TcoPageHeaderComponent, BlankComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TcoPageHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
