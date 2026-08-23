import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { BrandDto, CreateBrandDto } from '../../models/models/brandDto';
import { Observable } from 'rxjs';
import { CreateModelDto, ModelDto } from '../../models/models/modelDto';
import { CreateEngineDto, EngineDto } from '../../models/models/engineDto';
import { CreateTransmissionDto, TransmissionDto } from '../../models/models/transmissionDto';
import { CreateDrivetrainDto, DrivetrainDto } from '../../models/models/drivetrainDto';
import { CreateVariantDto, VariantDto } from '../../models/models/variantDto';
import { SelectEngineDto } from '../../models/interfaces/selectDtos/select-engine-dto';
import { SelectTransmissionDto } from '../../models/interfaces/selectDtos/select-transmission-dto';
import { SelectDrivetrainDto } from '../../models/interfaces/selectDtos/select-drivetrain-dto';
import { SelectFueltypeDto } from '../../models/interfaces/selectDtos/select-fueltype-dto';
import { environment } from '../../../../environments/environment';

@Service()
export class CreateServices {
    private http = inject(HttpClient);

    private apiUrl = environment.apiUrl;

    createBrand(dto: CreateBrandDto): Observable<BrandDto> {
    return this.http.post<BrandDto>(this.apiUrl + '/Brands', dto);
    }

    createModel(dto: CreateModelDto): Observable<ModelDto> {
    return this.http.post<ModelDto>(this.apiUrl + '/Models', dto);
    }

    createEngine(dto: CreateEngineDto): Observable<EngineDto> {
    return this.http.post<EngineDto>(this.apiUrl + '/Specifications/engine', dto);
    }

    createTransmission(dto: CreateTransmissionDto): Observable<TransmissionDto> {
    return this.http.post<TransmissionDto>(this.apiUrl + '/Specifications/transmission',dto);
    }

    createDrivetrain(dto: CreateDrivetrainDto): Observable<DrivetrainDto> {
    return this.http.post<DrivetrainDto>(this.apiUrl + '/Specifications/drivetrain',dto);
    }

    createVariant(modelId: number,dto: CreateVariantDto): Observable<VariantDto> {
    return this.http.post<VariantDto>(this.apiUrl + '/Variants/' + modelId + '/variants', dto);
    }

    getAllEngines() : Observable<SelectEngineDto[]> {
    return this.http.get<SelectEngineDto[]>(this.apiUrl + '/Specifications/allengines');
    }

    getAllTransmission() : Observable<SelectTransmissionDto[]> {
    return this.http.get<SelectTransmissionDto[]>(this.apiUrl + '/Specifications/alltransmissions');
    }

    getAllDrivetrain() : Observable<SelectDrivetrainDto[]> {
    return this.http.get<SelectDrivetrainDto[]>(this.apiUrl + '/Specifications/alldrivetrains');
    }

    getAllFueltype() : Observable<SelectFueltypeDto[]> {
    return this.http.get<SelectFueltypeDto[]>(this.apiUrl + '/Specifications/allfueltypes');
    }

}
