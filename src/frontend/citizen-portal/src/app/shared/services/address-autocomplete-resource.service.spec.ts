import { TestBed } from '@angular/core/testing';

import { AddressAutocompleteResource } from './address-autocomplete-resource.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';

describe('AddressAutocompleteResource', () => {
  let service: AddressAutocompleteResource;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        MatSnackBarModule
      ],
    });
    service = TestBed.inject(AddressAutocompleteResource);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
