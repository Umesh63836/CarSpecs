import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from '../../../../environments/environment';
import { IChatResponse } from '../../models/interfaces/ichat-response';

@Service()
export class Chatservice {
    private http = inject(HttpClient);

    private apiUrl: string = environment.apiUrl;

    sendMessage(message: string): Observable<IChatResponse> {
        return this.http.post<IChatResponse>( this.apiUrl + "/chat", {message: message} );
    }
}
