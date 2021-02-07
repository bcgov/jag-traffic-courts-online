import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-page-footer',
  templateUrl: './page-footer.component.html',
  styleUrls: ['./page-footer.component.scss'],
})
export class PageFooterComponent implements OnInit {
  @Input() public hasSecondaryAction: boolean;
  @Input() public disableSave: boolean;
  @Output() public save: EventEmitter<void>;
  @Output() public continue: EventEmitter<void>;
  @Output() public back: EventEmitter<void>;

  @Input() public saveButtonLabel: string;
  @Input() public secondaryActionButtonLabel: string;

  constructor() {
    this.hasSecondaryAction = true;

    this.save = new EventEmitter<void>();
    this.continue = new EventEmitter<void>();
    this.back = new EventEmitter<void>();
    this.saveButtonLabel = 'Save and Continue';
    this.secondaryActionButtonLabel = 'Back';
  }

  public onSave(): void {
    this.save.emit();
  }

  public onSecondaryAction(): void {
    // allowed ? this.back.emit() : this.continue.emit();
    this.back.emit();
  }

  public ngOnInit(): void {
    // if (Allowed to go back) {
    // } else {
    // this.saveButtonLabel = 'Continue';
    // this.hasSecondaryAction = false;
    // }
  }
}
