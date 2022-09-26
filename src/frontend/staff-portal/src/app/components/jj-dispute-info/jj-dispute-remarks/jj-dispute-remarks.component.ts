import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeRemark } from 'app/api';

@Component({
  selector: 'app-jj-dispute-remarks',
  templateUrl: './jj-dispute-remarks.component.html',
  styleUrls: ['./jj-dispute-remarks.component.scss'],
})
export class JJDisputeRemarksComponent implements OnInit {
  @Input() public data: JJDisputeRemark[];
  dataSource = new MatTableDataSource<JJDisputeRemark>();
  @ViewChild(MatSort) sort = new MatSort();
  public displayedColumns: string[] = [
    "createdTs",
    "userFullName",
    "note",
  ]

  constructor(
  ) {
  }

  ngOnInit(): void {
    this.data = this.data?.sort((a: JJDisputeRemark, b: JJDisputeRemark) => {
      return Date.parse(a.createdTs) - Date.parse(b.createdTs)
    });
    this.dataSource = new MatTableDataSource<JJDisputeRemark>(this.data);
  }
}
