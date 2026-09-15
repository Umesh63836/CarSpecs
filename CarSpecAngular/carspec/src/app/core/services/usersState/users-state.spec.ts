import { TestBed } from '@angular/core/testing';

import { UsersState } from './users-state';

describe('UsersState', () => {
  let service: UsersState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UsersState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
