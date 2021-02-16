import { TestBed } from '@angular/core/testing';

import { ViewportService } from './viewport.service';

describe('ViewportService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should create', () => {
    const service: ViewportService = TestBed.inject(ViewportService);
    expect(service).toBeTruthy();
  });

  it('should be is mobile', () => {
    const service: ViewportService = TestBed.inject(ViewportService);
    const after = service.isMobile;
    expect(after).toBeDefined();
  });

  it('should be is tablet', () => {
    const service: ViewportService = TestBed.inject(ViewportService);
    const after = service.isTablet;
    expect(after).toBeDefined();
  });

  it('should be is desktop', () => {
    const service: ViewportService = TestBed.inject(ViewportService);
    const after = service.isDesktop;
    expect(after).toBeDefined();
  });

  it('should be is wide desktop', () => {
    const service: ViewportService = TestBed.inject(ViewportService);
    const after = service.isWideDesktop;
    expect(after).toBeDefined();
  });
});
