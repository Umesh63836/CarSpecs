import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Variants } from './variants';

describe('Variants', () => {
  let component: Variants;
  let fixture: ComponentFixture<Variants>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Variants],
    }).compileComponents();

    fixture = TestBed.createComponent(Variants);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
