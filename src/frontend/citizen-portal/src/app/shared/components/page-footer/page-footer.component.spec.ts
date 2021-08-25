import {
  async,
  ComponentFixture,
  TestBed,
  waitForAsync,
} from '@angular/core/testing';
import { Component } from '@angular/core';

import { PageFooterComponent } from './page-footer.component';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
@Component({ selector: 'test-blank', template: `` })
class BlankComponent {}

describe('PageFooterComponent', () => {
  let component: PageFooterComponent;
  let fixture: ComponentFixture<PageFooterComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        imports: [
          RouterTestingModule.withRoutes([
            { path: 'ticket/find', component: BlankComponent },
          ]),
          TranslateModule.forRoot(),
        ],
        declarations: [PageFooterComponent, BlankComponent],
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(PageFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit on save', () => {
    spyOn(component.save, 'emit');

    const nativeElement = fixture.nativeElement;
    const button = nativeElement.querySelector('#primaryButton');
    button.dispatchEvent(new Event('click'));

    fixture.detectChanges();

    expect(component.save.emit).toHaveBeenCalled();
  });

  it('should emit on back', () => {
    spyOn(component.back, 'emit');

    const nativeElement = fixture.nativeElement;
    const button = nativeElement.querySelector('#secondaryButton');
    button.dispatchEvent(new Event('click'));

    fixture.detectChanges();

    expect(component.back.emit).toHaveBeenCalled();
  });
});
