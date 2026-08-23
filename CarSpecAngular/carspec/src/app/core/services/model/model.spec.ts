import { TestBed } from '@angular/core/testing';

import { Model } from './model';

describe('Model', () => {
  let service: Model;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Model);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
