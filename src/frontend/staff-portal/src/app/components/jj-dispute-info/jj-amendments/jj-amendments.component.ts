import { Component, Input, Output, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { BsDatepickerDirective } from 'ngx-bootstrap/datepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { JJDispute, JJDisputedCount, JJDisputeCourtAppearanceAmendments } from 'app/api';
import { LookupsService, Statute } from 'app/services/lookups.service';

@Component({
  standalone: false,
  selector: 'app-jj-amendments',
  templateUrl: './jj-amendments.component.html',
  styleUrls: ['./jj-amendments.component.scss']
})
export class JJAmendmentsComponent implements OnInit {
  @Input() jjDisputeInfo: JJDispute;
  @Input() isViewOnly: boolean = false;
  @Input() existingAmendmentData: JJDisputeCourtAppearanceAmendments;
  @Input() showCheckbox: boolean = true; // Control whether to show the checkbox header
  @Output() amendmentDataChange: EventEmitter<JJDisputeCourtAppearanceAmendments> = new EventEmitter<JJDisputeCourtAppearanceAmendments>();

  @ViewChild('violationDatepicker') violationDatepicker: BsDatepickerDirective;

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
      disputantSurnameNm: ['', Validators.maxLength(30)],
      disputantGivenNamesNm: ['', Validators.maxLength(100)],
      violationDateDtm: [''],
      otherNotesTxt: ['', Validators.maxLength(500)]
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
          actSectDescTxt: ['', Validators.maxLength(500)],
          otherTxt: ['', Validators.maxLength(500)]
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
        this.amendmentCheckbox = true;
        this.showAmendmentSection = true;

        // Load common fields
        this.commonFieldsForm.patchValue({
          disputantSurnameNm: this.existingAmendmentData.disputantSurnameNm || '',
          disputantGivenNamesNm: this.existingAmendmentData.disputantGivenNamesNm || '',
          violationDateDtm: this.existingAmendmentData.violationDateDtm ? new Date(this.existingAmendmentData.violationDateDtm) : '',
          otherNotesTxt: this.existingAmendmentData.otherNotesTxt || ''
        });

        // Map flat count fields to individual forms
        const countFields = [
          { actSectDescTxt: this.existingAmendmentData.count1ActSectDescTxt, otherTxt: this.existingAmendmentData.count1OtherTxt },
          { actSectDescTxt: this.existingAmendmentData.count2ActSectDescTxt, otherTxt: this.existingAmendmentData.count2OtherTxt },
          { actSectDescTxt: this.existingAmendmentData.count3ActSectDescTxt, otherTxt: this.existingAmendmentData.count3OtherTxt },
        ];
        this.amendmentForms.forEach((form, index) => {
          if (index < countFields.length) {
            const fields = countFields[index];
            form.patchValue({
              isAmended: !!(fields.actSectDescTxt || fields.otherTxt),
              actSectDescTxt: fields.actSectDescTxt || '',
              otherTxt: fields.otherTxt || ''
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
        form.get('actSectDescTxt')?.value ||
        form.get('otherTxt')?.value
      );

      if (hasData) {
        // Close datepicker before opening dialog to prevent overlay conflict
        this.violationDatepicker?.hide();

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
        actSectDescTxt: '',
        otherTxt: ''
      });
    });
  }

  emitAmendmentData(): void {
    const getCountValue = (index: number, field: 'actSectDescTxt' | 'otherTxt'): string | null => {
      if (index >= this.amendmentForms.length) return null;
      const form = this.amendmentForms[index];
      if (!form.get('isAmended')?.value) return null;
      return form.get(field)?.value || null;
    };

    const data: JJDisputeCourtAppearanceAmendments = {
      disputantSurnameNm:
        this.commonFieldsForm?.get('disputantSurnameNm')?.value || null,
      disputantGivenNamesNm:
        this.commonFieldsForm?.get('disputantGivenNamesNm')?.value || null,
      violationDateDtm: (() => {
        const v = this.commonFieldsForm?.get('violationDateDtm')?.value;
        if (!v) return null;
        const pad = (n: number) => n.toString().padStart(2, '0');
        const date = new Date(v);
        return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
      })(),
      otherNotesTxt: this.commonFieldsForm?.get('otherNotesTxt')?.value || null,
      count1ActSectDescTxt: getCountValue(0, 'actSectDescTxt'),
      count1OtherTxt: getCountValue(0, 'otherTxt'),
      count2ActSectDescTxt: getCountValue(1, 'actSectDescTxt'),
      count2OtherTxt: getCountValue(1, 'otherTxt'),
      count3ActSectDescTxt: getCountValue(2, 'actSectDescTxt'),
      count3OtherTxt: getCountValue(2, 'otherTxt'),
    };

    this.amendmentDataChange.emit(data);
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
      
      this.dialog.open(ConfirmDialogComponent, { data, width: "400px" }).afterClosed()
        .subscribe((confirmed: any) => {
          if (confirmed) {
            // User confirmed - clear the amendment data for this count
            form.patchValue({
              isAmended: false,
              actSectDescTxt: '',
              otherTxt: ''
            });
          } else {
            // User cancelled - revert the checkbox visual state back to checked.
            // The form value is still true, but the checkbox DOM is visually unchecked.
            // Sync form to false first, then restore to true to force Angular to re-render.
            form.patchValue({ isAmended: false });
            setTimeout(() => form.patchValue({ isAmended: true }), 0);
          }
        });
    }
  }

  onAmendedStatuteKeyup(countIndex: number): void {
    const value = this.amendmentForms[countIndex].get('actSectDescTxt')?.value || '';
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
