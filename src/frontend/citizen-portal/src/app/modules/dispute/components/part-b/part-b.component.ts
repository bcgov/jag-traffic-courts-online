import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ViewportService } from '@core/services/viewport.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-part-b',
  templateUrl: './part-b.component.html',
  styleUrls: ['./part-b.component.scss'],
})
export class PartBComponent implements OnInit {
  public form: FormGroup;
  public busy: Subscription;

  constructor(
    protected formBuilder: FormBuilder,
    private viewportService: ViewportService
  ) {}

  public ngOnInit(): void {
    this.form = this.formBuilder.group({
      surname: [null, [Validators.required]],
    });
  }

  public onSubmit(): void {
    console.log('onSubmit');
  }

  public onBack() {
    console.log('onBack');
  }

  public get isMobile(): boolean {
    return this.viewportService.isMobile;
  }
}
