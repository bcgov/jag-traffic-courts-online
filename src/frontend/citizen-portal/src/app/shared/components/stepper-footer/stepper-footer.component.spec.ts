import { Component } from '@angular/core';
import {
  ComponentFixture,
  TestBed,
  waitForAsync
} from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { StepperFooterComponent } from './stepper-footer.component';

@Component({ selector: 'app-test-blank', template: `` })
class BlankComponent { }

describe('StepperFooterComponent', () => {
  let component: StepperFooterComponent;
  let fixture: ComponentFixture<StepperFooterComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        imports: [
          RouterTestingModule.withRoutes([
            { path: 'ticket/find', component: BlankComponent },
          ]),
          TranslateModule.forRoot(),
        ],
        declarations: [StepperFooterComponent, BlankComponent],
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(StepperFooterComponent);
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
