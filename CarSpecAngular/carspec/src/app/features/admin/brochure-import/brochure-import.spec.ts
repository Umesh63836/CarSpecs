import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BrochureImport } from './brochure-import';

describe('BrochureImport', () => {
  let component: BrochureImport;
  let fixture: ComponentFixture<BrochureImport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BrochureImport],
    }).compileComponents();

    fixture = TestBed.createComponent(BrochureImport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
