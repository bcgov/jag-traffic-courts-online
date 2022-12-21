import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { LoggerService } from "@core/services/logger.service";
import { AppRoutes } from "app/app.routes";
import { TicketTypes } from "../../shared/enums/ticket-type.enum";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { ViolationTicket } from "app/api";

@Component({
  selector: "app-initiate-resolution",
  templateUrl: "./initiate-resolution.component.html",
  styleUrls: ["./initiate-resolution.component.scss"],
})
export class InitiateResolutionComponent implements OnInit {
  private params: any;
  ticket: ViolationTicket;
  ticketType: string;
  ticketTypes = TicketTypes ;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private logger: LoggerService,
    private violationTicketService: ViolationTicketService,
  ) {
    // always reconstruct current component
    this.router.routeReuseStrategy.shouldReuseRoute = () => { return false; };
    this.params = this.route.snapshot.queryParams;
  }

  ngOnInit(): void {
    if (this.violationTicketService.validateTicket(this.params)) {
      this.logger.info("InitiateResolutionComponent:: Use existing ticket");
      this.ticket = this.violationTicketService.ticket;
      this.ticketType = this.violationTicketService.ticketType;
    } else {
      this.violationTicketService.searchTicket().subscribe(res => res);
    }
  }

  onDisputeTicket(): void {
    this.logger.info("InitiateResolutionComponent::onDisputeTicket", this.violationTicketService.ticket);
    this.router.navigate([AppRoutes.ticketPath(AppRoutes.STEPPER)]);
  }
}
