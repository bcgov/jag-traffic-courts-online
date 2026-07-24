import { Component, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Sort } from '@angular/material/sort';
import { DisputeCaseFileSummary, YesNo } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-jj-hearing-table',
  templateUrl: './jj-hearing-table.component.html',
  styleUrls: ['./jj-hearing-table.component.scss'],
  standalone: false,
})
export class JJHearingTableComponent {
  @Output() sort = new EventEmitter<Sort>();
  @Output() openDispute = new EventEmitter<DisputeCaseFileSummary>();

  @Input() dataSource!: MatTableDataSource<
    DisputeCaseFileSummary,
    MatPaginator
  >;
  @Input() displayedColumns!: string[];

  jjList: UserRepresentation[] = [];

  disputeStatus = DisputeStatus;
  yesNo = YesNo;

  constructor(private authService: AuthService) {
    this.authService.jjList$.subscribe((result) => {
      this.jjList = result;
    });
  }

  getName(jjAssignedTo: string): string {
    const jj = this.jjList.find((j) => j.idir === jjAssignedTo);
    return jj?.jjDisplayName ?? '';
  }

  isEditable(element: DisputeCaseFileSummary): boolean {
    return new Set([
      DisputeStatus.New,
      DisputeStatus.Review,
      DisputeStatus.InProgress,
      DisputeStatus.HearingScheduled,
    ]).has(element.disputeStatus?.code as DisputeStatus);
  }
}
