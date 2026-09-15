import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { Observable } from 'rxjs';
import { BrochureImportReviewDto } from '../../../models/interfaces/DataProcessingDtos/apiResponseDtos/brochure-import-review-dto';
import { AdminAuditDecisionDto } from '../../../models/interfaces/DataProcessingDtos/apiResponseDtos/admin-audit-decision-dto';
import { SaveAdminReviewResponse } from '../../../models/interfaces/DataProcessingDtos/apiResponseDtos/save-admin-review-response';
import { ApproveReviewResponse } from '../../../models/interfaces/DataProcessingDtos/apiResponseDtos/approve-review-response';

@Service()
export class AdminReviewService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl + "/admin-review";

  getReview(importBatchId: number): Observable<BrochureImportReviewDto> {
    return this.http.get<BrochureImportReviewDto>(
      this.apiUrl + "/" + importBatchId
    );
  }

  saveDecisions(importBatchId: number, decisions: AdminAuditDecisionDto[], reviewedBy?: number | null): Observable<SaveAdminReviewResponse> {
    return this.http.post<SaveAdminReviewResponse>(
      this.apiUrl + "/" + importBatchId + "/decisions",
      {
        reviewedBy: reviewedBy ?? null,
        decisions
      }
    );
  }

  approveReview(importBatchId: number): Observable<ApproveReviewResponse> {
    return this.http.post<ApproveReviewResponse>(
      this.apiUrl + "/" + importBatchId + "/build-final",
      {}
    );
  }
}
