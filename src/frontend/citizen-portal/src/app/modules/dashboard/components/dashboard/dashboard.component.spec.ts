import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardComponent } from './dashboard.component';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DashboardComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should isMobile', () => {
    expect(typeof component.isMobile).toEqual('boolean');
  });

  it('should isDesktop', () => {
    expect(typeof component.isDesktop).toEqual('boolean');
  });

  it('should check username', () => {
    expect(component.username).toEqual('Joe Smith');
  });
});
