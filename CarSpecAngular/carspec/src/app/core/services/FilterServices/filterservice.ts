import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { CarFilterRequest } from '../../models/models/carFilterRequest';
import { CarFilterResponse } from '../../models/interfaces/filterResponse/car-filter-response';

@Service()
export class Filterservice {
    private http = inject(HttpClient);

    private apiUrl = environment.apiUrl;;

    FilterCars(request: CarFilterRequest) {
      let params = new HttpParams();
      Object.entries(request).forEach(([key, value]) => {
        if (
          value !== undefined &&
          value !== null &&
          value !== ''
        ) {
          params = params.set(key, value.toString());
        }
      });

      return this.http.get<CarFilterResponse>(this.apiUrl + "/filter",  { params }
      );
    }
}
