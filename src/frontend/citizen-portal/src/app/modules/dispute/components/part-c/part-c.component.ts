import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
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
    private mockDisputeService: MockDisputeService
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

    this.mockDisputeService.ticket$.subscribe((ticket: Ticket) => {
      this.form.patchValue(ticket);
    });
  }

  public onSubmit(): void {}

  public onBack() {}

  public get interpreterRequired(): FormControl {
    return this.form.get('interpreterRequired') as FormControl;
  }
}
