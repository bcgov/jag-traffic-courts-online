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

  // Mock amendment data for demonstration - maps row index to amendment details
  mockAmendments: { [key: number]: any } = {
    0: {
      lastName: 'Smith',
      givenName: 'Robert',
      violationDate: '',
      other: '',
      counts: [
        { number: 2, section: '144(1)(a)', offence: '', mvaSection: '', other: '' }
      ]
    },
    1: {
      lastName: '',
      givenName: '',
      violationDate: '15-Feb-2024',
      other: 'Date corrected as per disputant testimony',
      counts: [
        { number: 1, section: '', offence: 'MVR 6.07 Emergency brake inadequate', mvaSection: '', other: '' }
      ]
    },
    2: {
      lastName: 'Doe',
      givenName: 'Jane',
      violationDate: '10-Jan-2024',
      other: '',
      counts: [
        { number: 1, section: '146(1)', offence: 'Fail to obey traffic control device', mvaSection: 'Motor Vehicle Act', other: 'Amended per evidence review' },
        { number: 2, section: '144(1)(b)', offence: 'Drive without reasonable consideration', mvaSection: 'Motor Vehicle Act', other: '' }
      ]
    }
  };

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

  hasAmendments(index: number): boolean {
    return this.mockAmendments[index] !== undefined;
  }

  getAmendments(index: number): any {
    return this.mockAmendments[index];
  }
}
