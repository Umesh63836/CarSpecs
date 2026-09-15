import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BrochureReview } from './brochure-review';

describe('BrochureReview', () => {
  let component: BrochureReview;
  let fixture: ComponentFixture<BrochureReview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BrochureReview],
    }).compileComponents();

    fixture = TestBed.createComponent(BrochureReview);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
