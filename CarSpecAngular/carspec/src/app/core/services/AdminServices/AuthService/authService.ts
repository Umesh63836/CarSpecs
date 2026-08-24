import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Service, signal } from '@angular/core';
import { LoginDto } from '../../../models/models/loginDto';
import { catchError, map, Observable, of } from 'rxjs';
import { LoginResponseDto } from '../../../models/models/loginResponseDto';
import { environment } from '../../../../../environments/environment';

@Service()
export class AuthService {
    private http = inject(HttpClient);
    private apiUrl = environment.apiUrl;

    isloggedIn = signal(!!localStorage.getItem('accessToken'));
    isloggedout = signal(false);

    showSuccess() {
    this.isloggedout.set(true);
    // Automatically remove after 5 seconds
    setTimeout(() => {
      this.isloggedout.set(false);
    }, 3000);
    }

    login(loginDto: LoginDto): Observable<LoginResponseDto> {
        return this.http.post<LoginResponseDto>(this.apiUrl + "/Auth/login", loginDto)
    }

    refreshToken(refreshToken: string): Observable<LoginResponseDto>  {
        const params = new HttpParams().set('refreshtoken', refreshToken)
        return this.http.post<LoginResponseDto>(this.apiUrl + "/Auth/refresh", {params});
    }
  
    logout(refreshToken: string): Observable<void>  {
        const params = new HttpParams().set('refreshToken', refreshToken)
        return this.http.post<void>(this.apiUrl + "/Auth/logout", null , {params} );
    }

    getAccessToken() {
        return localStorage.getItem('accessToken');
    }

    getRefreshToken() {
        return localStorage.getItem('refreshToken');
    }

    isLoggedIn() {
        return !!this.getAccessToken();
    }

    clearTokens() {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
    }
}
