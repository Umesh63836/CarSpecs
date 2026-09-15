import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateBrand } from './create-brand';

describe('CreateBrand', () => {
  let component: CreateBrand;
  let fixture: ComponentFixture<CreateBrand>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateBrand],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateBrand);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
