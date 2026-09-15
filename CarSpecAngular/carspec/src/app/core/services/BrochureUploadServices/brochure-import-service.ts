import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { BrochureUploadResponse } from '../../models/interfaces/DataProcessingDtos/brochure-upload-response';

@Service()
export class BrochureImportService {
  private http = inject(HttpClient);

  private apiUrl = environment.apiUrl;

  uploadBrochure(file: File, sourceName: string, sourceUrl?: string, publisher?: string, publishedDate?: string, notes?: string): Observable<BrochureUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('sourceName', sourceName);
    if (sourceUrl)
      formData.append('sourceUrl', sourceUrl);
    if (publisher) 
      formData.append('publisher', publisher);
    if (publishedDate) 
      formData.append('publishedDate', publishedDate);
    if (notes) 
      formData.append('notes', notes);

    return this.http.post<BrochureUploadResponse>(this.apiUrl + "/import/brochure", formData
    );
  }

  selectPages(importDocumentId: number, fileName: string, selectedPages: number[]): Observable<any> {
    return this.http.post(this.apiUrl + "/import/brochurePages", {importDocumentId, fileName, selectedPages}
    );
  }
}
