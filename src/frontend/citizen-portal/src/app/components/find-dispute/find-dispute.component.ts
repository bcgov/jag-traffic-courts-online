import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';
import { filter, Subscription } from 'rxjs';
import { DisputeStore, selectLoadingDispute } from 'app/store';
import { select, Store } from '@ngrx/store';

@Component({
  selector: 'app-find-dispute',
  templateUrl: './find-dispute.component.html',
  styleUrls: ['./find-dispute.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindDisputeComponent implements OnInit {
  private progressRef: NgProgressRef;
  busy: Subscription;
  form: FormGroup;

  notFound = false;

  constructor(
    private formBuilder: FormBuilder,
    private formUtilsService: FormUtilsService,
    private ngProgress: NgProgress,
    private logger: LoggerService,
    private store: Store
  ) {
  }

  ngOnInit(): void {
    this.progressRef = this.ngProgress.ref();

    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      time: [null, [Validators.required]],
    });
  }

  onSearch(): void {
    this.logger.log('FindDisputeComponent::onSearch');

    this.notFound = false;
    const validity = this.formUtilsService.checkValidity(this.form);
    const errors = this.formUtilsService.getFormErrors(this.form);

    this.logger.log('validity', validity);
    this.logger.log('errors', errors);
    this.logger.log('form.value', this.form.value);

    if (!validity) {
      return;
    }
    this.store.dispatch(DisputeStore.Actions.Search({ params: this.form.value }));
    this.busy = this.store.pipe(select(selectLoadingDispute), filter(i => !i)).subscribe(() => this.busy.unsubscribe());
  }
}
