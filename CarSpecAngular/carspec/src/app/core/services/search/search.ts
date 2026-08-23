import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { ISearch } from '../../models/interfaces/search';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Service()
export class Search {

  private http = inject(HttpClient);

  private apiUrl = environment.apiUrl + '/Search/search/';

  search(query: string): Observable<ISearch[]> {
    return this.http.get<ISearch[]>(
      this.apiUrl + query
    );
  }
}
