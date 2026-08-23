import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateModel } from './create-model';

describe('CreateModel', () => {
  let component: CreateModel;
  let fixture: ComponentFixture<CreateModel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateModel],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateModel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
