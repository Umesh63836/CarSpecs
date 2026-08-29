import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowFilter } from './show-filter';

describe('ShowFilter', () => {
  let component: ShowFilter;
  let fixture: ComponentFixture<ShowFilter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShowFilter],
    }).compileComponents();

    fixture = TestBed.createComponent(ShowFilter);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
