import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { JJDispute, JJDisputedCount } from 'app/api';
import { LookupsService, Statute } from 'app/services/lookups.service';

export interface Amendment {
  count: number;
  isAmended: boolean;
  amendedStatute?: string;
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
  commonFieldsForm: FormGroup;
  amendmentForms: FormGroup[] = [];
  counts: JJDisputedCount[] = [];
  filteredStatutes: Statute[][] = []; // Array of filtered statutes for each count

  constructor(
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    private lookupsService: LookupsService
  ) {}

  ngOnInit(): void {
    // Initialize common fields form
    this.commonFieldsForm = this.formBuilder.group({
      lastName: ['', Validators.maxLength(30)],
      givenName: ['', Validators.maxLength(92)],
      violationDate: [''],
      other: ['', Validators.maxLength(512)]
    });

    // Subscribe to common fields changes
    this.commonFieldsForm.valueChanges.subscribe(() => {
      this.emitAmendmentData();
    });

    if (this.jjDisputeInfo && this.jjDisputeInfo.jjDisputedCounts) {
      this.counts = this.jjDisputeInfo.jjDisputedCounts;
      
      // Initialize forms for each count
      this.counts.forEach((count, index) => {
        const form = this.formBuilder.group({
          isAmended: [false],
          amendedStatute: ['', Validators.maxLength(240)],
          other: ['', Validators.maxLength(512)]
        });
        
        this.amendmentForms.push(form);
        this.filteredStatutes.push([...this.lookupsService.statutes]); // Initialize with all statutes
        
        // Subscribe to form changes
        form.valueChanges.subscribe(() => {
          this.emitAmendmentData();
        });
      });

      // Load existing amendment data if provided
      if (this.existingAmendmentData) {
        this.amendmentCheckbox = this.existingAmendmentData.isAmended;
        this.showAmendmentSection = this.existingAmendmentData.isAmended;
        
        // Load common fields
        this.commonFieldsForm.patchValue({
          lastName: this.existingAmendmentData.lastName || '',
          givenName: this.existingAmendmentData.givenName || '',
          violationDate: this.existingAmendmentData.violationDate || '',
          other: this.existingAmendmentData.other || ''
        });
        
        this.existingAmendmentData.amendments.forEach((amendment, index) => {
          if (index < this.amendmentForms.length) {
            this.amendmentForms[index].patchValue({
              isAmended: amendment.isAmended,
              amendedStatute: amendment.amendedStatute,
              other: amendment.other
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
        form.get('amendedStatute')?.value ||
        form.get('other')?.value
      );

      if (hasData) {
        // Show confirmation dialog
        const data: DialogOptions = {
          titleKey: 'Clear Amendments?',
          messageKey: 'Are you sure you want to clear all amendment data? This action cannot be undone.',
          actionTextKey: 'Clear All',
          actionType: 'warn',
          cancelTextKey: 'Cancel',
          icon: 'warning'
        };

        this.dialog.open(ConfirmDialogComponent, {
          data: data,
          width: '40%'
        }).afterClosed().subscribe((confirmed: any) => {
          if (confirmed) {
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
        amendedStatute: '',
        other: ''
      });
    });
  }

  emitAmendmentData(): void {
    const amendments: Amendment[] = this.amendmentForms.map((form, index) => ({
      count: index + 1,
      isAmended: form.get('isAmended')?.value || false,
      amendedStatute: form.get('amendedStatute')?.value || '',
      other: form.get('other')?.value || ''
    }));

    const amendmentData: AmendmentData = {
      isAmended: this.amendmentCheckbox,
      lastName: this.commonFieldsForm?.get('lastName')?.value || '',
      givenName: this.commonFieldsForm?.get('givenName')?.value || '',
      violationDate: this.commonFieldsForm?.get('violationDate')?.value || '',
      other: this.commonFieldsForm?.get('other')?.value || '',
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

  onCountCheckboxChange(countIndex: number, isChecked: boolean): void {
    const form = this.amendmentForms[countIndex];
    
    if (isChecked) {
      // Checkbox is being checked - just update the form
      form.patchValue({ isAmended: true });
    } else {
      // Checkbox is being unchecked - confirm with user
      const data: DialogOptions = {
        titleKey: "Clear Amendment for Count " + this.getCountNumber(countIndex) + "?",
        messageKey: "Are you sure you want to clear the amendment data for this count? This action cannot be undone.",
        actionTextKey: "Clear",
        actionType: "warn",
        cancelTextKey: "Cancel",
        icon: "warning"
      };
      
      this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
        .subscribe((confirmed: any) => {
          if (confirmed) {
            // User confirmed - clear the amendment data for this count
            form.patchValue({
              isAmended: false,
              amendedStatute: '',
              other: ''
            });
          }
          // If not confirmed, the checkbox model binding will keep it checked
          // We need to manually trigger change detection
        });
    }
  }

  onAmendedStatuteKeyup(countIndex: number): void {
    const value = this.amendmentForms[countIndex].get('amendedStatute')?.value || '';
    this.filteredStatutes[countIndex] = this.filterStatutes(value);
  }

  filterStatutes(val: string): Statute[] {
    if (!this.lookupsService.statutes || this.lookupsService.statutes.length === 0) {
      return [];
    }
    if (!val || val.trim() === '') {
      return this.lookupsService.statutes;
    }
    return this.lookupsService.statutes.filter(option => 
      option.__statuteString.toLowerCase().indexOf(val.toLowerCase()) >= 0
    );
  }
}
