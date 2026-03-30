import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { JJDisputeCourtAppearanceAmendments } from 'app/api';

export interface AmendmentProcessingStatus {
  countNumber: number;
  isCompleted: boolean;
  completedBy?: string;
  completedDate?: Date;
}

export interface AmendmentValidationResult {
  amendmentsAcknowledged: boolean;
}

@Component({
  standalone: false,
  selector: 'app-staff-amendment-validation',
  templateUrl: './staff-amendment-validation.component.html',
  styleUrls: ['./staff-amendment-validation.component.scss']
})
export class StaffAmendmentValidationComponent implements OnInit {
  @Input() amendmentData: JJDisputeCourtAppearanceAmendments;
  @Input() isViewOnly: boolean = false;
  @Output() amendmentStatusChange: EventEmitter<AmendmentProcessingStatus[]> = new EventEmitter<AmendmentProcessingStatus[]>();
  @Output() validationStatusChange = new EventEmitter<AmendmentValidationResult>();

  amendmentProcessingStatuses: AmendmentProcessingStatus[] = [];
  hasAmendments: boolean = false;
  amendedCounts: number[] = [];
  isAcknowledged: boolean = false;

  constructor() {}

  ngOnInit(): void {
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
    if (this.amendmentData) {
      this.hasAmendments = true;

      // Build amended counts from flat fields
      this.amendedCounts = [];
      if (this.amendmentData.count1ActSectDescTxt || this.amendmentData.count1OtherTxt) this.amendedCounts.push(1);
      if (this.amendmentData.count2ActSectDescTxt || this.amendmentData.count2OtherTxt) this.amendedCounts.push(2);
      if (this.amendmentData.count3ActSectDescTxt || this.amendmentData.count3OtherTxt) this.amendedCounts.push(3);

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
    const validationResult: AmendmentValidationResult = {
      amendmentsAcknowledged: this.isAcknowledged,
    };

    this.validationStatusChange.emit(validationResult);
  }

  getAmendmentForCount(countNumber: number): { actSectDescTxt: string | null, otherTxt: string | null } | null {
    if (countNumber === 1) return { actSectDescTxt: this.amendmentData?.count1ActSectDescTxt, otherTxt: this.amendmentData?.count1OtherTxt };
    if (countNumber === 2) return { actSectDescTxt: this.amendmentData?.count2ActSectDescTxt, otherTxt: this.amendmentData?.count2OtherTxt };
    if (countNumber === 3) return { actSectDescTxt: this.amendmentData?.count3ActSectDescTxt, otherTxt: this.amendmentData?.count3OtherTxt };
    return null;
  }

}
