import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Uploadbrochure } from './uploadbrochure';

describe('Uploadbrochure', () => {
  let component: Uploadbrochure;
  let fixture: ComponentFixture<Uploadbrochure>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Uploadbrochure],
    }).compileComponents();

    fixture = TestBed.createComponent(Uploadbrochure);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
