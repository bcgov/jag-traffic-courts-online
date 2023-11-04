import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeCourtAppearanceRoP } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';

@Component({
  selector: 'app-jj-dispute-court-appearances',
  templateUrl: './jj-dispute-court-appearances.component.html',
  styleUrls: ['./jj-dispute-court-appearances.component.scss'],
})
export class JJDisputeCourtAppearancesComponent implements OnInit {
  @Input() data: JJDisputeCourtAppearanceRoP[];
  @ViewChild(MatSort) sort = new MatSort();

  dataSource = new MatTableDataSource<JJDisputeCourtAppearanceRoP>();
  tempData: JJDisputeCourtAppearanceRoP[] = [];
  displayedColumns: string[] = [
    "appearanceTs",
    "room",
    "reason",
    "appCd",
    "noAppTs",
    "clerkRecord",
    "defenceCounsel",
    "dattCd",
    "crown",
    "jjSeized",
    "adjudicator"
  ];
  jjList: UserRepresentation[];

  constructor(
    private authService: AuthService,
  ) {
    this.authService.jjList$.subscribe(result => {
      this.jjList = result;
    });
  }

  getJJName(jjIDIR: string) {
    let foundJJ = this.jjList.filter(x => x.idir === jjIDIR);
    if (foundJJ.length > 0) return foundJJ[0].fullName;
    else return jjIDIR;
  }

  ngOnInit(): void {
    this.data.forEach(courtAppearance => { this.tempData.push(courtAppearance) }); // make a copy
    this.tempData = this.tempData?.sort((a: JJDisputeCourtAppearanceRoP, b: JJDisputeCourtAppearanceRoP) => {
      return Date.parse(b.appearanceTs) - Date.parse(a.appearanceTs)
    });
    this.tempData.shift(); // exclude most recent
    this.dataSource = new MatTableDataSource<JJDisputeCourtAppearanceRoP>(this.tempData);
  }
}
