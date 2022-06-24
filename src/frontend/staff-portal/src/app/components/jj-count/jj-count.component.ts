import { Component, EventEmitter, Input, Output, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { JJDisputeService, JJFinalDispositionCount } from '../../services/jj-dispute.service';
import { JJDisputeView } from '../../services/jj-dispute.service';
import { Subscription } from 'rxjs';
import { DisputedCount, ViolationTicketCount } from 'app/api';
import { MatRadioChange } from '@angular/material/radio';

@Component({
  selector: 'app-jj-count',
  templateUrl: './jj-count.component.html',
  styleUrls: ['./jj-count.component.scss']
})
export class JJCountComponent implements OnInit {
  @Input() public jjDisputeInfo: JJDisputeView
  @Input() public count: number;
  @Input() public finalDispositionCount: JJFinalDispositionCount;
  @Output() public finalDispositionCountUpdate: EventEmitter<JJFinalDispositionCount> = new EventEmitter<JJFinalDispositionCount>();
  public isMobile: boolean;
  public todayDate: Date = new Date();
  public violationDate: string = "";
  public violationTicketCount: ViolationTicketCount;
  public disputedCount: DisputedCount;
  public form: FormGroup;
  public timeToPay: string = "no";
  public fineReduction: string = "no";
  public inclSurcharge: string = "yes";
  public partialFineAmount: number = 0;
  public surcharge: number = 0;
  
  constructor(
    protected route: ActivatedRoute,
    private utilsService: UtilsService,
    public mockConfigService: MockConfigService,
    public jjDisputeService: JJDisputeService,
    protected formBuilder: FormBuilder,
    private logger: LoggerService,
    @Inject(Router) private router,
    ) {
      const today = new Date();
      this.isMobile = this.utilsService.isMobile();
    }

    ngOnInit(): void {
      this.violationDate = this.jjDisputeInfo.violationDate.split("T")[0];
      this.violationTicketCount = this.jjDisputeInfo.violationTicket.violationTicketCounts.filter(x => x.count == this.count)[0];
      this.disputedCount = this.jjDisputeInfo.disputedCounts.filter(x => x.count == this.count)[0];

      this.form = this.formBuilder.group({
        fineAmount: [null, [Validators.required]],
        partialFineAmount: [null, [Validators.required]],
        dueTs: [null, [Validators.required]],
        comments: [null]
      });

      this.form.patchValue(this.finalDispositionCount);

      // listen for form changes
      this.form.valueChanges.subscribe(() => {
        this.finalDispositionCount.comments = this.form.get('comments').value;
        this.finalDispositionCount.dueTs = this.form.get('dueTs').value;
        this.finalDispositionCount.fineAmount = this.form.get('fineAmount').value;
        this.finalDispositionCountUpdate.emit(this.finalDispositionCount);
      });
      
    }

    onChangePartialFineAmount() {
      // surcharge is always 15%
      if (this.inclSurcharge == "yes") {
        this.form.get('fineAmount').setValue(this.form.get('partialFineAmount').value);
        this.partialFineAmount = this.form.get('partialFineAmount').value / 1.15;
        this.surcharge = 0.15 * this.partialFineAmount;
      } else {
        this.form.get('fineAmount').setValue(this.form.get('partialFineAmount').value *  1.15);
        this.partialFineAmount = this.form.get('partialFineAmount').value;
        this.surcharge = this.form.get('partialFineAmount').value * 0.15;
      }
    }

    updateFineAmount(event: MatRadioChange) {
      // if they select no set it back to ticketed Amount
      // do nothing if yes
      if (event.value == "no") {
        this.form.get('fineAmount').setValue(this.violationTicketCount.ticketedAmount);
      }
    }

    updateInclSurcharge(event: MatRadioChange) {
      // surcharge is always 15%
      if (event.value == "yes") {
        this.form.get('fineAmount').setValue(this.form.get('partialFineAmount').value);
        this.partialFineAmount = this.form.get('partialFineAmount').value / 1.15;
        this.surcharge = 0.15 * this.partialFineAmount;
      } else {
        this.form.get('fineAmount').setValue(this.form.get('partialFineAmount').value *  1.15);
        this.partialFineAmount = this.form.get('partialFineAmount').value;
        this.surcharge = this.form.get('partialFineAmount').value * 0.15;
      }
    }

    updateDueTs(event: MatRadioChange) {
      // if they select no set it back to violation date plus 30 days
      // do nothing if yes
      if (event.value == "no") {
        this.form.get('dueTs').setValue(this.jjDisputeService.addThirtyDays(this.jjDisputeInfo.violationDate));
      }
    }
  }


