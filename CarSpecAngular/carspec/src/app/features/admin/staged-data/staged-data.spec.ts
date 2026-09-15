import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StagedData } from './staged-data';

describe('StagedData', () => {
  let component: StagedData;
  let fixture: ComponentFixture<StagedData>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StagedData],
    }).compileComponents();

    fixture = TestBed.createComponent(StagedData);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
