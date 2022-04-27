import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';

import { EMPTY } from 'rxjs';
import { debounceTime, exhaustMap, switchMap } from 'rxjs/operators';

import { ToastService } from '@core/services/toast.service';
import { Address } from '@shared/models/address.model';

@Component({
  selector: 'app-address-form-autocomplete',
  templateUrl: './address-autocomplete.component.html',
  styleUrls: ['./address-autocomplete.component.scss']
})
export class AddressAutocompleteComponent implements OnInit {
  @Input() inBc: boolean;
  @Output() autocompleteAddress: EventEmitter<Address>;

  public form: FormGroup;
  public addressAutocompleteFields: any[];

  constructor(
    private fb: FormBuilder,
  ) {
    this.autocompleteAddress = new EventEmitter<Address>();
    this.inBc = false;
  }

  public get autocomplete(): FormControl {
    return this.form.get('autocomplete') as FormControl;
  }

  public onAutocomplete(id: string) {
  }

  public ngOnInit(): void {
    this.form = this.fb.group({ autocomplete: [''] });
  }
}
