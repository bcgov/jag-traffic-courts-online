import { Injectable } from '@angular/core';
import { Config, Configuration, ProvinceConfig } from '@config/config.model';
import { SortWeight, UtilsService } from '@core/services/utils.service';
import { AppConfigService } from 'app/services/app-config.service';
import { BehaviorSubject, Observable, of } from 'rxjs';

export interface IConfigService extends Configuration {
  load(): Observable<Configuration>;
}

@Injectable({
  providedIn: 'root',
})
export class ConfigService implements IConfigService {
  protected configuration: Configuration;

  private disputeSubmitted: BehaviorSubject<string> =
    new BehaviorSubject<string>('');
  private disputeValidationError: BehaviorSubject<string> =
    new BehaviorSubject<string>('');
  private ticketError: BehaviorSubject<string> = new BehaviorSubject<string>(
    ''
  );
  private disputeError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeCreateError: BehaviorSubject<string> =
    new BehaviorSubject<string>('');
  private statuteError: BehaviorSubject<string> = new BehaviorSubject<string>('');

  constructor(
    protected utilsService: UtilsService,
    protected appConfigService: AppConfigService,
  ) { }

  public get dispute_submitted$(): BehaviorSubject<string> {
    return this.disputeSubmitted;
  }

  public get dispute_submitted(): string {
    return this.disputeSubmitted.value;
  }

  public get dispute_validation_error$(): BehaviorSubject<string> {
    return this.disputeValidationError;
  }

  public get dispute_validation_error(): string {
    return this.disputeValidationError.value;
  }

  public get ticket_error$(): BehaviorSubject<string> {
    return this.ticketError;
  }

  public get dispute_error(): string {
    return this.disputeError.value;
  }

  public get dispute_error$(): BehaviorSubject<string> {
    return this.disputeError;
  }

  public get ticket_error(): string {
    return this.ticketError.value;
  }

  public get statute_error$(): BehaviorSubject<string> {
    return this.statuteError;
  }
  public get statute_error(): string {
    return this.statuteError.value;
  }

  public get dispute_create_error$(): BehaviorSubject<string> {
    return this.disputeCreateError;
  }

  public get dispute_create_error(): string {
    return this.disputeCreateError.value;
  }

  public get provinces(): ProvinceConfig[] {
    return [...this.configuration.provinces].sort(this.sortConfigByName());
  }

  public get courtLocations(): Config<string>[] {
    return [...this.configuration.courtLocations].sort(this.sortConfigByName());
  }

  public get policeLocations(): Config<string>[] {
    return [...this.configuration.policeLocations].sort(
      this.sortConfigByName()
    );
  }

  public get languages(): Config<string>[] {
    return [...this.configuration.languages].sort(this.sortConfigByName());
  }

  public get countries(): Config<string>[] {
    return [...this.configuration.countries].sort(this.sortConfigByName());
  }

  public get statuses(): Config<number>[] {
    return [...this.configuration.statuses].sort(this.sortConfigByName());
  }

  public get statutes(): Config<number>[] {
    return [...this.configuration.statutes].sort(this.sortConfigByName());
  }

  /**
   * @description
   * Load the runtime configuration.
   */
  public load(): Observable<Configuration> {
    if (!this.configuration) {
      return this.appConfigService.loadAppConfig()
    }

    return of({ ...this.configuration });
  }

  /**
   * @description
   * Get the configuration for bootstrapping the application.
   */
  private getConfiguration(): Observable<Configuration> {
    return null;
  }

  /**
   * @description
   * Sort the configuration by name.
   */
  private sortConfigByName(): (
    a: Config<number | string>,
    b: Config<number | string>
  ) => SortWeight {
    return (a: Config<number | string>, b: Config<number | string>) =>
      this.utilsService.sortByKey<Config<number | string>>(a, b, 'name');
  }
}
