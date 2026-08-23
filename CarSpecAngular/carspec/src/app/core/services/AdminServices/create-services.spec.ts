import { TestBed } from '@angular/core/testing';

import { CreateServices } from './create-services';

describe('CreateServices', () => {
  let service: CreateServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CreateServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
