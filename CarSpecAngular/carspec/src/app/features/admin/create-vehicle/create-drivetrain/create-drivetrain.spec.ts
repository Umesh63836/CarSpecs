import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateDrivetrain } from './create-drivetrain';

describe('CreateDrivetrain', () => {
  let component: CreateDrivetrain;
  let fixture: ComponentFixture<CreateDrivetrain>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateDrivetrain],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateDrivetrain);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
