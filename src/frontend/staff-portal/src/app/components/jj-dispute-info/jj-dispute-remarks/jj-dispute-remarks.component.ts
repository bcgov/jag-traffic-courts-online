import { Component, OnChanges, ViewChild, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeRemark } from 'app/api';

@Component({
  selector: 'app-jj-dispute-remarks',
  templateUrl: './jj-dispute-remarks.component.html',
  styleUrls: ['./jj-dispute-remarks.component.scss'],
})
export class JJDisputeRemarksComponent implements OnChanges {
  @Input() data: JJDisputeRemark[];
  @ViewChild(MatSort) sort = new MatSort();

  dataSource = new MatTableDataSource<JJDisputeRemark>();
  displayedColumns: string[] = [
    "createdTs",
    "userFullName",
    "note",
  ]

  constructor(
  ) {
  }

  ngOnChanges(): void {
    this.data = this.data?.sort((a: JJDisputeRemark, b: JJDisputeRemark) => {
      return Date.parse(a.createdTs) - Date.parse(b.createdTs)
    });
    this.dataSource = new MatTableDataSource<JJDisputeRemark>(this.data);
  }
}
