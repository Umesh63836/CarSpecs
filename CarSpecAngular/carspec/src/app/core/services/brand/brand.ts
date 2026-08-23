import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { IBrand } from '../../models/interfaces/brand';
import { environment } from '../../../../environments/environment';

@Service()
export class Brand {
    private http = inject(HttpClient)
    private apiUrl: string = environment.apiUrl;

    getBrands(): Observable<IBrand[]> {
        return this.http.get<IBrand[]>(this.apiUrl + "/Brands")
    }
}
