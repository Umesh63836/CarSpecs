import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Specifications } from './specifications';

describe('Specifications', () => {
  let component: Specifications;
  let fixture: ComponentFixture<Specifications>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Specifications],
    }).compileComponents();

    fixture = TestBed.createComponent(Specifications);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
