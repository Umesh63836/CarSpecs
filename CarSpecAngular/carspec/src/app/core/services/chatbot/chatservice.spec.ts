import { TestBed } from '@angular/core/testing';

import { Chatservice } from './chatservice';

describe('Chatservice', () => {
  let service: Chatservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Chatservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
