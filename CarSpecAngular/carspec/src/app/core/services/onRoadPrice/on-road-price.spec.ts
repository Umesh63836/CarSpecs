import { TestBed } from '@angular/core/testing';
import { OnRoadPrice } from './on-road-price';


describe('OnRoadPrice', () => {
  let service: OnRoadPrice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OnRoadPrice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
