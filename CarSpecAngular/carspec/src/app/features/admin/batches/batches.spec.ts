import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Batches } from './batches';

describe('Batches', () => {
  let component: Batches;
  let fixture: ComponentFixture<Batches>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Batches],
    }).compileComponents();

    fixture = TestBed.createComponent(Batches);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
