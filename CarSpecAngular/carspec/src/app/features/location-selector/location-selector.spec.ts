import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LocationSelector } from './location-selector';

describe('LocationSelector', () => {
  let component: LocationSelector;
  let fixture: ComponentFixture<LocationSelector>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocationSelector],
    }).compileComponents();

    fixture = TestBed.createComponent(LocationSelector);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
