import {
  Component,
  OnInit,
  inject,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ImportBatchService } from '../../../core/services/AdminServices/AdminReviewService/import-batch-service';
import { ImportBatchListDto } from '../../../core/models/interfaces/DataProcessingDtos/import-batch-list-dto';
import { StagedBatchResponse } from '../../../core/models/interfaces/DataProcessingDtos/apiResponseDtos/staged-batch-response';

@Component({
  selector: 'app-batches',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './batches.html',
  styleUrl: './batches.css'
})
export class Batches implements OnInit {

  private readonly importBatchService =
    inject(ImportBatchService);

  private readonly router =
    inject(Router);

  stageResult = signal<StagedBatchResponse | null>(null);

isStageResultPopupOpen = signal(false);
  // =========================================================
  // DATA
  // =========================================================

  batches = signal<ImportBatchListDto[]>([]);


  // =========================================================
  // PAGINATION
  // =========================================================

  currentPage = signal(1);

  pageSize = signal(10);

  totalRecords = signal(0);

  totalPages = signal(0);

  hasPreviousPage = signal(false);

  hasNextPage = signal(false);


  // =========================================================
  // UI STATE
  // =========================================================

  isLoading = signal(false);

  /**
   * Separate loading state for the staging operation.
   *
   * This keeps the batch table visible while placing a
   * loading overlay over it.
   */
  isStaging = signal(false);

  errorMessage = signal<string | null>(null);


  /**
   * Batch selected from the Reviewed&Merged rows.
   */
  selectedBatchId = signal<number | null>(null);


  // =========================================================
  // FLOATING NOTIFICATION
  // =========================================================

  notificationMessage =
    signal<string | null>(null);

  notificationType =
    signal<'success' | 'error'>('success');

  private notificationTimer:
    ReturnType<typeof setTimeout> | null = null;


  // =========================================================
  // INITIAL LOAD
  // =========================================================

  ngOnInit(): void {

    this.loadBatches(1);

  }


  // =========================================================
  // LOAD BATCHES
  // =========================================================

  loadBatches(pageNumber: number): void {

    if (pageNumber < 1) {
      return;
    }


    if (
      this.totalPages() > 0 &&
      pageNumber > this.totalPages()
    ) {
      return;
    }


    this.isLoading.set(true);

    this.errorMessage.set(null);


    this.importBatchService
      .getImportBatches(pageNumber)
      .subscribe({

        next: (response: any) => {

          this.batches.set(
            response.batches ?? []
          );


          this.currentPage.set(
            response.pageNumber
          );


          this.pageSize.set(
            response.pageSize
          );


          this.totalRecords.set(
            response.totalRecords
          );


          this.totalPages.set(
            response.totalPages
          );


          this.hasPreviousPage.set(
            response.hasPreviousPage
          );


          this.hasNextPage.set(
            response.hasNextPage
          );


          /*
           * A reload clears the current selection.
           *
           * This is particularly important after staging because
           * the selected batch will come back as Staged and must
           * therefore no longer be selectable.
           */
          this.selectedBatchId.set(null);


          this.isLoading.set(false);

        },


        error: (error: unknown) => {

          console.error(
            'Failed to load import batches:',
            error
          );


          this.batches.set([]);


          this.errorMessage.set(
            'Unable to load import batches. Please try again.'
          );


          this.isLoading.set(false);

        }

      });

  }


  // =========================================================
  // REFRESH
  // =========================================================

  refreshBatches(): void {

    /*
     * Do not allow refresh while staging.
     *
     * The staging request owns the table loading state until
     * its response is received.
     */
    if (this.isStaging()) {
      return;
    }


    this.loadBatches(
      this.currentPage()
    );

  }


  // =========================================================
  // STATUS HELPERS
  // =========================================================

  private normalizeStatus(
    status: string | null | undefined
  ): string {

    return (status ?? '')
      .trim()
      .toLowerCase();

  }


  // =========================================================
  // CHECK IF BATCH IS SELECTABLE
  // =========================================================

  isBatchSelectable(
    batch: ImportBatchListDto
  ): boolean {

    const status =
      this.normalizeStatus(batch.status);


    /*
     * Staged is intentionally NOT included.
     *
     * Therefore a staged batch is completely inactive.
     */
    return (
      status === 'aicompleted' ||
      status === 'adminreview' ||
      status === 'reviewed&merged'
    );

  }


  // =========================================================
  // CHECK REVIEW STATUS
  // =========================================================

  isReviewAndMergeStatus(
    batch: ImportBatchListDto
  ): boolean {

    const status =
      this.normalizeStatus(batch.status);


    return (
      status === 'aicompleted' ||
      status === 'adminreview'
    );

  }


  // =========================================================
  // CHECK REVIEWED & MERGED
  // =========================================================

  isReviewedAndMerged(
    batch: ImportBatchListDto
  ): boolean {

    return (
      this.normalizeStatus(batch.status) ===
      'reviewed&merged'
    );

  }


  // =========================================================
  // CHECK STAGED
  // =========================================================

  isStaged(
    batch: ImportBatchListDto
  ): boolean {

    return (
      this.normalizeStatus(batch.status) ===
      'staged'
    );

  }


  // =========================================================
  // CHECK SELECTED
  // =========================================================

  isSelected(
    batch: ImportBatchListDto
  ): boolean {

    return (
      this.selectedBatchId() ===
      batch.importBatchId
    );

  }


  // =========================================================
  // GET ACTION LABEL
  // =========================================================

  getBatchActionLabel(
    batch: ImportBatchListDto
  ): string {

    if (this.isReviewAndMergeStatus(batch)) {

      return 'Review and Merge';

    }


    if (this.isReviewedAndMerged(batch)) {

      return 'Click to select';

    }


    return '';

  }


  // =========================================================
  // SELECT BATCH
  // =========================================================

  selectBatch(
    batch: ImportBatchListDto
  ): void {

    /*
     * Do nothing while staging.
     */
    if (this.isStaging()) {
      return;
    }


    /*
     * Staged and other inactive statuses cannot be selected.
     */
    if (!this.isBatchSelectable(batch)) {
      return;
    }


    /*
     * Reviewed&Merged behaves differently.
     *
     * Clicking it selects the row and exposes:
     *
     * Validate and stage
     * Preview and edit
     * Show review
     *
     * in the Import History header.
     */
    if (this.isReviewedAndMerged(batch)) {

      this.selectedBatchId.set(
        this.isSelected(batch)
          ? null
          : batch.importBatchId
      );

      return;
    }


    /*
     * Existing behavior for AICompleted/AdminReview:
     *
     * clicking the row opens the existing review page.
     */
    this.selectedBatchId.set(
      batch.importBatchId
    );


    this.router.navigate([
      'admindashboard',
      'admin-review',
      batch.importBatchId
    ]);

  }


  // =========================================================
  // KEYBOARD SELECTION
  // =========================================================

  onBatchKeydown(
    event: KeyboardEvent,
    batch: ImportBatchListDto
  ): void {

    if (
      this.isStaging() ||
      !this.isBatchSelectable(batch)
    ) {
      return;
    }


    if (
      event.key === 'Enter' ||
      event.key === ' '
    ) {

      event.preventDefault();

      this.selectBatch(batch);

    }

  }


  // =========================================================
  // GET SELECTED BATCH
  // =========================================================

  private getSelectedReviewedBatch():
    ImportBatchListDto | null {

    const batchId =
      this.selectedBatchId();


    if (batchId === null) {
      return null;
    }


    const batch =
      this.batches().find(
        x => x.importBatchId === batchId
      );


    if (
      !batch ||
      !this.isReviewedAndMerged(batch)
    ) {
      return null;
    }


    return batch;

  }


  // =========================================================
  // VALIDATE AND STAGE
  // =========================================================

  validateAndStageSelectedBatch(): void {

    const batch =
      this.getSelectedReviewedBatch();


    if (
      !batch ||
      this.isStaging()
    ) {
      return;
    }


    this.stageBatch(
      batch.importBatchId
    );

  }


  // =========================================================
  // STAGE BATCH
  // =========================================================

  private stageBatch(
    batchId: number
  ): void {

    /*
     * This controls the loading overlay across the complete
     * batches table.
     */
    this.isStaging.set(true);


    this.errorMessage.set(null);


    this.importBatchService
      .stageImportBatch(batchId)
      .subscribe({

next: (response: StagedBatchResponse) => {

  /*
   * Save the complete response returned by the API.
   */
  this.stageResult.set(response);

  /*
   * Open the staging result popup.
   */
  this.isStageResultPopupOpen.set(true);

  /*
   * Remove the selected batch.
   */
  this.selectedBatchId.set(null);

  /*
   * Refresh the table so the batch changes from
   * Reviewed&Merged -> Staged.
   *
   * Keep the loading state active until the refresh
   * has completed.
   */
  this.loadBatchesAfterStaging(batchId);

},


        error: (error: unknown) => {

          console.error(
            `Failed to validate and stage batch #${batchId}:`,
            error
          );


          this.isStaging.set(false);


          this.showNotification(
            'error',
            this.getStageErrorMessage(error)
          );

        }

      });

  }


  // =========================================================
  // RELOAD AFTER SUCCESSFUL STAGING
  // =========================================================

  private loadBatchesAfterStaging(
    batchId: number
  ): void {

    this.importBatchService
      .getImportBatches(
        this.currentPage()
      )
      .subscribe({

        next: (response: any) => {

          this.batches.set(
            response.batches ?? []
          );


          this.currentPage.set(
            response.pageNumber
          );


          this.pageSize.set(
            response.pageSize
          );


          this.totalRecords.set(
            response.totalRecords
          );


          this.totalPages.set(
            response.totalPages
          );


          this.hasPreviousPage.set(
            response.hasPreviousPage
          );


          this.hasNextPage.set(
            response.hasNextPage
          );


          this.selectedBatchId.set(null);

          this.isStaging.set(false);


          this.showNotification(
            'success',
            `Batch #${batchId} was successfully validated and staged.`
          );

        },


        error: (error: unknown) => {

          /*
           * The staging API itself succeeded, but refreshing
           * the table failed. We still stop the loading state
           * and tell the admin what happened.
           */
          console.error(
            'Batch was staged but the batch list could not be refreshed:',
            error
          );


          this.isStaging.set(false);


          this.showNotification(
            'success',
            `Batch #${batchId} was successfully validated and staged.`
          );


          this.errorMessage.set(
            'Batch was staged successfully, but the batch list could not be refreshed. Please refresh the page.'
          );

        }

      });

  }


  // =========================================================
  // STAGE ERROR MESSAGE
  // =========================================================

  private getStageErrorMessage(
    error: any
  ): string {

    const serverMessage =
      error?.error?.message ??
      error?.error?.title ??
      error?.error?.detail ??
      error?.message;


    if (
      typeof serverMessage === 'string' &&
      serverMessage.trim().length > 0
    ) {
      return serverMessage;
    }


    return (
      'Unable to validate and stage the batch. Please try again.'
    );

  }


  // =========================================================
  // PREVIEW AND EDIT
  // =========================================================

  previewAndEditSelectedBatch(): void {

    /*
     * Intentionally inactive.
     *
     * FinalJson preview/edit popup will be implemented later.
     */

    return;

  }


  // =========================================================
  // SHOW REVIEW
  // =========================================================

  showReviewSelectedBatch(): void {

    const batch =
      this.getSelectedReviewedBatch();


    if (
      !batch ||
      this.isStaging()
    ) {
      return;
    }


    this.router.navigate([
      'admindashboard',
      'admin-review',
      batch.importBatchId
    ]);

  }


  // =========================================================
  // FLOATING NOTIFICATION
  // =========================================================

  private showNotification(
    type: 'success' | 'error',
    message: string
  ): void {

    if (this.notificationTimer) {

      clearTimeout(
        this.notificationTimer
      );

    }


    this.notificationType.set(type);

    this.notificationMessage.set(message);


    this.notificationTimer =
      setTimeout(() => {

        this.notificationMessage.set(null);

        this.notificationTimer = null;

      }, 5000);

  }


  // =========================================================
  // CLOSE NOTIFICATION
  // =========================================================

  closeNotification(): void {

    if (this.notificationTimer) {

      clearTimeout(
        this.notificationTimer
      );

      this.notificationTimer = null;

    }


    this.notificationMessage.set(null);

  }


  // =========================================================
  // PAGE NUMBERS
  // =========================================================

  getPageNumbers(): number[] {

    const totalPages =
      this.totalPages();


    if (totalPages <= 0) {
      return [];
    }


    if (totalPages <= 7) {

      return Array.from(
        { length: totalPages },
        (_, index) => index + 1
      );

    }


    const current =
      this.currentPage();


    const pages: number[] = [];


    /*
     * First page
     */
    pages.push(1);


    /*
     * Ellipsis before current range
     */
    if (current > 4) {
      pages.push(-1);
    }


    /*
     * Current page range
     */
    const start =
      Math.max(2, current - 1);


    const end =
      Math.min(
        totalPages - 1,
        current + 1
      );


    for (
      let page = start;
      page <= end;
      page++
    ) {

      pages.push(page);

    }


    /*
     * Ellipsis after current range
     */
    if (
      current <
      totalPages - 3
    ) {
      pages.push(-1);
    }


    /*
     * Last page
     */
    pages.push(totalPages);


    return pages;

  }


  // =========================================================
  // NEXT PAGE
  // =========================================================

  nextPage(): void {

    if (
      !this.hasNextPage() ||
      this.isStaging()
    ) {
      return;
    }


    this.loadBatches(
      this.currentPage() + 1
    );

  }


  // =========================================================
  // PREVIOUS PAGE
  // =========================================================

  previousPage(): void {

    if (
      !this.hasPreviousPage() ||
      this.isStaging()
    ) {
      return;
    }


    this.loadBatches(
      this.currentPage() - 1
    );

  }


  // =========================================================
  // GO TO PAGE
  // =========================================================

  goToPage(
    page: number
  ): void {

    if (
      page < 1 ||
      page > this.totalPages() ||
      page === this.currentPage() ||
      this.isStaging()
    ) {
      return;
    }


    this.loadBatches(page);

  }


  // =========================================================
  // LAST RECORD NUMBER
  // =========================================================

  getLastRecordNumber(): number {

    return Math.min(
      this.currentPage() *
      this.pageSize(),
      this.totalRecords()
    );

  }


  // =========================================================
  // TRACK BY
  // =========================================================

  trackByBatchId(
    _index: number,
    batch: ImportBatchListDto
  ): number {

    return batch.importBatchId;

  }


  // =========================================================
  // FORMAT FILE SIZE
  // =========================================================

  formatFileSize(
    bytes: number | null
  ): string {

    if (
      bytes === null ||
      bytes === undefined ||
      bytes <= 0
    ) {
      return '—';
    }


    const units = [
      'B',
      'KB',
      'MB',
      'GB'
    ];


    const index = Math.floor(
      Math.log(bytes) /
      Math.log(1024)
    );


    const safeIndex =
      Math.min(
        index,
        units.length - 1
      );


    const size =
      bytes /
      Math.pow(
        1024,
        safeIndex
      );


    return `${size.toFixed(2)} ${units[safeIndex]}`;

  }


  // =========================================================
  // FORMAT DATE
  // =========================================================

  formatDate(
    value: string | null
  ): string {

    if (!value) {
      return '—';
    }


    const date =
      new Date(value);


    if (
      Number.isNaN(
        date.getTime()
      )
    ) {
      return '—';
    }


    return date.toLocaleString(
      'en-IN',
      {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }
    );

  }


  // =========================================================
  // STATUS CLASS
  // =========================================================

  getStatusClass(
    status: string
  ): string {

    switch (
      this.normalizeStatus(status)
    ) {

      case 'completed':

        return (
          'bg-green-50 text-green-700 border-green-200'
        );


      case 'aicompleted':

        return (
          'bg-blue-50 text-blue-700 border-blue-200'
        );


      case 'adminreview':

        return (
          'bg-orange-50 text-orange-700 border-orange-200'
        );


      case 'reviewed&merged':

        return (
          'bg-purple-50 text-purple-700 border-purple-200'
        );


      case 'staged':

        return (
          'bg-emerald-50 text-emerald-700 border-emerald-200'
        );


      case 'processing':

        return (
          'bg-blue-50 text-blue-700 border-blue-200'
        );


      case 'pending':

        return (
          'bg-yellow-50 text-yellow-700 border-yellow-200'
        );


      case 'failed':

        return (
          'bg-red-50 text-red-700 border-red-200'
        );


      default:

        return (
          'bg-gray-50 text-gray-700 border-gray-200'
        );

    }

  }


  // =========================================================
  // DOCUMENT SUMMARY
  // =========================================================

  getDocumentSummary(
    batch: ImportBatchListDto
  ): string {

    if (
      !batch.documents ||
      batch.documents.length === 0
    ) {
      return 'No document';
    }


    if (
      batch.documents.length === 1
    ) {

      return batch.documents[0].documentName;

    }


    return (
      `${batch.documents.length} documents`
    );
  }

  // =========================================================
// STAGING RESULT POPUP
// =========================================================

closeStageResultPopup(): void {

  this.isStageResultPopupOpen.set(false);

  this.stageResult.set(null);

}


// =========================================================
// STAGING RESULT WARNING COUNT
// =========================================================

getStageWarningCount(): number {

  return this.stageResult()?.warnings?.length ?? 0;

}

}