import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeCourtAppearanceAmendments, JJDisputeCourtAppearanceRoP } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { AppConfigService } from 'app/services/app-config.service';
import { featureType } from 'app/shared/directives/feature-flag.directive';

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
  amendmentsEnabled: boolean;
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
    private appConfigService: AppConfigService,
  ) {
    this.authService.jjList$.subscribe(result => {
      this.jjList = result;
    });
    this.amendmentsEnabled = this.appConfigService.isFeatureFlagEnabled(featureType.amendments);
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

  hasAmendmentData(element: JJDisputeCourtAppearanceRoP): boolean {
    const a = element?.amendments as JJDisputeCourtAppearanceAmendments;
    if (!a) return false;
    return !!(a.disputantSurnameNm || a.disputantGivenNamesNm || a.violationDateDtm ||
              a.otherNotesTxt || a.count1ActSectDescTxt || a.count1OtherTxt ||
              a.count2ActSectDescTxt || a.count2OtherTxt || a.count3ActSectDescTxt || a.count3OtherTxt);
  }
}
