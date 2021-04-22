import { Component, OnInit } from '@angular/core';
import { ConfigService } from '@config/config.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(private configService: ConfigService) {}

  public ngOnInit(): void {
    console.log('languages', this.configService.languages);
  }
}
