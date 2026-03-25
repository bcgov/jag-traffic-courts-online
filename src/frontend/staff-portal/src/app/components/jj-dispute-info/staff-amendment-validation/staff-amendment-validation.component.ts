import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { AmendmentData } from '../jj-amendments/jj-amendments.component';

export interface AmendmentProcessingStatus {
  countNumber: number;
  isCompleted: boolean;
  completedBy?: string;
  completedDate?: Date;
}

export interface AmendmentValidationResult {
  allAmendmentsProcessed: boolean;
  pendingAmendments: number[];
}

@Component({
  standalone: false,
  selector: 'app-staff-amendment-validation',
  templateUrl: './staff-amendment-validation.component.html',
  styleUrls: ['./staff-amendment-validation.component.scss']
})
export class StaffAmendmentValidationComponent implements OnInit {
  @Input() amendmentData: AmendmentData;
  @Input() isViewOnly: boolean = false;
  @Output() amendmentStatusChange: EventEmitter<AmendmentProcessingStatus[]> = new EventEmitter<AmendmentProcessingStatus[]>();
  @Output() validationStatusChange: EventEmitter<AmendmentValidationResult> = new EventEmitter<AmendmentValidationResult>();

  amendmentProcessingStatuses: AmendmentProcessingStatus[] = [];
  hasAmendments: boolean = false;
  amendedCounts: number[] = [];
  isAcknowledged: boolean = false;

  constructor() {}

  ngOnInit(): void {
    // TODO: Remove mock data
    if (!this.amendmentData?.isAmended) {
      this.amendmentData = {
        isAmended: true,
        lastName: 'Smith',
        givenName: 'John',
        violationDate: '2024-01-15',
        other: 'Speed limit sign was not visible',
        amendments: [
          { count: 1, isAmended: true, amendedStatute: '148(1) MVA', other: null },
          { count: 2, isAmended: true, amendedStatute: null, other: 'Count 2 corrected description' }
        ]
      };
    }
    this.initializeAmendmentStatuses();
  }

  hasValue(value: string | null | undefined): boolean {
    return !!value && value.trim().length > 0;
  }

  onAcknowledgedChange(isChecked: boolean): void {
    this.isAcknowledged = isChecked;
    
    // Update all amendment statuses based on acknowledged state
    this.amendmentProcessingStatuses.forEach(status => {
      status.isCompleted = isChecked;
      
      if (isChecked) {
        status.completedDate = new Date();
        status.completedBy = 'Current Staff User';
      } else {
        status.completedDate = undefined;
        status.completedBy = undefined;
      }
    });

    this.emitStatusChange();
    this.emitValidationStatus();
  }

  initializeAmendmentStatuses(): void {
    if (this.amendmentData && this.amendmentData.isAmended) {
      this.hasAmendments = true;
      
      // Get all counts that have amendments
      this.amendedCounts = this.amendmentData.amendments
        .filter(amendment => amendment.isAmended)
        .map(amendment => amendment.count);

      // Initialize processing statuses
      this.amendmentProcessingStatuses = this.amendedCounts.map(countNumber => ({
        countNumber: countNumber,
        isCompleted: false
      }));

      this.emitValidationStatus();
    }
  }

  emitStatusChange(): void {
    this.amendmentStatusChange.emit(this.amendmentProcessingStatuses);
  }

  emitValidationStatus(): void {
    const pendingAmendments = this.amendmentProcessingStatuses
      .filter(status => !status.isCompleted)
      .map(status => status.countNumber);

    const validationResult: AmendmentValidationResult = {
      allAmendmentsProcessed: pendingAmendments.length === 0 && this.amendmentProcessingStatuses.length > 0,
      pendingAmendments: pendingAmendments
    };

    this.validationStatusChange.emit(validationResult);
  }

  getAmendmentForCount(countNumber: number) {
    return this.amendmentData?.amendments?.find(a => a.count === countNumber);
  }

}
