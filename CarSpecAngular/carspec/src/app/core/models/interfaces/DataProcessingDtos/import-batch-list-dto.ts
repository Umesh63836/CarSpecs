import { ImportBatchDocumentDto } from "./import-batch-document-dto";

export interface ImportBatchListDto {
  importBatchId: number;

  startedAt: string;

  completedAt: string | null;

  status: string;

  importedBy: number | null;

  notes: string | null;

  aiModel: string | null;

  aiProcessedAt: string | null;

  hasAIResult: boolean;

  hasFinalResult: boolean;

  documents: ImportBatchDocumentDto[];
}
