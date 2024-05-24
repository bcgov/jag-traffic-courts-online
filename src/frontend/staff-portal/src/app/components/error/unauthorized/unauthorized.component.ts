import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
  styleUrls: ['./unauthorized.component.scss']
})
export class UnauthorizedComponent implements OnInit {
  application: string;
  constructor(private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.application = this.route.snapshot.queryParamMap.get('application');
  }
}
