import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEngine } from './create-engine';

describe('CreateEngine', () => {
  let component: CreateEngine;
  let fixture: ComponentFixture<CreateEngine>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateEngine],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateEngine);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
