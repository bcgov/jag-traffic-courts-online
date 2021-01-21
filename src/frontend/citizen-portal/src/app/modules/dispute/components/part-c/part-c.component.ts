import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { Ticket } from '@shared/models/ticket.model';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';

@Component({
  selector: 'app-part-c',
  templateUrl: './part-c.component.html',
  styleUrls: ['./part-c.component.scss'],
})
export class PartCComponent extends BaseDisputeFormPage implements OnInit {
  public form: FormGroup;
  public busy: Subscription;

  constructor(
    protected formBuilder: FormBuilder,
    private service: DisputeResourceService
  ) {
    super(formBuilder);
  }

  public ngOnInit(): void {
    //   {
    //     validators: [
    //       FormGroupValidators.requiredIfTrue(
    //         'interpreterRequired',
    //         'interpreterLanguage'
    //       ),
    //     ],
    //   }

    this.service.ticket$.subscribe((ticket: Ticket) => {
      this.form.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    // do nothing for now
  }

  public onBack(): void {
    // do nothing for now
  }

  public get interpreterRequired(): FormControl {
    return this.form.get('interpreterRequired') as FormControl;
  }
}
