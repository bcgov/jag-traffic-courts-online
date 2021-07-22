import { Component, OnInit } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-ticket-image',
  templateUrl: './ticket-image.component.html',
  styleUrls: ['./ticket-image.component.scss'],
})
export class TicketImageComponent implements OnInit {
  public busy: Subscription;
  public imageSrc: string;
  public fileName = '';

  constructor(private logger: LoggerService) {}

  ngOnInit(): void {}

  public onFileChange(event: any) {
    if (!event.target.files[0] || event.target.files[0].length === 0) {
      this.logger.log('You must select an image');
      return;
    }

    const mimeType = event.target.files[0].type;

    if (mimeType.match(/image\/*/) == null) {
      this.logger.log('Only images are supported');
      return;
    }

    const reader = new FileReader();
    const file: File = event.target.files[0];
    this.fileName = file.name;
    reader.readAsDataURL(file);
    this.logger.log('file', file.name, file.lastModified);

    reader.onload = () => {
      this.imageSrc = reader.result as string;

      // this.myForm.patchValue({
      //   fileSource: reader.result,
      // });
    };
  }
}
