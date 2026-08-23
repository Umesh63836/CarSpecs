import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateFuelType } from './create-fuel-type';

describe('CreateFuelType', () => {
  let component: CreateFuelType;
  let fixture: ComponentFixture<CreateFuelType>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateFuelType],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateFuelType);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
