import { async, ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AlertComponent } from './alert.component';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';

describe('AlertComponent', () => {
  let component: AlertComponent;
  let fixture: ComponentFixture<AlertComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule(
      {
        imports: [
          NgxMaterialModule
        ],
        declarations: [
          AlertComponent
        ]
      }
    ).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AlertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
