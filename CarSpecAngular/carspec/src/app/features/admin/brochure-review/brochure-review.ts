import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { AdminReviewService } from '../../../core/services/AdminServices/AdminReviewService/admin-review-service';

import { BrochureImportReviewDto } from '../../../core/models/interfaces/DataProcessingDtos/apiResponseDtos/brochure-import-review-dto';
import { AuditChangeReviewDto } from '../../../core/models/interfaces/DataProcessingDtos/apiResponseDtos/audit-change-review-dto';
import { AdminAuditDecisionDto } from '../../../core/models/interfaces/DataProcessingDtos/apiResponseDtos/admin-audit-decision-dto';


/* ============================================================
   MODEL 1 VIEW TYPE
   ============================================================ */

interface Model1Section {
  key: string;
  label: string;
  value: unknown;
  itemCount: number | null;
  type: 'array' | 'object' | 'value';
}


/* ============================================================
   COMPONENT
   ============================================================ */

@Component({
  selector: 'app-brochure-review',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './brochure-review.html',
  styleUrl: './brochure-review.css',
})
export class BrochureReview implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly adminReviewService = inject(AdminReviewService);


  // ============================================================
  // BASIC STATE
  // ============================================================

  importBatchId = signal(0);

  review = signal<BrochureImportReviewDto | null>(null);

  loading = signal(false);
  saving = signal(false);
  approving = signal(false);

  errorMessage = signal('');
  successMessage = signal('');


  // ============================================================
  // AUDIT SEARCH / FILTER
  // ============================================================

  activeFilter = signal('All');

  searchTerm = signal('');


  // ============================================================
  // AUDIT EXPANSION
  // ============================================================

  expandedData = signal<Set<string>>(new Set());

  expandedEvidence = signal<Set<string>>(new Set());


  // ============================================================
  // MODEL 1 JSON
  // ============================================================

  /*
   * Model 1 is supplied directly by BrochureImportReviewDto.
   *
   * It is READ ONLY on this page.
   *
   * We never modify this object.
   */

  expandedModelSections = signal<Set<string>>(new Set());

  model1PanelExpanded = signal(true);


  // ============================================================
  // INITIALIZATION
  // ============================================================

  ngOnInit(): void {

    const rawId =
      this.route.snapshot.paramMap.get('batchId');

    const id =
      Number(rawId);

    if (
      !Number.isInteger(id) ||
      id <= 0
    ) {

      this.errorMessage.set(
        'Invalid import batch ID.'
      );

      return;
    }

    this.importBatchId.set(id);

    this.loadReview();
  }


  // ============================================================
  // LOAD REVIEW
  // ============================================================

  loadReview(): void {

    const batchId =
      this.importBatchId();

    if (
      !Number.isInteger(batchId) ||
      batchId <= 0
    ) {

      this.errorMessage.set(
        'Unable to load review because the import batch ID is invalid.'
      );

      return;
    }


    /*
     * Prevent duplicate API calls while another operation
     * is already running.
     */
    if (
      this.loading() ||
      this.saving() ||
      this.approving()
    ) {
      return;
    }


    this.loading.set(true);

    this.errorMessage.set('');
    this.successMessage.set('');


    this.adminReviewService
      .getReview(batchId)
      .subscribe({

        next: (
          result: BrochureImportReviewDto
        ) => {

          if (!result) {

            this.review.set(null);

            this.errorMessage.set(
              'The review API returned an empty response.'
            );

            this.loading.set(false);

            return;
          }


          /*
           * Store the complete API response.
           *
           * This includes:
           * - status
           * - model1
           * - auditChanges
           * - warnings
           */
          this.review.set(result);


          /*
           * Reset audit UI state.
           */
          this.activeFilter.set('All');
          this.searchTerm.set('');

          this.expandedData.set(
            new Set()
          );

          this.expandedEvidence.set(
            new Set()
          );


          /*
           * Open useful Model 1 categories automatically.
           */
          this.expandedModelSections.set(
            this.getDefaultExpandedModelSections()
          );

          this.model1PanelExpanded.set(true);


          this.loading.set(false);
        },


        error: (error) => {

          console.error(
            'Failed to load admin review:',
            error
          );


          this.loading.set(false);

          this.review.set(null);


          this.errorMessage.set(
            this.getApiErrorMessage(
              error,
              'Failed to load admin review.'
            )
          );
        }

      });
  }


  // ============================================================
  // MODEL 1
  // ============================================================

  /**
   * Returns the Model 1 JSON directly from the API response.
   */
  get model1(): unknown {

    return this.review()?.model1 ?? null;
  }


  /**
   * Returns true when Model 1 contains usable data.
   */
  get hasModel1Data(): boolean {

    const model =
      this.model1;

    return (
      model !== null &&
      model !== undefined
    );
  }


  /**
   * Converts the top-level Model 1 JSON properties into
   * expandable sections.
   *
   * Example:
   *
   * {
   *   variants: [...],
   *   specifications: [...],
   *   features: [...]
   * }
   *
   * becomes:
   *
   * Variants
   * Specifications
   * Features
   */
  get model1Sections(): Model1Section[] {

    const model =
      this.model1;


    if (
      model === null ||
      model === undefined
    ) {

      return [];
    }


    /*
     * If Model 1 is an array or primitive value, show the
     * complete value as one section.
     */
    if (
      typeof model !== 'object' ||
      Array.isArray(model)
    ) {

      return [
        {
          key: '__root',
          label: 'Model 1 Result',
          value: model,
          itemCount: null,
          type: 'value'
        }
      ];
    }


    const entries =
      Object.entries(
        model as Record<string, unknown>
      );


    return entries.map(
      ([key, value]) => {

        if (
          Array.isArray(value)
        ) {

          return {
            key,
            label: this.formatModelSectionName(key),
            value,
            itemCount: value.length,
            type: 'array'
          };
        }


        if (
          value !== null &&
          typeof value === 'object'
        ) {

          return {
            key,
            label: this.formatModelSectionName(key),
            value,
            itemCount: Object.keys(
              value as Record<string, unknown>
            ).length,
            type: 'object'
          };
        }


        return {
          key,
          label: this.formatModelSectionName(key),
          value,
          itemCount: null,
          type: 'value'
        };
      }
    );
  }


  /**
   * Converts JSON property names into readable labels.
   *
   * specifications -> Specifications
   * fuelEfficiencies -> Fuel Efficiencies
   */
  private formatModelSectionName(
    key: string
  ): string {

    if (!key) {
      return 'Unknown';
    }


    return key
      .replace(
        /([a-z])([A-Z])/g,
        '$1 $2'
      )
      .replace(
        /[_-]+/g,
        ' '
      )
      .replace(
        /\s+/g,
        ' '
      )
      .trim()
      .replace(
        /\b\w/g,
        char => char.toUpperCase()
      );
  }


  /**
   * Opens the most useful Model 1 sections when the page
   * initially loads.
   */
  private getDefaultExpandedModelSections():
    Set<string> {

    const preferredSections = [
      'models',
      'model',
      'variants',
      'specifications',
      'features'
    ];


    const expanded =
      new Set<string>();


    for (
      const section of this.model1Sections
    ) {

      if (
        preferredSections.includes(
          section.key.toLowerCase()
        )
      ) {

        expanded.add(
          section.key
        );
      }
    }


    return expanded;
  }


  isModelSectionExpanded(
    key: string
  ): boolean {

    if (!key) {
      return false;
    }


    return this.expandedModelSections()
      .has(key);
  }


  toggleModelSection(
    key: string
  ): void {

    if (!key) {
      return;
    }


    const sections =
      new Set(
        this.expandedModelSections()
      );


    if (
      sections.has(key)
    ) {

      sections.delete(key);

    } else {

      sections.add(key);
    }


    this.expandedModelSections.set(
      sections
    );
  }


  expandAllModelSections(): void {

    this.expandedModelSections.set(
      new Set(
        this.model1Sections.map(
          section => section.key
        )
      )
    );
  }


  collapseAllModelSections(): void {

    this.expandedModelSections.set(
      new Set()
    );
  }


  toggleModelPanel(): void {

    this.model1PanelExpanded.update(
      expanded => !expanded
    );
  }


  // ============================================================
  // AUDIT DECISIONS
  // ============================================================

  setDecision(
    change: AuditChangeReviewDto,
    decision: string
  ): void {

    if (
      this.saving() ||
      this.approving()
    ) {
      return;
    }


    if (!change) {
      return;
    }


    const allowedDecisions = [
      'Accepted',
      'Rejected',
      'Modified'
    ];


    if (
      !allowedDecisions.includes(
        decision
      )
    ) {

      this.errorMessage.set(
        `Invalid audit decision '${decision}'.`
      );

      return;
    }


    change.decision =
      decision;


    /*
     * Modified Data only belongs to a Modified decision.
     */
    if (
      decision !== 'Modified'
    ) {

      change.modifiedData =
        null;
    }


    this.errorMessage.set('');
    this.successMessage.set('');
  }


  // ============================================================
  // SAVE DECISIONS
  // ============================================================

  saveDecisions(): void {

    const currentReview =
      this.review();


    if (!currentReview) {

      this.errorMessage.set(
        'There is no review loaded.'
      );

      return;
    }


    if (
      this.saving() ||
      this.approving()
    ) {
      return;
    }


    this.errorMessage.set('');
    this.successMessage.set('');


    const changes =
      Array.isArray(
        currentReview.auditChanges
      )
        ? currentReview.auditChanges
        : [];


    /*
     * Every change must have a decision.
     */
    const pending =
      changes.filter(
        change =>
          !change ||
          change.decision === 'Pending' ||
          !change.decision
      );


    if (
      pending.length > 0
    ) {

      this.errorMessage.set(
        `${pending.length} audit change(s) are still Pending.`
      );

      this.activeFilter.set(
        'Pending'
      );

      return;
    }


    /*
     * Only these decisions are allowed.
     */
    const invalidDecision =
      changes.filter(
        change =>
          ![
            'Accepted',
            'Rejected',
            'Modified'
          ].includes(
            change.decision
          )
      );


    if (
      invalidDecision.length > 0
    ) {

      this.errorMessage.set(
        `${invalidDecision.length} audit change(s) contain an invalid decision.`
      );

      return;
    }


    /*
     * Modified changes require data.
     */
    const modified =
      changes.filter(
        change =>
          change.decision === 'Modified'
      );


    const missingModifiedData =
      modified.filter(
        change =>
          !this.hasModifiedData(
            change.modifiedData
          )
      );


    if (
      missingModifiedData.length > 0
    ) {

      this.errorMessage.set(
        `${missingModifiedData.length} Modified change(s) are missing Modified Data.`
      );

      return;
    }


    /*
     * Modified Data must be valid JSON.
     */
    const invalidJson =
      modified.filter(
        change =>
          !this.isValidJson(
            change.modifiedData
          )
      );


    if (
      invalidJson.length > 0
    ) {

      this.errorMessage.set(
        `${invalidJson.length} Modified change(s) contain invalid JSON.`
      );

      return;
    }


    /*
     * Every change must have a valid Change ID.
     */
    const invalidIds =
      changes.filter(
        change =>
          !change.changeId ||
          typeof change.changeId !== 'string' ||
          !change.changeId.trim()
      );


    if (
      invalidIds.length > 0
    ) {

      this.errorMessage.set(
        `${invalidIds.length} audit change(s) have an invalid Change ID.`
      );

      return;
    }


    const decisions:
      AdminAuditDecisionDto[] =
        changes.map(
          change => ({

            changeId:
              change.changeId.trim(),

            decision:
              change.decision,

            data:
              change.decision === 'Modified'
                ? this.getModifiedDataForApi(
                    change.modifiedData
                  )
                : change.decision === 'Accepted'
                  ? this.getOriginalDataForApi(
                      change.data
                    )
                  : null,

            notes:
              change.notes?.trim()
                ? change.notes.trim()
                : null
          })
        );


    this.saving.set(true);


    this.adminReviewService
      .saveDecisions(
        this.importBatchId(),
        decisions
      )
      .subscribe({

        next: (response) => {

          this.saving.set(false);


          this.successMessage.set(
            response?.message ??
            'Review decisions saved successfully.'
          );


          /*
           * Reload persisted state from backend.
           */
          this.loadReview();
        },


        error: (error) => {

          console.error(
            'Failed to save review decisions:',
            error
          );


          this.saving.set(false);


          this.errorMessage.set(
            this.getApiErrorMessage(
              error,
              'Failed to save review decisions.'
            )
          );
        }

      });
  }


  // ============================================================
  // APPROVE REVIEW
  // ============================================================

  approveReview(): void {

    const currentReview =
      this.review();


    if (!currentReview) {

      this.errorMessage.set(
        'There is no review loaded.'
      );

      return;
    }


    if (
      this.approving() ||
      this.saving()
    ) {
      return;
    }


    if (
      currentReview.status === 'Approved'
    ) {
      return;
    }


    if (
      this.pendingCount > 0
    ) {

      this.errorMessage.set(
        'All audit changes must be reviewed before approval.'
      );

      this.activeFilter.set(
        'Pending'
      );

      return;
    }


    /*
     * Revalidate Modified Data before approval.
     */
    const modified =
      currentReview.auditChanges.filter(
        change =>
          change.decision === 'Modified'
      );


    const invalidModified =
      modified.filter(
        change =>
          !this.isValidJson(
            change.modifiedData
          )
      );


    if (
      invalidModified.length > 0
    ) {

      this.errorMessage.set(
        `${invalidModified.length} Modified change(s) contain invalid JSON.`
      );

      return;
    }


    const confirmed =
      window.confirm(
        'Approve this review?\n' +
        'Accepted and Modified audit changes will be merged into the final JSON and saved as FinalJson.\n\n'
      );


    if (!confirmed) {
      return;
    }


    this.approving.set(true);

    this.errorMessage.set('');
    this.successMessage.set('');


    this.adminReviewService
      .approveReview(
        this.importBatchId()
      )
      .subscribe({

        next: (response) => {

          this.approving.set(false);


          this.successMessage.set(
            response?.message ??
            'Review approved successfully.'
          );


          /*
           * Update local status so the UI immediately becomes
           * read-only.
           */
          this.review.update(
            current =>
              current
                ? {
                    ...current,
                    status: 'Approved'
                  }
                : current
          );
        },


        error: (error) => {

          console.error(
            'Failed to approve review:',
            error
          );


          this.approving.set(false);


          this.errorMessage.set(
            this.getApiErrorMessage(
              error,
              'Failed to approve review.'
            )
          );
        }

      });
  }


  // ============================================================
  // FILTER / SEARCH
  // ============================================================

  setFilter(
    filter: string
  ): void {

    const allowedFilters = [
      'All',
      'Pending',
      'Accepted',
      'Modified',
      'Rejected'
    ];


    if (
      !allowedFilters.includes(
        filter
      )
    ) {
      return;
    }


    this.activeFilter.set(
      filter
    );
  }


  setSearchTerm(
    value: string
  ): void {

    this.searchTerm.set(
      typeof value === 'string'
        ? value
        : ''
    );
  }


  clearSearch(): void {

    this.searchTerm.set('');
  }


  get filteredChanges():
    AuditChangeReviewDto[] {

    const currentReview =
      this.review();


    if (
      !currentReview ||
      !Array.isArray(
        currentReview.auditChanges
      )
    ) {

      return [];
    }


    const filter =
      this.activeFilter();


    const search =
      this.searchTerm()
        .trim()
        .toLowerCase();


    return currentReview.auditChanges
      .filter(
        change => {

          if (!change) {
            return false;
          }


          if (
            filter !== 'All' &&
            change.decision !== filter
          ) {

            return false;
          }


          if (!search) {
            return true;
          }


          const searchableText = [
            change.changeId,
            change.issueType,
            change.category,
            change.severity,
            change.action,
            change.collection,
            change.identity,
            change.reason,
            change.evidence
          ]
            .filter(
              value =>
                value !== null &&
                value !== undefined
            )
            .join(' ')
            .toLowerCase();


          return searchableText.includes(
            search
          );
        }
      );
  }


  // ============================================================
  // COUNTS
  // ============================================================

  get totalCount(): number {

    return (
      this.review()
        ?.auditChanges
        ?.length ??
      0
    );
  }


  get pendingCount(): number {

    return (
      this.review()
        ?.auditChanges
        ?.filter(
          change =>
            !change ||
            change.decision === 'Pending' ||
            !change.decision
        )
        .length ??
      0
    );
  }


  get acceptedCount(): number {

    return (
      this.review()
        ?.auditChanges
        ?.filter(
          change =>
            change?.decision === 'Accepted'
        )
        .length ??
      0
    );
  }


  get rejectedCount(): number {

    return (
      this.review()
        ?.auditChanges
        ?.filter(
          change =>
            change?.decision === 'Rejected'
        )
        .length ??
      0
    );
  }


  get modifiedCount(): number {

    return (
      this.review()
        ?.auditChanges
        ?.filter(
          change =>
            change?.decision === 'Modified'
        )
        .length ??
      0
    );
  }


  get reviewedCount(): number {

    return (
      this.acceptedCount +
      this.rejectedCount +
      this.modifiedCount
    );
  }


  get reviewProgress(): number {

    if (
      this.totalCount <= 0
    ) {
      return 0;
    }


    return Math.min(
      100,
      Math.max(
        0,
        Math.round(
          (
            this.reviewedCount /
            this.totalCount
          ) * 100
        )
      )
    );
  }


  // ============================================================
  // AUDIT DATA EXPANSION
  // ============================================================

  isDataExpanded(
    changeId: string
  ): boolean {

    if (!changeId) {
      return false;
    }


    return this.expandedData()
      .has(changeId);
  }


  toggleData(
    changeId: string
  ): void {

    if (!changeId) {
      return;
    }


    const expanded =
      new Set(
        this.expandedData()
      );


    if (
      expanded.has(changeId)
    ) {

      expanded.delete(changeId);

    } else {

      expanded.add(changeId);
    }


    this.expandedData.set(
      expanded
    );
  }


  isEvidenceExpanded(
    changeId: string
  ): boolean {

    if (!changeId) {
      return false;
    }


    return this.expandedEvidence()
      .has(changeId);
  }


  toggleEvidence(
    changeId: string
  ): void {

    if (!changeId) {
      return;
    }


    const expanded =
      new Set(
        this.expandedEvidence()
      );


    if (
      expanded.has(changeId)
    ) {

      expanded.delete(changeId);

    } else {

      expanded.add(changeId);
    }


    this.expandedEvidence.set(
      expanded
    );
  }


  // ============================================================
  // JSON HELPERS
  // ============================================================

  isValidJson(
    value: any
  ): boolean {

    if (
      value === null ||
      value === undefined
    ) {

      return false;
    }

    if (
      typeof value === 'string'
    ) {

      if (!value.trim()) {
        return false;
      }

      try {

        JSON.parse(value);

        return true;

      } catch {

        return false;
      }
    }

    try {

      JSON.stringify(value);

      return true;

    } catch {

      return false;
    }
  }


  getJsonError(
    value: any
  ): string {

    if (
      !this.hasModifiedData(value)
    ) {

      return 'Modified data is required.';
    }

    if (
      typeof value !== 'string'
    ) {

      return '';
    }

    try {

      JSON.parse(value);

      return '';

    } catch (error) {

      return error instanceof Error
        ? error.message
        : 'Invalid JSON.';
    }
  }


  /**
   * Checks whether Modified Data contains a usable value.
   */
  private hasModifiedData(
    value: any
  ): boolean {

    if (
      value === null ||
      value === undefined
    ) {

      return false;
    }

    if (
      typeof value === 'string'
    ) {

      return value.trim().length > 0;
    }

    return true;
  }


  /**
   * Converts Modified Data into the JSON value expected by
   * AdminAuditDecisionDto.Data.
   *
   * If the UI contains JSON text, parse it before sending.
   * If it already contains an object/value, send it as-is.
   */
  /**
   * Converts the original audit data into the JSON value expected by
   * AdminAuditDecisionDto.Data when an audit change is Accepted.
   *
   * Accepted changes must send the original audit data because the
   * backend validates Add/Replace data as a JSON object.
   * Rejected changes send null and Modified changes send modifiedData.
   */
  private getOriginalDataForApi(
    value: any
  ): any {

    if (
      value === null ||
      value === undefined
    ) {
      return null;
    }

    if (
      typeof value !== 'string'
    ) {
      return value;
    }

    if (!value.trim()) {
      return null;
    }

    try {
      return JSON.parse(value);
    } catch {
      return value;
    }
  }


  private getModifiedDataForApi(
    value: any
  ): any {

    if (
      !this.hasModifiedData(value)
    ) {

      return null;
    }

    if (
      typeof value !== 'string'
    ) {

      return value;
    }

    return JSON.parse(value);
  }


  formatJson(
    value: unknown
  ): string {

    if (
      value === null ||
      value === undefined
    ) {

      return '';
    }


    try {

      return JSON.stringify(
        value,
        null,
        2
      );

    } catch {

      return String(value);
    }
  }


  // ============================================================
  // COPY
  // ============================================================

  async copyData(
    value: unknown
  ): Promise<void> {

    if (
      value === null ||
      value === undefined
    ) {

      return;
    }


    const text =
      this.formatJson(value);


    if (!text) {
      return;
    }


    try {

      /*
       * Modern browser Clipboard API.
       */
      if (
        navigator.clipboard &&
        typeof navigator.clipboard.writeText === 'function'
      ) {

        await navigator.clipboard.writeText(
          text
        );

      } else {

        /*
         * Fallback for browsers/contexts where Clipboard API
         * is unavailable.
         */
        const textarea =
          document.createElement(
            'textarea'
          );


        textarea.value =
          text;


        textarea.style.position =
          'fixed';

        textarea.style.left =
          '-9999px';


        document.body.appendChild(
          textarea
        );


        textarea.select();


        document.execCommand(
          'copy'
        );


        document.body.removeChild(
          textarea
        );
      }


      this.successMessage.set(
        'Data copied to clipboard.'
      );

    } catch (error) {

      console.error(
        'Failed to copy data:',
        error
      );


      this.errorMessage.set(
        'Unable to copy data to clipboard.'
      );
    }
  }


  // ============================================================
  // UI HELPERS
  // ============================================================

  getSeverityClass(
    severity: string | null | undefined
  ): string {

    switch (
      severity?.toLowerCase()
    ) {

      case 'critical':
        return 'severity-critical';

      case 'high':
        return 'severity-high';

      case 'medium':
        return 'severity-medium';

      case 'low':
        return 'severity-low';

      default:
        return 'severity-unknown';
    }
  }


  getDecisionClass(
    decision: string | null | undefined
  ): string {

    switch (decision) {

      case 'Accepted':
        return 'decision-accepted';

      case 'Rejected':
        return 'decision-rejected';

      case 'Modified':
        return 'decision-modified';

      default:
        return 'decision-pending';
    }
  }


  getActionClass(
    action: string | null | undefined
  ): string {

    switch (
      action?.toLowerCase()
    ) {

      case 'add':
        return 'action-add';

      case 'replace':
        return 'action-replace';

      case 'remove':
        return 'action-remove';

      default:
        return 'action-default';
    }
  }


  getModelSectionIcon(
    key: string
  ): string {

    switch (
      key?.toLowerCase()
    ) {

      case 'variants':
        return 'variants';

      case 'specifications':
      case 'specs':
        return 'specifications';

      case 'features':
        return 'features';

      case 'powertrains':
        return 'powertrains';

      case 'engines':
        return 'engines';

      case 'transmissions':
        return 'transmissions';

      case 'drivetrains':
        return 'drivetrains';

      case 'fuelefficiencies':
      case 'fuel-efficiencies':
        return 'fuel';

      case 'warranties':
        return 'warranty';

      default:
        return 'default';
    }
  }


  getApiErrorMessage(
    error: any,
    fallback: string
  ): string {

    const message =
      error?.error?.message ??
      error?.error?.title ??
      error?.message;


    if (
      typeof message === 'string' &&
      message.trim()
    ) {

      return message.trim();
    }


    if (
      typeof error?.error === 'string' &&
      error.error.trim()
    ) {

      return error.error.trim();
    }


    return fallback;
  }


  dismissError(): void {

    this.errorMessage.set('');
  }


  dismissSuccess(): void {

    this.successMessage.set('');
  }


  // ============================================================
  // TRACKING
  // ============================================================

  trackByChangeId(
    index: number,
    change: AuditChangeReviewDto
  ): string {

    return (
      change?.changeId ||
      `change-${index}`
    );
  }


  trackByModelSection(
    index: number,
    section: Model1Section
  ): string {

    return (
      section?.key ||
      `section-${index}`
    );
  }
}