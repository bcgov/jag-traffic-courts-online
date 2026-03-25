import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeCourtAppearanceRoP } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';

@Component({
  selector: 'app-jj-dispute-court-appearances',
  templateUrl: './jj-dispute-court-appearances.component.html',
  styleUrls: ['./jj-dispute-court-appearances.component.scss'],
  standalone: false,
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
        { number: 2, amendedStatute: 'Motor Vehicle Act 144(1)(a) - Drive without due care and attention', other: '' }
      ]
    },
    1: {
      lastName: '',
      givenName: '',
      violationDate: '15-Feb-2024',
      other: 'Date corrected as per disputant testimony',
      counts: [
        { number: 1, amendedStatute: 'MVR 6.07 - Emergency brake inadequate', other: '' }
      ]
    },
    2: {
      lastName: 'Doe',
      givenName: 'Jane',
      violationDate: '10-Jan-2024',
      other: '',
      counts: [
        { number: 1, amendedStatute: 'Motor Vehicle Act 146(1) - Fail to obey traffic control device', other: 'Amended per evidence review' },
        { number: 2, amendedStatute: 'Motor Vehicle Act 144(1)(b) - Drive without reasonable consideration', other: '' }
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
