import { TestBed } from '@angular/core/testing';

import { Variant } from './variant';

describe('Variant', () => {
  let service: Variant;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Variant);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
