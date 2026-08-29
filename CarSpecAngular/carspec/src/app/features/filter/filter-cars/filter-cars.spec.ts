import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterCars } from './filter-cars';

describe('FilterCars', () => {
  let component: FilterCars;
  let fixture: ComponentFixture<FilterCars>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FilterCars],
    }).compileComponents();

    fixture = TestBed.createComponent(FilterCars);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
