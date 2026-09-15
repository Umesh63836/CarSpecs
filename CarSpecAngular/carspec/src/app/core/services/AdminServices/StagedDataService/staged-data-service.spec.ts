import { TestBed } from '@angular/core/testing';

import { StagedDataService } from './staged-data-service';

describe('StagedDataService', () => {
  let service: StagedDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StagedDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
