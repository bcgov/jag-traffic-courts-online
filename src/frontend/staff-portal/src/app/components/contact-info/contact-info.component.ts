import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';

@Component({
  selector: 'app-contact-info',
  templateUrl: './contact-info.component.html',
  styleUrls: ['./contact-info.component.scss']
})
export class ContactInfoComponent implements OnInit {
    @Input() public ticketInfo: any;
    @Output() public backTicketList: EventEmitter<MatStepper> = new EventEmitter();
    public isMobile: boolean;
    public previousButtonIcon = 'keyboard_arrow_left';
    public previousButtonKey = 'stepper.backReview';
    public saveButtonKey = 'stepper.next';
  
    public maxDateOfBirth: Date;
    public form: FormGroup;
    public collapseObj: any = {
       contactInformation: true
    }
    private MINIMUM_AGE = 18;
  
    /**
     * @description
     * Whether to show the address line fields.
     */
     public showManualButton: boolean;
     public showAddressFields: boolean;
     public disableForm:boolean = true 
  
    constructor(
      protected route: ActivatedRoute,
      protected router: Router,
      protected formBuilder: FormBuilder,
      private utilsService: UtilsService,
      private logger: LoggerService
    ) {
      const today = new Date();
      this.maxDateOfBirth = new Date();
      this.maxDateOfBirth.setFullYear(today.getFullYear() - this.MINIMUM_AGE);
      this.isMobile = this.utilsService.isMobile();
      this.showManualButton = true;
      this.showAddressFields = true;
    }
  
    public ngOnInit() {
      this.form = this.formBuilder.group({
        violationTicketNumber: [null, [Validators.required]],
        mailingAddress: [null],
        country: [null],
        phoneNumber: [null],
        emailAddress: [null],
        violationDate: [null, [Validators.required]],
        violationTime: [null, [Validators.required]],
        surname: [null, [Validators.required]],
        givenNames: [null, [Validators.required]],
        birthdate: [null], // Optional
        gender: [null, [Validators.required]],
        address: [null, [Validators.required]],
        city: [null, [Validators.required]],
        province: [null, [Validators.required]],
        postalCode: [null, [Validators.required]],
        driverLicenceNumber: [null, [Validators.required]],
        driverLicenceProvince: [null, [Validators.required]],
      });
  
      this.form.patchValue({
        violationTicketNumber: "",
        violationDate: "",
        violationTime: "",
        surname: this.ticketInfo.Surname,
        givenNames: this.ticketInfo.GivenName,
        birthdate: [null], // Optional
        gender: "",
        address: "",
        city: "",
        province: "",
        postalCode: "",
        driverLicenceNumber: "",
        driverLicenceProvince: "",
      })
    }
  
    public onBack() {
      this.backTicketList.emit();
    }

    public handleCollapse(name: string) {
      this.collapseObj[name]= !this.collapseObj[name]
    }
  
    public onSubmit(): void {
    }
  
  
    public showManualAddress(): void {
      this.showAddressFields = true;
    }
  
    public editForm(): void {
      this.disableForm = false;
    }

}
