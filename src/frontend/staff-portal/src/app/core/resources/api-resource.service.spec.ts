import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { ApiResource } from './api-resource.service';

describe('ApiResource', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ApiResource],
    });
  });

  it('should be created', inject([ApiResource], (service: ApiResource) => {
    expect(service).toBeTruthy();
  }));
});
