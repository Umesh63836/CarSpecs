import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { ISpecs } from '../../models/interfaces/specs';
import { environment } from '../../../../environments/environment';

@Service()
export class Specs {
    private http = inject(HttpClient)
    private apiUrl: string = environment.apiUrl;

    getSpecs(variantId : number): Observable<ISpecs> {
        return this.http.get<ISpecs>(this.apiUrl + "/Specifications/Variant/" + variantId)
    }
}
