import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { Config } from '@config/config.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Observable, Subscription } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { ConfigService } from '@config/config.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';

export function autocompleteObjectValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    if (typeof control.value === 'string') {
      return { invalidAutocompleteObject: { value: control.value } };
    }
    return null; /* valid option selected */
  };
}

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindTicketComponent implements OnInit {
  public busy: Subscription;
  public form: FormGroup;

  public notFound = false;

  public statutes: Config<number>[];
  public filteredStatutes: Observable<Config<number>[]>;

  constructor(
    private route: Router,
    private formBuilder: FormBuilder,
    private disputeResource: DisputeResourceService,
    private configService: ConfigService,
    private formUtilsService: FormUtilsService,
    private disputeService: DisputeService,
    private logger: LoggerService
  ) {
    this.statutes = this.configService.statutes;
  }

  public ngOnInit(): void {
    // const testDefault = this.statutes[5];
    this.form = this.formBuilder.group({
      ticketNumber: ['EZ02000460', [Validators.required]],
      time: ['09:54', [Validators.required]],
      test: [null, [autocompleteObjectValidator()]], // Validators.required,
    });

    this.filteredStatutes = this.test.valueChanges.pipe(
      startWith(''),
      map((value) => (typeof value === 'string' ? value : value.name)),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );
  }

  private filterStatutes(value: string): Config<number>[] {
    const trimValue = value.toLowerCase().replace(/\s+/g, ''); // Get rid of whitespace
    const noBracketValue = trimValue.replace(/[\(\)']+/g, ''); // Get rid of brackets

    if (trimValue === noBracketValue) {
      return this.statutes.filter((option) =>
        option.name
          .toLowerCase()
          .replace(/\s+/g, '') // Get rid of whitespace
          .replace(/[\(\)']+/g, '') // Get rid of brackets
          .includes(noBracketValue)
      );
    }

    return this.statutes.filter((option) =>
      option.name.toLowerCase().replace(/\s+/g, '').includes(trimValue)
    );
  }

  public onDisplayWithStatute(statute?: Config<number>): string | undefined {
    return statute ? statute.name : undefined;
  }

  public onStatuteSelected(event$: MatAutocompleteSelectedEvent): void {
    this.logger.log('onStatuteSelected', event$.option.value);
  }

  public onSearch(): void {
    const validity = this.formUtilsService.checkValidity(this.form);
    const errors = this.formUtilsService.getFormErrors(this.form);

    this.logger.log('validity', validity);
    this.logger.log('errors', errors);
    this.logger.log('form.value', this.form.value);

    if (!validity) {
      return;
    }

    this.notFound = false;

    const formParams = { ...this.form.value };
    this.busy = this.disputeResource
      .getTicket(formParams)
      .subscribe((response) => {
        this.disputeService.ticket$.next(response);

        if (response) {
          this.route.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
            queryParams: formParams,
          });
        } else {
          this.notFound = true;
        }
      });
  }

  public get test(): FormControl {
    return this.form.get('test') as FormControl;
  }
}
