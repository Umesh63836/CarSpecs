import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from '../../loading/loading';
import { finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  if (req.url.includes('/search') || req.url.includes('/chat') || req.url.includes('/import/brochure') || req.url.includes('/import/brochurePages') 
      || req.url.includes('/batches') || req.url.includes('/admin') || req.url.includes('/admin-review') || req.url.includes('/import-batches') || req.url.includes('/import-batches/stage')) {
    return next(req);
  }

  loadingService.show();

  return next(req).pipe(
    finalize(() => {
      loadingService.hide();
    })
  );
};
