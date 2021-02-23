import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NgBusyModule } from 'ng-busy';

import { BusyOverlayComponent } from '../busy-overlay/busy-overlay.component';
import { busyConfig } from '../busy.config';

describe('BusyOverlayComponent', () => {
  let component: BusyOverlayComponent;
  let fixture: ComponentFixture<BusyOverlayComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        imports: [NgBusyModule.forRoot(busyConfig)],
        declarations: [BusyOverlayComponent],
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(BusyOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit started', () => {
    const component = fixture.componentInstance;
    spyOn(component.started, 'emit');

    component.onBusyStart(true);
    fixture.detectChanges();

    expect(component.started.emit).toHaveBeenCalledWith(true);
  });

  it('should emit started', () => {
    const component = fixture.componentInstance;
    spyOn(component.stopped, 'emit');

    component.onBusyStop(true);
    fixture.detectChanges();

    expect(component.stopped.emit).toHaveBeenCalledWith(true);
  });
});
