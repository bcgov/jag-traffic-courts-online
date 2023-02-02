import { TestBed } from '@angular/core/testing';

import { SnowplowService } from './snowplow.service';
import { WindowRefService } from './window-ref.service';

describe('SnowplowService', () => {
  let service: SnowplowService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WindowRefService],
    });
    service = TestBed.inject(SnowplowService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
