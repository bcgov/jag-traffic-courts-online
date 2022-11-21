import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeCourtAppearanceRoP } from 'app/api';

@Component({
  selector: 'app-jj-dispute-court-appearances',
  templateUrl: './jj-dispute-court-appearances.component.html',
  styleUrls: ['./jj-dispute-court-appearances.component.scss'],
})
export class JJDisputeCourtAppearancesComponent implements OnInit {
  @Input() public data: JJDisputeCourtAppearanceRoPView[];
  dataSource = new MatTableDataSource<JJDisputeCourtAppearanceRoPView>();
  @ViewChild(MatSort) sort = new MatSort();
  public displayedColumns: string[] = [
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
      return Date.parse(a.appearanceTs) - Date.parse(b.appearanceTs)
    });
    this.data = this.data?.filter(x => x.appearanceTs !== this.data[0]?.appearanceTs); // exclude most recent
    this.data?.forEach(appearance => {
      appearance.appearanceDate = new Date(appearance.appearanceTs);
      appearance.appearanceTime = new Date(appearance.appearanceTs);
    })
    this.dataSource = new MatTableDataSource<JJDisputeCourtAppearanceRoPView>(this.data);
  }
}

export interface JJDisputeCourtAppearanceRoPView extends JJDisputeCourtAppearanceRoP {
  appearanceDate: Date,
  appearanceTime: Date
}
