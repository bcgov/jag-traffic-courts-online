import {
  ComponentFixture,
  TestBed,
} from '@angular/core/testing';
import { CoreModule } from '@core/core.module';
import { LoggerService } from '@core/services/logger.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { NgxProgressModule } from '@shared/modules/ngx-progress/ngx-progress.module';
import { HeaderComponent } from './header.component';

describe('HeaderComponent', () => {
  let component: HeaderComponent;
  let fixture: ComponentFixture<HeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxMaterialModule, NgxProgressModule, CoreModule],
      declarations: [HeaderComponent],
      providers: [
        HeaderComponent,
        LoggerService,
      ],
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(HeaderComponent);
      });

    component = TestBed.inject(HeaderComponent);
  });
});
