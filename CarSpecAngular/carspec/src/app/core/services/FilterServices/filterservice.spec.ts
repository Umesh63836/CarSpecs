import { TestBed } from '@angular/core/testing';

import { Filterservice } from './filterservice';

describe('Filterservice', () => {
  let service: Filterservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Filterservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
