import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-page-footer',
  templateUrl: './page-footer.component.html',
  styleUrls: ['./page-footer.component.scss'],
})
export class PageFooterComponent implements OnInit {
  @Input() public hasSecondaryAction: boolean;
  @Input() public disableSave: boolean;
  @Output() public save: EventEmitter<void>;
  @Output() public back: EventEmitter<void>;

  @Input() public saveButtonKey: string;
  @Input() public secondaryActionButtonKey: string;

  // @Input() public saveButtonLabel: string;
  // @Input() public secondaryActionButtonLabel: string;
  @Input() public secondaryActionButtonIcon: string;

  constructor(private translateService: TranslateService) {
    this.hasSecondaryAction = true;

    this.save = new EventEmitter<void>();
    this.back = new EventEmitter<void>();
    // this.saveButtonLabel = 'Next';
    // this.secondaryActionButtonLabel = 'Back';
    this.secondaryActionButtonIcon = 'keyboard_arrow_left';
  }

  public ngOnInit(): void {
    //
  }

  public onSave(): void {
    this.save.emit();
  }

  public onSecondaryAction(): void {
    this.back.emit();
  }
}
