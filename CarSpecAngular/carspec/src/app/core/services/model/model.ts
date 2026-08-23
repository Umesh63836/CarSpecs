import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { IModel } from '../../models/interfaces/model';
import { IVariantModel } from '../../models/interfaces/variant-model';
import { environment } from '../../../../environments/environment';

@Service()
export class Model {
    private http = inject(HttpClient)
    private apiUrl: string = environment.apiUrl;

    getModels(brandId : number): Observable<IModel[]> {
        return this.http.get<IModel[]>(this.apiUrl + "/Models/Brand/" + brandId)
    }

    getModelByModelId(modelId : number): Observable<IVariantModel> {
        return this.http.get<IVariantModel>(this.apiUrl + "/Models/Model/" + modelId)
    }
}
