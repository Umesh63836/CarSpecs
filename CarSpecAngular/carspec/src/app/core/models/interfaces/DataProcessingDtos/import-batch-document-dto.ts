export interface ImportBatchDocumentDto {
  importDocumentId: number;
  documentType: string;
  documentName: string;
  fileSizeBytes: number | null;
}
