import {
  HttpClient,
  HttpParams
} from '@angular/common/http';

import {
  Injectable,
  inject
} from '@angular/core';

import {
  Observable
} from 'rxjs';

import {
  StagedModelListDto,
  StagedModelDetailDto,

  UpdateStagedModelDto,
  UpdateStagedVariantDto,
  UpdateStagedEngineDto,
  UpdateStagedEnginePerformanceDto,
  UpdateStagedMotorPerformanceDto,
  UpdateStagedFeatureDto,
  UpdateStagedSpecificationDto,
  UpdateStagedFuelEfficiencyDto,
  UpdateStagedWarrantyDto,

  CreateStagedFuelEfficiencyDto,
  CreateStagedWarrantyDto,
  StagedWarrantyDto,
  PaginatedStagedModelResponseDto,
  CreateStagedFeatureDto,
  CreateStagedSpecificationDto,
  UpdateStagedPowertrainDto,
  UpdateStagedTransmissionDto,
  UpdateStagedDrivetrainDto,
  CreateStagedDrivetrainDto,
  CreateStagedTransmissionDto,
  CreateStagedPowertrainDto,
  CreateStagedEngineDto,
  CreateStagedEnginePerformanceDto,
  CreateStagedMotorPerformanceDto,
  StagedPowertrainDependenciesDto,
  StagedFeatureSpecificationCatalogDto,
  StagedPowertrainLookupDto,
  StagedTransmissionLookupDto,
  StagedDrivetrainLookupDto,
  ReplaceStagedPowertrainReferenceDto,
  ReplaceStagedPowertrainReferenceResultDto,
  ReplaceStagedTransmissionReferenceDto,
  ReplaceStagedTransmissionReferenceResultDto,
  ReplaceStagedDrivetrainReferenceDto,
  ReplaceStagedDrivetrainReferenceResultDto
} from '../../../models/interfaces/DataProcessingDtos/StagedDataDtos/staged-data-dto';


/*
 * These DTOs are not currently present in the DTO file supplied with
 * the component, so they are kept local to this service until you add
 * them to staged-data-dto.ts.
 */



@Injectable({
  providedIn: 'root'
})
export class StagedDataService {

  private readonly http =
    inject(HttpClient);


  /*
   * Keep this as the single API root.
   *
   * If your application already has an environment/config service,
   * replace only this value with your existing API URL.
   */
  private readonly apiUrl =
    'https://localhost:7135/api';

  private readonly baseUrl =
    `${this.apiUrl}/admin/StagedData`;


  // ============================================================
  // MODELS
  // ============================================================

  getModels(
    pageNumber: number = 1,
    pageSize: number = 10,
    search: string = ''
  ): Observable<PaginatedStagedModelResponseDto> {

    let params =
      new HttpParams()
        .set('pageNumber', pageNumber)
        .set('pageSize', pageSize);

    const trimmedSearch =
      search.trim();

    if (trimmedSearch) {
      params =
        params.set(
          'search',
          trimmedSearch
        );
    }

    return this.http.get<
      PaginatedStagedModelResponseDto
    >(
      `${this.baseUrl}/models`,
      { params }
    );
  }


  getModel(
    importModelId: number
  ): Observable<StagedModelDetailDto> {

    return this.http.get<
      StagedModelDetailDto
    >(
      `${this.baseUrl}/models/${importModelId}`
    );
  }

  getPowertrainDependencies(): Observable<StagedPowertrainDependenciesDto> {
  return this.http.get<StagedPowertrainDependenciesDto>(
    `${this.baseUrl}/powertrain-dependencies`
  );
}


  updateModel(
    importModelId: number,
    dto: UpdateStagedModelDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/models/${importModelId}`,
      dto
    );
  }


  // ============================================================
  // VARIANT
  // ============================================================

  updateVariant(
    importVariantId: number,
    dto: UpdateStagedVariantDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/variants/${importVariantId}`,
      dto
    );
  }


  // ============================================================
  // ENGINE
  // ============================================================

  updateEngine(
    importEngineId: number,
    dto: UpdateStagedEngineDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/engines/${importEngineId}`,
      dto
    );
  }


  // ============================================================
  // ENGINE PERFORMANCE
  // ============================================================

  updateEnginePerformance(
    importEnginePerformanceId: number,
    dto: UpdateStagedEnginePerformanceDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/engine-performances/${importEnginePerformanceId}`,
      dto
    );
  }


  // ============================================================
  // MOTOR PERFORMANCE
  // ============================================================

  updateMotorPerformance(
    importMotorPerformanceId: number,
    dto: UpdateStagedMotorPerformanceDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/motor-performances/${importMotorPerformanceId}`,
      dto
    );
  }


  // ============================================================
  // FEATURE
  // ============================================================

  updateFeature(
    importFeatureVariantId: number,
    dto: UpdateStagedFeatureDto
  ): Observable<void> {

    /*
     * Controller route:
     * PUT feature-variants/{importFeatureVariantId}
     */
    return this.http.put<void>(
      `${this.baseUrl}/feature-variants/${importFeatureVariantId}`,
      dto
    );
  }


  /*
   * Alias retained for component code that uses the more explicit
   * method name.
   */
  updateFeatureVariant(
    importFeatureVariantId: number,
    dto: UpdateStagedFeatureDto
  ): Observable<void> {

    return this.updateFeature(
      importFeatureVariantId,
      dto
    );
  }


  createFeature(
    dto: CreateStagedFeatureDto
  ): Observable<{ importFeatureId: number }> {

    return this.http.post<{
      importFeatureId: number
    }>(
      `${this.baseUrl}/features`,
      dto
    );
  }


  // ============================================================
  // SPECIFICATION
  // ============================================================

  updateSpecification(
    importSpecificationVariantId: number,
    dto: UpdateStagedSpecificationDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/specification-variants/${importSpecificationVariantId}`,
      dto
    );
  }


  updateSpecificationVariant(
    importSpecificationVariantId: number,
    dto: UpdateStagedSpecificationDto
  ): Observable<void> {

    return this.updateSpecification(
      importSpecificationVariantId,
      dto
    );
  }


  createSpecification(
    dto: CreateStagedSpecificationDto
  ): Observable<{ importSpecificationId: number }> {

    return this.http.post<{
      importSpecificationId: number
    }>(
      `${this.baseUrl}/specifications`,
      dto
    );
  }


  // ============================================================
  // FUEL EFFICIENCY
  // ============================================================

  updateFuelEfficiency(
    importFuelEfficiencyId: number,
    dto: UpdateStagedFuelEfficiencyDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/fuel-efficiencies/${importFuelEfficiencyId}`,
      dto
    );
  }


  createFuelEfficiency(
    dto: CreateStagedFuelEfficiencyDto
  ): Observable<{ importFuelEfficiencyId: number }> {

    return this.http.post<{
      importFuelEfficiencyId: number
    }>(
      `${this.baseUrl}/fuel-efficiencies`,
      dto
    );
  }


  // ============================================================
  // WARRANTY
  // ============================================================

  updateWarranty(
    importWarrantyId: number,
    dto: UpdateStagedWarrantyDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/warranties/${importWarrantyId}`,
      dto
    );
  }


  createWarranty(
    dto: CreateStagedWarrantyDto
  ): Observable<{ importWarrantyId: number }> {

    return this.http.post<{
      importWarrantyId: number
    }>(
      `${this.baseUrl}/warranties`,
      dto
    );
  }


  // ============================================================
  // POWERTRAIN
  // ============================================================

  updatePowertrain(
    importPowertrainId: number,
    dto: UpdateStagedPowertrainDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/powertrains/${importPowertrainId}`,
      dto
    );
  }


  createPowertrain(
    dto: CreateStagedPowertrainDto
  ): Observable<{ importPowertrainId: number }> {

    return this.http.post<{
      importPowertrainId: number
    }>(
      `${this.baseUrl}/powertrains`,
      dto
    );
  }

  createEngine(
  importPowertrainId: number,
  dto: CreateStagedEngineDto
  ): Observable<{ importEngineId: number }> {

    return this.http.post<{
      importEngineId: number
    }>(
      `${this.baseUrl}/powertrains/${importPowertrainId}/engines`,
      dto
    );
  }

  createEnginePerformance(
  importEngineId: number,
  dto: CreateStagedEnginePerformanceDto
  ): Observable<{ importEnginePerformanceId: number }> {

    return this.http.post<{
      importEnginePerformanceId: number
    }>(
      `${this.baseUrl}/engines/${importEngineId}/performances`,
      dto
    );
  }


  createMotorPerformance(
  importPowertrainId: number,
  dto: CreateStagedMotorPerformanceDto
  ): Observable<{ importMotorPerformanceId: number }> {

    return this.http.post<{
      importMotorPerformanceId: number
    }>(
      `${this.baseUrl}/powertrains/${importPowertrainId}/motor-performances`,
      dto
    );
  }


  // ============================================================
  // TRANSMISSION
  // ============================================================

  updateTransmission(
    importTransmissionId: number,
    dto: UpdateStagedTransmissionDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/transmissions/${importTransmissionId}`,
      dto
    );
  }


  createTransmission(
    dto: CreateStagedTransmissionDto
  ): Observable<{ importTransmissionId: number }> {

    return this.http.post<{
      importTransmissionId: number
    }>(
      `${this.baseUrl}/transmissions`,
      dto
    );
  }


  // ============================================================
  // DRIVETRAIN
  // ============================================================

  updateDrivetrain(
    importDrivetrainId: number,
    dto: UpdateStagedDrivetrainDto
  ): Observable<void> {

    return this.http.put<void>(
      `${this.baseUrl}/drivetrains/${importDrivetrainId}`,
      dto
    );
  }


  createDrivetrain(
    dto: CreateStagedDrivetrainDto
  ): Observable<{ importDrivetrainId: number }> {

    return this.http.post<{
      importDrivetrainId: number
    }>(
      `${this.baseUrl}/drivetrains`,
      dto
    );
  }

  getFeatureSpecificationCatalog(): Observable<StagedFeatureSpecificationCatalogDto> {
  return this.http.get<StagedFeatureSpecificationCatalogDto>(
    `${this.baseUrl}/feature-specification-catalog`
  );
  }


  getPowertrainsForModel(
  importModelId: number
): Observable<StagedPowertrainLookupDto[]> {
  return this.http.get<StagedPowertrainLookupDto[]>(
    `${this.baseUrl}/models/${importModelId}/powertrains`
  );
}

getTransmissionsForModel(
  importModelId: number
): Observable<StagedTransmissionLookupDto[]> {
  return this.http.get<StagedTransmissionLookupDto[]>(
    `${this.baseUrl}/models/${importModelId}/transmissions`
  );
}

getDrivetrainsForModel(
  importModelId: number
): Observable<StagedDrivetrainLookupDto[]> {
  return this.http.get<StagedDrivetrainLookupDto[]>(
    `${this.baseUrl}/models/${importModelId}/drivetrains`
  );
}

replaceVariantPowertrainReference(
  importVariantId: number,
  dto: ReplaceStagedPowertrainReferenceDto
): Observable<ReplaceStagedPowertrainReferenceResultDto> {
  return this.http.put<ReplaceStagedPowertrainReferenceResultDto>(
    `${this.baseUrl}/variants/${importVariantId}/powertrain-reference`,
    dto
  );
}

replaceVariantTransmissionReference(
  importVariantId: number,
  dto: ReplaceStagedTransmissionReferenceDto
): Observable<ReplaceStagedTransmissionReferenceResultDto> {
  return this.http.put<ReplaceStagedTransmissionReferenceResultDto>(
    `${this.baseUrl}/variants/${importVariantId}/transmission-reference`,
    dto
  );
}

replaceVariantDrivetrainReference(
  importVariantId: number,
  dto: ReplaceStagedDrivetrainReferenceDto
): Observable<ReplaceStagedDrivetrainReferenceResultDto> {
  return this.http.put<ReplaceStagedDrivetrainReferenceResultDto>(
    `${this.baseUrl}/variants/${importVariantId}/drivetrain-reference`,
    dto
  );
}


}
