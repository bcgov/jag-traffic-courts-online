import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoadingStore } from '@core/store';
import { provideMockStore, MockStore } from '@ngrx/store/testing';
import { PageComponent } from './page.component';

describe('PageComponent', () => {
  let component: PageComponent;
  let fixture: ComponentFixture<PageComponent>;
  let store: MockStore;
  let initialState = { ...LoadingStore.InitialState };

  beforeEach(async () => {
    TestBed.configureTestingModule({
      declarations: [PageComponent],
      providers: [provideMockStore({ initialState })],
    })

    store = TestBed.inject(MockStore);
    await TestBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
