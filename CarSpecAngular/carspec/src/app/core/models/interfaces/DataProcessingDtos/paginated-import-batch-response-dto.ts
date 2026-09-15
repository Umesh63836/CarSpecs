import { ImportBatchListDto } from "./import-batch-list-dto";

export interface PaginatedImportBatchResponseDto {
  pageNumber: number;

  pageSize: number;

  totalRecords: number;

  totalPages: number;

  hasPreviousPage: boolean;

  hasNextPage: boolean;

  batches: ImportBatchListDto[];
}
