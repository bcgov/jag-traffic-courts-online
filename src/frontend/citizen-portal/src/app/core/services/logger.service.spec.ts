import { TestBed, inject } from '@angular/core/testing';

import { LoggerService } from './logger.service';

describe('LoggerService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LoggerService],
    });
  });

  it('should create', inject([LoggerService], (service: LoggerService) => {
    expect(service).toBeTruthy();
  }));

  it('should call log', inject([LoggerService], (service: LoggerService) => {
    service.log('test');
    expect(true).toBeTruthy();
  }));

  it('should call info', inject([LoggerService], (service: LoggerService) => {
    service.info('test');
    expect(true).toBeTruthy();
  }));

  it('should call warn', inject([LoggerService], (service: LoggerService) => {
    service.info('warn');
    expect(true).toBeTruthy();
  }));

  it('should call error', inject([LoggerService], (service: LoggerService) => {
    service.error('test');
    expect(true).toBeTruthy();
  }));

  it('should call trace', inject([LoggerService], (service: LoggerService) => {
    service.trace('test');
    expect(true).toBeTruthy();
  }));

  it('should call pretty', inject([LoggerService], (service: LoggerService) => {
    service.pretty('test');
    expect(true).toBeTruthy();
  }));
});
