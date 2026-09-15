import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { Observable } from 'rxjs';
import { PaginatedImportBatchResponseDto } from '../../../models/interfaces/DataProcessingDtos/paginated-import-batch-response-dto';
import { StagedBatchResponse } from '../../../models/interfaces/DataProcessingDtos/apiResponseDtos/staged-batch-response';

@Service()
export class ImportBatchService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiUrl + '/import';
  /**
   * Get paginated import batches.
   * Backend returns maximum 10 batches per page.
   */
  getImportBatches(pageNumber: number): Observable<PaginatedImportBatchResponseDto> {
    const params = new HttpParams().set('pageNumber', pageNumber.toString());
    return this.http.get<PaginatedImportBatchResponseDto>(this.apiUrl + "/batches", { params } );
  }

  stageImportBatch(importBatchId: number): Observable<StagedBatchResponse> {
    return this.http.post<StagedBatchResponse>(this.apiUrl + "/import-batches/" + importBatchId + "/stage", null);
  }

}
