import {ChangeDetectionStrategy, Component, inject, OnDestroy, signal} from '@angular/core';
import { BrochureImportService } from '../../../core/services/BrochureUploadServices/brochure-import-service';
import { PdfPage } from '../../../core/models/interfaces/DataProcessingDtos/pdf-page';

@Component({
  selector: 'app-brochure-import',
  imports: [],
  templateUrl: './brochure-import.html',
  styleUrl: './brochure-import.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class BrochureImport implements OnDestroy {
  // Services
  private readonly brochureImportService = inject(
    BrochureImportService
  );

  // PDF / Import State
  selectedFile = signal<File | null>(null);
  pages = signal<PdfPage[]>([]);
  uploading = signal(false);
  processing = signal(false);
  generatingThumbnails = signal(false);
  importDocumentId = signal<number | null>(null);
  private thumbnailGenerationId = 0;

  // Data Source Metadata
  sourceName = signal('');
  sourceUrl = signal('');
  publisher = signal('');
  publishedDate = signal('');
  notes = signal('');

  // File Selection

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) {
      return;
    }
    const file = input.files[0];
    if (
      file.type !== 'application/pdf' &&
      !file.name.toLowerCase().endsWith('.pdf')
    ) {
      alert('Please select a PDF file.');
      input.value = '';
      return;
    }
    if (file.size > 25 * 1024 * 1024) {
      alert('Brochure cannot be larger than 25 MB.');
      input.value = '';
      return;
    }
    this.selectedFile.set(file);
    // Reset previous import state
    this.importDocumentId.set(null);
    this.revokePageUrls(this.pages());
    // Generate browser-side thumbnails
    await this.generatePageThumbnails(file);
  }

  // Generate PDF Page Thumbnails

  async generatePageThumbnails(file: File): Promise<void> {
    const generationId = ++this.thumbnailGenerationId;
    this.generatingThumbnails.set(true);
    this.pages.set([]);
    try {
      const pdfjsLib = await import('pdfjs-dist');
      pdfjsLib.GlobalWorkerOptions.workerSrc = new URL(
        // 'pdfjs-dist/build/pdf.worker.mjs',
        '/assets/pdfjs/pdf.worker.mjs',
        import.meta.url
      ).toString();
      const arrayBuffer = await file.arrayBuffer();
      const pdf = await pdfjsLib.getDocument({data: arrayBuffer}).promise;
      for (let pageNumber = 1; pageNumber <= pdf.numPages; pageNumber++) {
        if (generationId !== this.thumbnailGenerationId) {
          return;
        }
        const page = await pdf.getPage(pageNumber);
        const viewport = page.getViewport({scale: 0.25});
        const canvas = document.createElement('canvas');
        canvas.width = Math.floor(viewport.width);
        canvas.height = Math.floor(viewport.height);
        const context = canvas.getContext('2d');
        if (!context) {
          page.cleanup();
          continue;
        }
        await page.render({canvas, canvasContext: context, viewport}).promise;
        const imageUrl = await this.createThumbnailUrl(canvas);
        if (generationId !== this.thumbnailGenerationId) {
          URL.revokeObjectURL(imageUrl);
          return;
        }
        this.pages.update(currentPages => [
          ...currentPages,
          {pageNumber, imageUrl, selected: false}
        ]);
        page.cleanup();
        canvas.width = 0;
        canvas.height = 0;
        await new Promise(resolve => setTimeout(resolve, 0));
      }
    } catch (error) {
      if (generationId !== this.thumbnailGenerationId) {
        return;
      }
      console.error('Failed to generate PDF thumbnails:', error);
      alert('Unable to read the PDF. Please try another file.');
      this.pages.set([]);
    } finally {
      if (generationId === this.thumbnailGenerationId) {
        this.generatingThumbnails.set(false);
      }
    }
  }


  private createThumbnailUrl(canvas: HTMLCanvasElement): Promise<string> {
    return new Promise((resolve, reject) => {
      canvas.toBlob(blob => {
        if (blob) {
          resolve(URL.createObjectURL(blob));
        } else {
          reject(new Error('Unable to create PDF thumbnail.'));
        }
      }, 'image/jpeg', 0.7);
    });
  }

  private revokePageUrls(pages: PdfPage[]): void {
    for (const page of pages) {
      if (page.imageUrl.startsWith('blob:')) {
        URL.revokeObjectURL(page.imageUrl);
      }
    }
  }

  ngOnDestroy(): void {
    this.thumbnailGenerationId++;
    this.revokePageUrls(this.pages());
  }
  // Page Selection
  
  togglePage(pageNumber: number): void {
    this.pages.update(currentPages =>
      currentPages.map(page =>
        page.pageNumber === pageNumber
          ? {
              ...page,
              selected: !page.selected
            }
          : page
      )
    );
  }

  selectAll(): void {
    this.pages.update(currentPages =>
      currentPages.map(page => ({
        ...page,
        selected: true
      }))
    );
  }


  clearAll(): void {
    this.pages.update(currentPages =>
      currentPages.map(page => ({
        ...page,
        selected: false
      }))
    );
  }

  // Upload Full PDF + Process Selected Pages
  
  uploadAndProcess(): void {
    const file = this.selectedFile();
    if (!file) {
      return;
    }
    // Get selected page numbers
    const selectedPages = this.pages()
      .filter(page => page.selected)
      .map(page => page.pageNumber);
    if (selectedPages.length === 0) {
      alert(
        'Please select at least one page.'
      );
      return;
    }
    // Source name is required
    const sourceName = this.sourceName().trim();
    if (!sourceName) {
      alert(
        'Please enter a source name.'
      );
      return;
    }
    // Prevent duplicate requests
    if (
      this.uploading() ||
      this.processing()
    ) {
      return;
    }

    
    // Start Upload

    this.uploading.set(true);
    this.brochureImportService
      .uploadBrochure(
        file,
        sourceName,
        this.sourceUrl().trim() || undefined,
        this.publisher().trim() || undefined,
        this.publishedDate() || undefined,
        this.notes().trim() || undefined
      )
      .subscribe({
        // Upload Success
        next: response => {
          console.log(
            'Brochure uploaded:',
            response
          );
          this.importDocumentId.set(
            response.importDocumentId
          );
          this.uploading.set(false);
          this.processing.set(true);
          // Process only the selected pages
          this.processSelectedPages(
            response.importDocumentId,
            response.fileName,
            selectedPages
          );
        },
        // Upload Error
        error: error => {
          console.error(
            'Brochure upload failed:',
            error
          );
          this.uploading.set(false);
          this.processing.set(false);
          alert(
            error?.error?.message ??
            'Upload failed. Please try again.'
          );
        }
      });
  }
  // Process Selected Pages
  private processSelectedPages(
    importDocumentId: number,
    fileName: string,
    selectedPages: number[]
  ): void {
    this.brochureImportService
      .selectPages(
        importDocumentId,
        fileName,
        selectedPages
      )
      .subscribe({
        // Processing Success
        next: response => {
          console.log(
            'Selected pages processed:',
            response
          );
          this.processing.set(false);
        },

        // Processing Error
        error: error => {
          console.error(
            'Page processing failed:',
            error
          );
          this.processing.set(false);
          alert(
            'Page processing failed. Please try again.'
          );
        }
      });
  }
}