import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { JJDispute, JJDisputedCount } from 'app/api';

export interface Amendment {
  count: number;
  isAmended: boolean;
  mvaSection?: string;
  section?: string;
  offence?: string;
  other?: string;
}

export interface AmendmentData {
  isAmended: boolean;
  lastName?: string;
  givenName?: string;
  violationDate?: string;
  other?: string;
  amendments: Amendment[];
}

@Component({
  selector: 'app-jj-amendments',
  templateUrl: './jj-amendments.component.html',
  styleUrls: ['./jj-amendments.component.scss']
})
export class JJAmendmentsComponent implements OnInit {
  @Input() jjDisputeInfo: JJDispute;
  @Input() isViewOnly: boolean = false;
  @Input() existingAmendmentData: AmendmentData;
  @Input() showCheckbox: boolean = true; // Control whether to show the checkbox header
  @Output() amendmentDataChange: EventEmitter<AmendmentData> = new EventEmitter<AmendmentData>();

  amendmentCheckbox: boolean = false;
  showAmendmentSection: boolean = false;
  amendmentForms: FormGroup[] = [];
  counts: JJDisputedCount[] = [];

  constructor(
    private formBuilder: FormBuilder,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    if (this.jjDisputeInfo && this.jjDisputeInfo.jjDisputedCounts) {
      this.counts = this.jjDisputeInfo.jjDisputedCounts;
      
      // Initialize forms for each count
      this.counts.forEach((count, index) => {
        const form = this.formBuilder.group({
          isAmended: [false],
          mvaSection: ['', Validators.maxLength(50)],
          section: ['', Validators.maxLength(50)],
          offence: ['', Validators.maxLength(500)]
        });
        
        this.amendmentForms.push(form);
        
        // Subscribe to form changes
        form.valueChanges.subscribe(() => {
          this.emitAmendmentData();
        });
      });

      // Load existing amendment data if provided
      if (this.existingAmendmentData) {
        this.amendmentCheckbox = this.existingAmendmentData.isAmended;
        this.showAmendmentSection = this.existingAmendmentData.isAmended;
        
        this.existingAmendmentData.amendments.forEach((amendment, index) => {
          if (index < this.amendmentForms.length) {
            this.amendmentForms[index].patchValue({
              isAmended: amendment.isAmended,
              mvaSection: amendment.mvaSection,
              section: amendment.section,
              offence: amendment.offence
            });
          }
        });
      }
    }
  }

  onAmendmentCheckboxChange(): void {
    if (!this.amendmentCheckbox) {
      // User is unchecking - check if there's any data entered
      const hasData = this.amendmentForms.some(form => 
        form.get('isAmended')?.value || 
        form.get('mvaSection')?.value || 
        form.get('section')?.value || 
        form.get('offence')?.value
      );

      if (hasData) {
        // Show confirmation dialog
        const data = {
          title: 'Confirm Discard Changes',
          message: 'You have entered amendment data. Are you sure you want to discard these changes?',
          actionText: 'Yes, Discard',
          cancelText: 'No, Keep'
        };

        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
          data: data,
          width: '500px'
        });

        dialogRef.afterClosed().subscribe(result => {
          if (result) {
            // User confirmed - clear all data
            this.showAmendmentSection = false;
            this.clearAllAmendmentForms();
            this.emitAmendmentData();
          } else {
            // User cancelled - recheck the box
            this.amendmentCheckbox = true;
          }
        });
      } else {
        // No data, just hide the section
        this.showAmendmentSection = false;
        this.emitAmendmentData();
      }
    } else {
      // User is checking the box - show the section
      this.showAmendmentSection = true;
      this.emitAmendmentData();
    }
  }

  clearAllAmendmentForms(): void {
    this.amendmentForms.forEach(form => {
      form.reset({
        isAmended: false,
        mvaSection: '',
        section: '',
        offence: ''
      });
    });
  }

  emitAmendmentData(): void {
    const amendments: Amendment[] = this.amendmentForms.map((form, index) => ({
      count: index + 1,
      isAmended: form.get('isAmended')?.value || false,
      mvaSection: form.get('mvaSection')?.value || '',
      section: form.get('section')?.value || '',
      offence: form.get('offence')?.value || ''
    }));

    const amendmentData: AmendmentData = {
      isAmended: this.amendmentCheckbox,
      amendments: amendments
    };

    this.amendmentDataChange.emit(amendmentData);
  }

  isCountDisputed(count: JJDisputedCount): boolean {
    return count !== null && count !== undefined;
  }

  getCountNumber(index: number): number {
    return index + 1;
  }
}
