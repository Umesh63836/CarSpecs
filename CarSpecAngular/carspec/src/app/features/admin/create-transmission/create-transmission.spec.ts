import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTransmission } from './create-transmission';

describe('CreateTransmission', () => {
  let component: CreateTransmission;
  let fixture: ComponentFixture<CreateTransmission>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateTransmission],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTransmission);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
