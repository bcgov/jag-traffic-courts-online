import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { LoggerService } from "@core/services/logger.service";
import { AppRoutes } from "app/app.routes";
import { Subscription } from "rxjs";
import { ticketTypes } from "../../shared/enums/ticket-type.enum";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { ViolationTicket } from "app/api";
import { DatePipe } from "@angular/common";

@Component({
  selector: "app-initiate-resolution",
  templateUrl: "./initiate-resolution.component.html",
  styleUrls: ["./initiate-resolution.component.scss"],
})
export class InitiateResolutionComponent implements OnInit {
  private params: any;

  public busy: Subscription;
  public ticket: ViolationTicket;
  public ticketType: string;
  public ticketTypes = ticketTypes;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    private logger: LoggerService,
    private datePipe: DatePipe,
    private violationTicketService: ViolationTicketService,
  ) {
    // always reconstruct current component
    this.router.routeReuseStrategy.shouldReuseRoute = () => { return false; };
    // check params
    this.route.queryParams.subscribe((params) => {
      this.logger.info("InitiateResolutionComponent::params", params);

      if (Object.keys(params).length === 0) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }
      this.params = params;
    });
  }

  public ngOnInit(): void {
    const ticketNumber = this.params.ticketNumber;
    const ticketTime = this.params.time;

    const ticket = this.violationTicketService.ticket;
    const storedTicketTime = this.datePipe.transform(ticket?.issued_date, "HH:mm");
    this.logger.info("InitiateResolutionComponent::ticket", ticket);

    if (ticket && ticket.ticket_number === ticketNumber && storedTicketTime === ticketTime) {
      this.logger.info("InitiateResolutionComponent:: Use existing ticket");
      this.ticket = ticket;
      this.ticketType = this.violationTicketService.ticketType;
      ;
    } else {
      this.busy = this.violationTicketService.searchTicket(this.params).subscribe(res => res);
    }
  }

  public onDisputeTicket(): void {
    this.logger.info("InitiateResolutionComponent::onDisputeTicket", this.violationTicketService.ticket);
    this.router.navigate([AppRoutes.disputePath(AppRoutes.STEPPER)]);
  }
}
