import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { DisputeRoutes } from '@dispute/dispute.routes';

@Component({
  selector: 'app-dispute-submit',
  templateUrl: './dispute-submit.component.html',
  styleUrls: ['./dispute-submit.component.scss'],
})
export class DisputeSubmitComponent implements OnInit {
  private currentParams: Params;

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      console.log('SUBMIT', params);
      this.currentParams = params;
    });
  }

  public onViewYourTicket(): void {
    this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.SUMMARY)], {
      queryParams: this.currentParams,
    });
  }
}
