import { TestBed } from '@angular/core/testing';

import { BrochureImportService } from './brochure-import-service';

describe('BrochureImportService', () => {
  let service: BrochureImportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BrochureImportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
