import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { IVariant } from '../../models/interfaces/variant';
import { environment } from '../../../../environments/environment';

@Service()
export class Variant {
    private http = inject(HttpClient)
    private apiUrl: string = environment.apiUrl;

    getVariants(modelId : number): Observable<IVariant[]> {
        return this.http.get<IVariant[]>(this.apiUrl + "/Variants/model/" + modelId)
    }
}
