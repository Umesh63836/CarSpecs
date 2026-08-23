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

    login(loginDto: LoginDto): Observable<LoginResponseDto> {
        return this.http.post<LoginResponseDto>(this.apiUrl + "/Auth/login", loginDto)
    }

    refreshToken(refreshToken: string): Observable<LoginResponseDto>  {
        const param = new HttpParams().set('refreshtoken', refreshToken)
        return this.http.post<LoginResponseDto>(this.apiUrl + "/Auth/refresh", {param});
    }
  
    logout(refreshToken: string): Observable<LoginResponseDto>  {
        const param = new HttpParams().set('refreshToken', refreshToken)
        return this.http.post<LoginResponseDto>(this.apiUrl + "/Auth/logout", {param});
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
