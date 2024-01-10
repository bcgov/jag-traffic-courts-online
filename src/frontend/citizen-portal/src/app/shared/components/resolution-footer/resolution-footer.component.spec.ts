import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResolutionFooterComponent } from './resolution-footer.component';
import { NgxMaterialLegacyModule as NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.legacy.module';
import { TranslateModule } from '@ngx-translate/core';

describe('ResolutionFooterComponent', () => {
  let component: ResolutionFooterComponent;
  let fixture: ComponentFixture<ResolutionFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxMaterialModule, TranslateModule.forRoot()],
      declarations: [ResolutionFooterComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResolutionFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
