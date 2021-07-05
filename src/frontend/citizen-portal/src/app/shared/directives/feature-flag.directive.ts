import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AppConfigService } from 'app/services/app-config.service';

export const featureType = {
  dispute: 'dispute',
} as const;

@Directive({
  selector: '[featureFlag]',
})
export class FeatureFlagDirective {
  @Input() set featureFlag(featureName: string) {
    const flagEnabled = this.appConfigService.isFeatureFlagEnabled(featureName);

    if (flagEnabled) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private appConfigService: AppConfigService
  ) {}
}
