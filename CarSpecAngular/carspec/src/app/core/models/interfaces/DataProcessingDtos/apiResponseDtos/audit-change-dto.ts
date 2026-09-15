import { AuditChangeTargetDto } from "./audit-change-target-dto";

export interface AuditChangeDto {
      changeId: string;
  issueType: string;
  category: string;
  severity: string;
  action: string;
  target: AuditChangeTargetDto;
  data: any;
  pageNumber: number | null;
  evidence: string | null;
  reason: string | null;
}
