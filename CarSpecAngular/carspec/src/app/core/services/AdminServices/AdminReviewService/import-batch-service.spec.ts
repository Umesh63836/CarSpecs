import { TestBed } from '@angular/core/testing';

import { ImportBatchService } from './import-batch-service';

describe('ImportBatchService', () => {
  let service: ImportBatchService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ImportBatchService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
