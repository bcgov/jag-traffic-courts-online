import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageRequirementsDialogComponent } from './image-requirements-dialog.component';

describe('ImageRequirementsDialogComponent', () => {
  let component: ImageRequirementsDialogComponent;
  let fixture: ComponentFixture<ImageRequirementsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImageRequirementsDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImageRequirementsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
