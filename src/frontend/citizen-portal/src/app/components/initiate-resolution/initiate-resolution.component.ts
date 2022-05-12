import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { LoggerService } from "@core/services/logger.service";
import { AppRoutes } from "app/app.routes";
import { Subscription } from "rxjs";
import { ticketTypes } from "../../shared/enums/ticket-type.enum";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { ViolationTicket } from "app/api";

@Component({
  selector: "app-initiate-resolution",
  templateUrl: "./initiate-resolution.component.html",
  styleUrls: ["./initiate-resolution.component.scss"],
})
export class InitiateResolutionComponent implements OnInit {
  public busy: Subscription;
  public ticket: ViolationTicket;
  public ticketType: string;
  public ticketTypes = ticketTypes;

  constructor(
    protected router: Router,
    private logger: LoggerService,
    private violationTicketService: ViolationTicketService,
  ) {
    // always reconstruct current component
    this.router.routeReuseStrategy.shouldReuseRoute = () => { return false; };
  }

  public ngOnInit(): void {
    if (this.violationTicketService.validateTicket()) {
      this.logger.info("InitiateResolutionComponent:: Use existing ticket");
      this.ticket = this.violationTicketService.ticket;
      this.ticketType = this.violationTicketService.ticketType;
      ;
    } else {
      this.busy = this.violationTicketService.searchTicket().subscribe(res => res);
    }
  }

  public onDisputeTicket(): void {
    this.logger.info("InitiateResolutionComponent::onDisputeTicket", this.violationTicketService.ticket);
    this.router.navigate([AppRoutes.disputePath(AppRoutes.STEPPER)]);
  }
}
