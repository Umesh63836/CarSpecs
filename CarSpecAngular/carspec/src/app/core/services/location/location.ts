import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Service, signal } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { LocationsearchResponse } from '../../models/interfaces/location-search-response';

@Service()
export class Location {
  private http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;
  openLocationSearch = signal(false);

  searchLocations(searchTerm: string, limit: number = 10): Observable<LocationsearchResponse[]> {
    const params = new HttpParams().set('location', searchTerm).set('limit', limit);
    return this.http.get<LocationsearchResponse[]>(this.apiUrl + "/Location/search", { params }
    );
  }
}
