import { TestBed } from '@angular/core/testing';

import { Brand } from './brand';

describe('Brand', () => {
  let service: Brand;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Brand);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
