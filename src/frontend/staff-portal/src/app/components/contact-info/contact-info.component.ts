import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ProvinceConfig, Config } from '@config/config.model';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { Dispute } from 'app/api';
import { DisputeView, DisputesService } from '../../services/disputes.service';
import { Subscription } from 'rxjs';
@Component({
  selector: 'app-contact-info',
  templateUrl: './contact-info.component.html',
  styleUrls: ['./contact-info.component.scss']
})
export class ContactInfoComponent implements OnInit {
  @Input() public disputeInfo: DisputeView;
  @Output() public backTicketList: EventEmitter<MatStepper> = new EventEmitter();
  public isMobile: boolean;
  public provinces: ProvinceConfig[];
  public states: ProvinceConfig[];
  public busy: Subscription;
  public initialDisputeValues: Dispute;
  public todayDate: Date = new Date();
  public lastUpdatedDispute: Dispute;

  public form: FormGroup;
  public collapseObj: any = {
    contactInformation: true
  }

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    private utilsService: UtilsService,
    public mockConfigService: MockConfigService,
    private disputesService: DisputesService,
    private logger: LoggerService
  ) {
    const today = new Date();
    this.isMobile = this.utilsService.isMobile();

    if (this.mockConfigService.provinces) {
      this.provinces = this.mockConfigService.provinces.filter(x => x.countryCode == 'CA');
      this.states = this.mockConfigService.provinces.filter(x => x.countryCode == 'US');
    }
  }

  public ngOnInit() {
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      homePhoneNumber: [null, [Validators.required]],
      emailAddress: [null, [Validators.email, Validators.required]],
      surname: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      birthdate: [null, [Validators.required]], // Optional
      address: [null, [Validators.required]],
      city: [null, [Validators.required]],
      province: [null, [Validators.required]],
      country: ["Canada", [Validators.required]],
      postalCode: [null, [Validators.required]],
      driversLicenceNumber: [null, [Validators.required]],
      driversLicenceProvince: [null, [Validators.required]],
    });
    this.getDispute();
  }
  
  public onSubmit(): void {
    this.putDispute({...this.lastUpdatedDispute, ...this.form.value});
  }


  onKeyPressNumbers(event: any, BCOnly: boolean) {
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57) && BCOnly) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
  }

  // change validators on drivers licence number in notice of dispute when changing province / state
  public onNoticeOfDisputeDLProvinceChange(province: string) {
    if (province == 'British Columbia') 
      this.form.get('driversLicenceNumber').setValidators([Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern(/^(\d{7}|\d{8}|\d{9})$/)]);
    else
      this.form.get('driversLicenceNumber').setValidators(Validators.required);
    this.form.get('driversLicenceNumber').updateValueAndValidity();
    this.form.updateValueAndValidity
  }

  // put dispute by id
  putDispute(dispute: Dispute): void {
    this.logger.log('TicketInfoComponent::putDispute', dispute);

    this.busy = this.disputesService.putDispute(dispute.id, dispute).subscribe((response: Dispute) => {
      this.logger.info(
        'TicketInfoComponent::putDispute response',
        response
      );

      // this structure contains last version of what was send to db
      this.lastUpdatedDispute = response;
    });

  }

  // get dispute by id
  getDispute(): void {
    this.logger.log('TicketInfoComponent::getDispute');

    this.busy = this.disputesService.getDispute(this.disputeInfo.id).subscribe((response: Dispute) => {
      this.logger.info(
        'TicketInfoComponent::getDispute response',
        response
      );

      // var responseObject = JSON.parse("{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"status\":\"NEW\",\"ticketNumber\":\"AQ92926841\",\"provincialCourtHearingLocation\":\"Franklin\",\"issuedDate\":\"2022-05-11T14:38:15.225Z\",\"submittedDate\":\"2022-05-11T14:38:15.225Z\",\"surname\":\"Dame\",\"givenNames\":\"Lorraine Frances\",\"birthdate\":\"2022-05-11T14:38:15.225Z\",\"driversLicenceProvince\":\"British Columbia\",\"driversLicenceNumber\":\"3333333\",\"address\":\"3-1409 Camosun Street\",\"city\":\"Victoria\",\"province\":\"British Columbia\",\"postalCode\":\"V8V4L5\",\"homePhoneNumber\":\"2222222222\",\"workPhoneNumber\":\"2222222222\",\"emailAddress\":\"lfdpanda@live.ca\",\"filingDate\":\"2022-05-11T14:38:15.225Z\",\"disputedCounts\":[{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"plea\":\"NOT_GUILTY\",\"count\":1,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa7\",\"plea\":\"NOT_GUILTY\",\"count\":2,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa8\",\"plea\":\"NOT_GUILTY\",\"count\":3,\"requestTimeToPay\":false,\"requestReduction\":false,\"appearInCourt\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}],\"representedByLawyer\":true,\"legalRepresentation\":{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"lawFirmName\":\"MickeyMouse\",\"lawyerFullName\":\"Mickey\",\"lawyerEmail\":\"1@1.com\",\"lawyerAddress\":\"1234EasyStreet\",\"lawyerPhoneNumber\":\"3333333333\",\"additionalProperties\":{\"additionalProp1\":\"string\",\"additionalProp2\":\"string\",\"additionalProp3\":\"string\"}},\"interpreterLanguage\":\"Albanian\",\"numberOfWitness\":2,\"fineReductionReason\":\"finereductionreason\",\"timeToPayReason\":\"Timetopayreason\",\"rejectedReason\":\"string\",\"disputantDetectedOcrIssues\":true,\"disputantOcrIssuesDescription\":\"Name incorrect WWWWWWW\",\"systemDetectedOcrIssues\":true,\"jjAssigned\":\"Bryan\",\"ocrViolationTicket\":\"string\",\"violationTicket\":{\"violationTicketImage\":{\"image\":\"\",\"mimeType\":{\"name\":\"apple\",\"primaryType\":\"image\",\"subType\":\"pdf\",\"description\":\"descr\",\"extensions\":[\"string\"]}},\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"ticketNumber\":\"AQ92926841\",\"surname\":\"Dame\",\"givenNames\":\"Lorraine\",\"isYoungPerson\":false,\"driversLicenceNumber\":\"33333333\",\"driversLicenceProvince\":\"British Columbia\",\"driversLicenceProducedYear\":2020,\"driversLicenceExpiryYear\":2025,\"birthdate\":\"1965-07-01T14:38:15.225Z\",\"address\":\"3-1409CamosunStreet\",\"city\":\"Victoria\",\"province\":\"BritishColumbia\",\"postalCode\":\"V8V4L5\",\"isChangeOfAddress\":false,\"isDriver\":true,\"isCyclist\":false,\"isOwner\":true,\"isPedestrian\":false,\"isPassenger\":false,\"isOther\":false,\"otherDescription\":\"\",\"issuedDate\":\"2022-05-11T14:38:15.225Z\",\"issuedOnRoadOrHighway\":\"CamosunStreet\",\"issuedAtOrNearCity\":\"Victoria\",\"isMvaOffence\":true,\"isWlaOffence\":false,\"isLcaOffence\":false,\"isMcaOffence\":false,\"isFaaOffence\":false,\"isTcrOffence\":false,\"isCtaOffence\":false,\"isOtherOffence\":false,\"otherOffenceDescription\":\"other\",\"organizationLocation\":\"Victoria\",\"violationTicketCounts\":[{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa0\",\"count\":1,\"description\":\"Parking in a handicap spot without the required sticker\",\"actRegulation\":\"MVA\",\"fullSection\":\"139\",\"section\":\"c\",\"subsection\":\"\",\"paragraph\":\"\",\"ticketedAmount\":140,\"isAct\":false,\"isRegulation\":true,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa1\",\"count\":2,\"description\":\"Distracted driving\",\"actRegulation\":\"MVA\",\"fullSection\":\"182\",\"section\":\"7\",\"subsection\":\"\",\"paragraph\":\"\",\"ticketedAmount\":110,\"isAct\":false,\"isRegulation\":false,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},{\"id\":\"3fa85f64-5717-4562-b3fc-2c963f66afa2\",\"count\":3,\"description\":\"Exceed licensed gross vehicle weight\",\"actRegulation\":\"MVA\",\"fullSection\":\"188\",\"section\":\"b\",\"subsection\":\"2\",\"paragraph\":\"\",\"ticketedAmount\":480,\"isAct\":false,\"isRegulation\":false,\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}],\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}},\"additionalProperties\":{\"additionalProp1\":\"\",\"additionalProp2\":\"\",\"additionalProp3\":\"\"}}")
      this.initialDisputeValues = response;
      this.lastUpdatedDispute = this.initialDisputeValues;
      this.form.patchValue(this.initialDisputeValues);
    });

  }

  public onBack() {
    this.backTicketList.emit();
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

}
