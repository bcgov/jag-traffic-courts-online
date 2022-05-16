import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { DatePipe } from '@angular/common';
import { DisputeView, DisputesService } from '../../services/disputes.service';
import { Subscription } from 'rxjs';
import { ConfigService, } from '../../config/config.service';
import { ProvinceConfig } from '@config/config.model';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { Mock } from 'protractor/built/driverProviders';
@Component({
  selector: 'app-ticket-info',
  templateUrl: './ticket-info.component.html',
  styleUrls: ['./ticket-info.component.scss'],
})
export class TicketInfoComponent implements OnInit {
  @Input() public disputeInfo: DisputeView;
  @Output() public backTicketList: EventEmitter<MatStepper> = new EventEmitter();
  public isMobile: boolean;
  public previousButtonIcon = 'keyboard_arrow_left';
  public previousButtonKey = 'stepper.backReview';
  public saveButtonKey = 'stepper.next';
  public busy: Subscription;
  public form: FormGroup;
  public flagsForm: FormGroup;
  public todayDate: Date = new Date();
  public provinces: ProvinceConfig[];
  public states: ProvinceConfig[];
  
  /**
   * @description
   * Whether to show the address line fields.
   */
   public showManualButton: boolean;
   public showAddressFields: boolean;
   public disableForm:boolean = true;
   public collapseObj: any = {
    ticketInformation: true,
    contactInformation: true,
    imageInformation: true
  }
  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    private utilsService: UtilsService,
    private datePipe: DatePipe,
    private logger: LoggerService,
    private disputesService: DisputesService,
    public mockConfigService: MockConfigService
  ) {
    const today = new Date();
    this.isMobile = this.utilsService.isMobile();
    this.showManualButton = true;
    this.showAddressFields = true;
    if (this.mockConfigService.provinces) {
      this.provinces = this.mockConfigService.provinces.filter(x => x.countryCode == 'CA');
      this.states = this.mockConfigService.provinces.filter(x => x.countryCode == 'US');
    }
    console.log(this.provinces, this.states, this.mockConfigService.provinces);
  }

  public ngOnInit() {
    this.flagsForm = this.formBuilder.group({
      disputantOcrIssuesDescription: [null],
      disputantDetectedOcrIssues: [null],
    });
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      homePhoneNumber: [null],
      emailAddress: [null],
      violationDate: [null, [Validators.required]],  // api returns issued date, extract date from that
      violationTime: [null, [Validators.required, Validators.pattern(/^(0[0-9]|1[0-9]|2[0-3])[0-5][0-9]$/)]],  // api returns issued date, extract date from that
      surname: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      country: ["Canada", [Validators.required]], // hard coded this is not returned from API, assumed always Canada
      birthdate: [null], // Optional
      address: [null, [Validators.required]],
      city: [null, [Validators.required]],
      province: [null, [Validators.required]],
      postalCode: [null, [Validators.required]], // space needs to be added back to the middle for display
      driversLicenceNumber: [null, [Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern(/^(0[0-9]|1[0-9]|2[0-3])[0-5][0-9]$/)]],
      driversLicenceProvince: [null, [Validators.required]],
      provincialCourtHearingLocation: [null, [Validators.required]],
      count1Description: [
        null,
        [Validators.required],
      ],
      count1TicketedAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      count2Description: [
        null,
        [Validators.required],
      ],
      count2TicketedAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      count3Description: [
        null,
        [Validators.required],
      ],
      count3TicketedAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      _chargeCount: [1],
      _amountOwing: [null],
    });
    // retreive fresh copy from db
    this.getDispute();
  }

  public onBack() {
    this.backTicketList.emit();
  }

  public onDLProvinceChange(province: string) {
    if (province == 'British Columbia') {
      this.form.get('drivers_licence_number').setValidators([Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern('^(\d{7}|\d{8}|\d{9})$')]);
      this.form.get('drivers_licence_number').updateValueAndValidity();
      this.form.updateValueAndValidity();

    } else {
      this.form.get('drivers_licence_number').setValidators(Validators.required);
      this.form.get('drivers_licence_number').updateValueAndValidity();
      this.form.updateValueAndValidity();
    }
  }

  public onSubmit(): void {
  }

  public handleCollapse(name: string) {
    this.collapseObj[name]= !this.collapseObj[name]
  }
  
  public showManualAddress(): void {
    this.showAddressFields = true;
  }

  public editForm(): void {
    this.disableForm = false;
  }

  getDispute(): void {
    this.logger.log('TicketInfoComponent::getDispute');

    // this.busy = this.disputesService.getDispute(this.disputeInfo.id).subscribe((response: Dispute) => {
    //   this.logger.info(
    //     'TicketInfoComponent::getDispute response',
    //     response
    //   );

    //   this.disputesService.dispute$.next(response);
    // });

    var responseObject = JSON.parse("{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"status\":\"NEW\",\"ticketNumber\":\"AQ92926841\",\"provincialCourtHearingLocation\":\"Franklin\",\"issuedDate\":\"2022-05-11T14:38:15.225Z\",\"submittedDate\":\"2022-05-11T14:38:15.225Z\",\"surname\":\"Dame\",\"givenNames\":\"Lorraine Frances\",\"birthdate\":\"2022-05-11T14:38:15.225Z\",\"driversLicenceProvince\":\"British Columbia\",\"driversLicenceNumber\":\"3333333\",\"address\":\"3-1409 Camosun Street\",\"city\":\"Victoria\",\"province\":\"British Columbia\",\"postalCode\":\"V8V4L5\",\"homePhoneNumber\":\"2222222222\",\"workPhoneNumber\":\"2222222222\",\"emailAddress\":\"lfdpanda@live.ca\",\"filingDate\":\"2022-05-11T14:38:15.225Z\",\"disputedCounts\":[{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"plea\":\"NotGuilty\",\"count\":1,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa7\",\"plea\":\"NotGuilty\",\"count\":2,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa8\",\"plea\":\"NotGuilty\",\"count\":3,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}],\"representedByLawyer\":true,\"legalRepresentation\":{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"lawFirmName\":\"MickeyMouse\",\"lawyerFullName\":\"Mickey\",\"lawyerEmail\":\"1@1.com\",\"lawyerAddress\":\"1234EasyStreet\",\"lawyerPhoneNumber\":\"3333333333\",\"additionalProperties\":{\"additionalProp1\":\"string\",\"additionalProp2\":\"string\",\"additionalProp3\":\"string\"}},\"interpreterLanguage\":\"Albanian\",\"numberOfWitness\":2,\"fineReductionReason\":\"finereductionreason\",\"timeToPayReason\":\"Timetopayreason\",\"rejectedReason\":\"string\",\"disputantDetectedOcrIssues\":true,\"disputantOcrIssuesDescription\":\"Nameincorrect\",\"systemDetectedOcrIssues\":true,\"jjAssigned\":\"Bryan\",\"ocrViolationTicket\":\"string\",\"violationTicket\":{\"violationTicketImage\":{\"image\":\"\",\"mimeType\":{\"name\":\"apple\",\"primaryType\":\"image\",\"subType\":\"pdf\",\"description\":\"descr\",\"extensions\":[\"string\"]}},\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"ticketNumber\":\"AQ92926841\",\"surname\":\"Dame\",\"givenNames\":\"Lorraine\",\"isYoungPerson\":false,\"driversLicenceNumber\":\"33333333\",\"driversLicenceProvince\":\"BritishColumbia\",\"driversLicenceProducedYear\":2020,\"driversLicenceExpiryYear\":2025,\"birthdate\":\"1965-07-01T14:38:15.225Z\",\"address\":\"3-1409CamosunStreet\",\"city\":\"Victoria\",\"province\":\"BritishColumbia\",\"postalCode\":\"V8V4L5\",\"isChangeOfAddress\":false,\"isDriver\":true,\"isCyclist\":false,\"isOwner\":true,\"isPedestrian\":false,\"isPassenger\":false,\"isOther\":false,\"otherDescription\":\"\",\"issuedDate\":\"2022-05-11T14:38:15.225Z\",\"issuedOnRoadOrHighway\":\"CamosunStreet\",\"issuedAtOrNearCity\":\"Victoria\",\"isMvaOffence\":true,\"isWlaOffence\":false,\"isLcaOffence\":false,\"isMcaOffence\":false,\"isFaaOffence\":false,\"isTcrOffence\":false,\"isCtaOffence\":false,\"isOtherOffence\":false,\"otherOffenceDescription\":\"other\",\"organizationLocation\":\"Victoria\",\"violationTicketCounts\":[{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa0\",\"count\":1,\"description\":\"ParkinginahandicapspotwithoutMVAtherequiredsticker\",\"actRegulation\":\"MVA\",\"fullSection\":\"139\",\"section\":\"c\",\"subsection\":\"\",\"paragraph\":\"\",\"ticketedAmount\":140,\"isAct\":false,\"isRegulation\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa1\",\"count\":2,\"description\":\"Distracteddriving\",\"actRegulation\":\"MVA\",\"fullSection\":\"182\",\"section\":\"7\",\"subsection\":\"\",\"paragraph\":\"\",\"ticketedAmount\":110,\"isAct\":false,\"isRegulation\":false,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa2\",\"count\":3,\"description\":\"Exceedlicensedgrossvehicleweight\",\"actRegulation\":\"MVA\",\"fullSection\":\"188\",\"section\":\"b\",\"subsection\":\"2\",\"paragraph\":\"\",\"ticketedAmount\":480,\"isAct\":false,\"isRegulation\":false,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}],\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}")
    this.form.patchValue(responseObject);

    // set violation date
    let violationDate = new Date(responseObject.issuedDate);
    let violationDateString = this.datePipe.transform(violationDate,"yyyy-MM-dd");
    this.form.controls['violationDate'].setValue(violationDateString);

    // set violation time
    let violationTimeString = this.datePipe.transform(violationDate,"hhmm");
    this.form.controls['violationTime'].setValue(violationTimeString);
    
    console.log("ticket-info getDispute", violationDate, violationDateString, violationTimeString);

  }
}
