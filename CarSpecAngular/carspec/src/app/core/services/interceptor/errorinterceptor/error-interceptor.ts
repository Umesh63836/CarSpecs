import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Error } from '../../error/error';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  const errorService = inject(Error);
  
  if (req.url.includes('/search')) {
    return next(req);
  }

  return next(req).pipe(

    catchError(error => {

      let message = 'Something went wrong. Please try again.';

      switch (error.status) {

        case 0:
          message = 'Unable to connect to the server. Please check your connection or try again later.';
          break;

        case 400:
          message = 'Invalid request.';
          break;

        case 401:
          message = 'Unauthorized. Login again!';
          break;

        case 403:
          message = 'You do not have permission to perform this action.';
          break;

        case 404:
          message = 'The requested resource was not found.';
          break;

        case 408:
          message = 'The request took too long. Please try again.';
          break;

        case 429:
          message = 'Too many requests. Please wait a moment and try again.';
          break;

        case 500:
          message = 'Something went wrong on the server.';
          break;

        case 502:
          message = 'The server is temporarily unavailable.';
          break;

        case 503:
          message = 'The API service is currently unavailable. Please try again later.';
          break;

        case 504:
          message = 'The server took too long to respond.';
          break;
      }

      errorService.showError(error.status, message);

      // Re-throw the error so the individual component can handle
      return throwError(() => error);
    })

  );
};