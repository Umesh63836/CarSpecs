import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { IOnRoadPriceDto } from '../../models/interfaces/OnRoadPriceDtos/on-road-price';

@Service()
export class OnRoadPrice {
  private http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getOnRoadPrice(variantId: number, stateId: number): Observable<IOnRoadPriceDto> {
    const params = new HttpParams().set('stateId', stateId);
    return this.http.get<IOnRoadPriceDto>(this.apiUrl + "/onroadprice/" + variantId, { params }
    );
  }
}
