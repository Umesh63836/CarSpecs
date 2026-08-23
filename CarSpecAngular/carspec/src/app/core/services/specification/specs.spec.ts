import { TestBed } from '@angular/core/testing';

import { Specs } from './specs';

describe('Specs', () => {
  let service: Specs;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Specs);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
