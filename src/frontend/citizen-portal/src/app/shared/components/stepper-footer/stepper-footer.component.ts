import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-stepper-footer',
  templateUrl: './stepper-footer.component.html',
  styleUrls: ['./stepper-footer.component.scss'],
})
export class StepperFooterComponent implements OnInit {
  @Input() public hasSecondaryAction: boolean;
  @Input() public disableSave: boolean;

  @Output() public save: EventEmitter<void>;
  @Output() public back: EventEmitter<void>;

  @Input() public saveButtonKey: string;
  @Input() public secondaryActionButtonKey: string;
  @Input() public secondaryActionButtonIcon: string;

  constructor() {
    this.hasSecondaryAction = true;

    this.save = new EventEmitter<void>();
    this.back = new EventEmitter<void>();
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
