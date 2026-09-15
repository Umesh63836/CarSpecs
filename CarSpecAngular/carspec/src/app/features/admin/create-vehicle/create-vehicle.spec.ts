import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateVehicle } from './create-vehicle';

describe('CreateVehicle', () => {
  let component: CreateVehicle;
  let fixture: ComponentFixture<CreateVehicle>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateVehicle],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateVehicle);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
