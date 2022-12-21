import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeCourtAppearanceRoP as JJDisputeCourtAppearanceRoPBase } from 'app/api';

@Component({
  selector: 'app-jj-dispute-court-appearances',
  templateUrl: './jj-dispute-court-appearances.component.html',
  styleUrls: ['./jj-dispute-court-appearances.component.scss'],
})
export class JJDisputeCourtAppearancesComponent implements OnInit {
  @Input() data: JJDisputeCourtAppearanceRoP[];
  @ViewChild(MatSort) sort = new MatSort();

  dataSource = new MatTableDataSource<JJDisputeCourtAppearanceRoP>();
  displayedColumns: string[] = [
    "appearanceDate",
    "appearanceTime",
    "room",
    "reason",
    "app",
    "noAppTs",
    "clerkRecord",
    "defenseCounsel",
    "crown",
    "jjSeized",
    "adjudicator"
  ]

  constructor(
  ) {
  }

  ngOnInit(): void {
    this.data = this.data?.sort((a: JJDisputeCourtAppearanceRoP, b: JJDisputeCourtAppearanceRoP) => {
      return Date.parse(b.appearanceTs) - Date.parse(a.appearanceTs)
    });
    this.data.shift(); // exclude most recent
    this.data?.forEach(appearance => {
      appearance.appearanceDate = new Date(appearance.appearanceTs);
      appearance.appearanceTime = new Date(appearance.appearanceTs);
    })
    this.dataSource = new MatTableDataSource<JJDisputeCourtAppearanceRoP>(this.data);
  }
}

export interface JJDisputeCourtAppearanceRoP extends JJDisputeCourtAppearanceRoPBase {
  appearanceDate: Date,
  appearanceTime: Date
}
