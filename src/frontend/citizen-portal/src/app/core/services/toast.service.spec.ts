import { TestBed } from '@angular/core/testing';

import { ToastService } from './toast.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('ToastService', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [NgxMaterialModule, BrowserAnimationsModule],
    })
  );

  it('should create', () => {
    const service: ToastService = TestBed.inject(ToastService);
    expect(service).toBeTruthy();
  });

  it('should call success', () => {
    const service: ToastService = TestBed.inject(ToastService);
    service.openSuccessToast('test');
    expect(true).toBeTruthy();
  });

  it('should call error', () => {
    const service: ToastService = TestBed.inject(ToastService);
    service.openErrorToast('test');
    expect(true).toBeTruthy();
  });
});
