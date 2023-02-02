import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeCourtAppearanceRoP as JJDisputeCourtAppearanceRoPBase } from 'app/api';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { UserRepresentation } from 'app/services/auth.service';

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
    "dattCd",
    "crown",
    "jjSeized",
    "adjudicator"
  ];
  jjList: UserRepresentation[];


  constructor(private jjDisputeService: JJDisputeService
  ) {
    this.jjDisputeService.jjList$.subscribe(result => {
      this.jjList = result;
    });
  }

  getJJName(jjIDIR: string) {
    let foundJJ = this.jjList.filter(x => x.idir === jjIDIR);
    if (foundJJ.length > 0) return foundJJ[0].fullName;
    else return jjIDIR;
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
