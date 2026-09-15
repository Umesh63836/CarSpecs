import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateVariant } from './create-variant';

describe('CreateVariant', () => {
  let component: CreateVariant;
  let fixture: ComponentFixture<CreateVariant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateVariant],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateVariant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
