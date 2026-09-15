import { Component, inject, signal } from '@angular/core';
import { BrochureImportService } from '../../../core/services/BrochureUploadServices/brochure-import-service';
import { BrochureUploadResponse } from '../../../core/models/interfaces/DataProcessingDtos/brochure-upload-response';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-uploadbrochure',
  imports: [FormsModule],
  templateUrl: './uploadbrochure.html',
  styleUrl: './uploadbrochure.css',
})
export class Uploadbrochure {
  private brochureService = inject(BrochureImportService);

  selectedFile = signal<File | null>(null);
  uploading = signal(false);
  result = signal<BrochureUploadResponse | null>(null);
  error = signal<string | null>(null);

  sourceName = '';
  sourceUrl = '';
  publisher = '';
  publishedDate = '';
  notes = '';

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file =input.files?.[0];
    if (!file) 
      return;
    if (file.type !== 'application/pdf') {
      this.error.set('Please select a PDF brochure.'
      );
      return;
    }
    if (file.size > 25 * 1024 * 1024) {
      this.error.set('Maximum brochure size allowed is 25 MB.'
      );
      return;
    }
    this.error.set(null);
    this.selectedFile.set(file);
  }


  upload(): void {
    const file =this.selectedFile();
    if (!file) {
      this.error.set('Please select a brochure.'
      );
      return;
    }
    if (!this.sourceName.trim()) {
      this.error.set('Source name is required.'
      );
      return;
    }

    this.uploading.set(true);
    this.error.set(null);
    this.result.set(null);
    this.brochureService.uploadBrochure(file, this.sourceName, this.sourceUrl || undefined, this.publisher || undefined, this.publishedDate || undefined, this.notes || undefined)
      .subscribe({next: response => {
          this.result.set(response);
          this.uploading.set(false);
        },
        error: error => {
          this.error.set(
            error?.error?.message ??
            'Brochure upload failed.'
          );
          this.uploading.set(false);
        }
      });
  }
}
