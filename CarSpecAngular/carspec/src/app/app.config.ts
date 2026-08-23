import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/services/interceptor/authinterceptor/auth-interceptor';
import { loadingInterceptor } from './core/services/interceptor/loadinginterceptor/loading-interceptor';
import { errorInterceptor } from './core/services/interceptor/errorinterceptor/error-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor,
        loadingInterceptor,errorInterceptor]))
  ]
};
