import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ImageRequirementsDialogComponent } from '../image-requirements-dialog/image-requirements-dialog.component';

@Component({
  selector: 'app-ticket-not-found-dialog',
  templateUrl: './ticket-not-found-dialog.component.html',
  styleUrls: ['./ticket-not-found-dialog.component.scss']
})
export class TicketNotFoundDialogComponent implements OnInit {

  constructor(
    private dialog: MatDialog,
  ) { 
  }

  ngOnInit(): void {
  }
  public onViewImageRequirements(): void {
    this.dialog.open(ImageRequirementsDialogComponent, {
      width: '600px',
    });
  }

}
