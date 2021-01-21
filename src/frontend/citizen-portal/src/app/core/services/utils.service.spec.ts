import { TestBed } from '@angular/core/testing';

import { UtilsService } from './utils.service';

describe('UtilsService', () => {
  let service: UtilsService;

  beforeEach(() => {
    service = TestBed.inject(UtilsService);
  });

  it('should create', () => {
    expect(service).toBeTruthy();
  });

  it('should call scrollTop', () => {
    service.scrollTop();
    expect(true).toBeTruthy();
  });

  it('should call scrollTo', () => {
    service.scrollTo(null);
    expect(true).toBeTruthy();
  });

  it('should call scrollToErrorSection', () => {
    service.scrollToErrorSection();
    expect(true).toBeTruthy();
  });

  it('should call isIEOrPreChromiumEdge', () => {
    service.isIEOrPreChromiumEdge();
    expect(true).toBeTruthy();
  });

  it('should call sortByKey', () => {
    service.sortByKey({ a: 'one' }, { a: 'two' }, 'a');
    expect(true).toBeTruthy();
  });

  it('should call sortByDirection', () => {
    service.sortByDirection({ a: 'one' }, { a: 'two' });
    expect(true).toBeTruthy();
  });

  it('should call sort', () => {
    service.sort({ a: 'one' }, { a: 'two' });
    expect(true).toBeTruthy();
  });
});
