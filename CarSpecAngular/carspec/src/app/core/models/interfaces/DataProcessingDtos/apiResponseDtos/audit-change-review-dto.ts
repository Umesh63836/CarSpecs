export interface AuditChangeReviewDto {
  changeId: string;
  issueType: string;
  category: string;
  severity: string;
  action: string;

  collection: string;
  indexId: number | null;
  identity: string;

  data: any;

  pageNumber: number | null;
  evidence: string | null;
  reason: string | null;

  decision: string;

  modifiedData: string | null;
  notes: string | null;
}
