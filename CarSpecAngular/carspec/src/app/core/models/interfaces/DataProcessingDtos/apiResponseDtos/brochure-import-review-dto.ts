import { AuditChangeReviewDto } from "./audit-change-review-dto";

export interface BrochureImportReviewDto {
  importBatchId: number;
  status: string;
  model1: any;
  auditChanges: AuditChangeReviewDto[];
  warnings: string[];
}
