import { Component, inject, signal } from '@angular/core';
import { NavigationCancel, NavigationEnd, NavigationError, NavigationStart, RouterOutlet } from '@angular/router';
import { Navbar } from "./shared/navbar/navbar";
import { Footer } from "./shared/footer/footer";
import { LoadingService } from './core/services/loading/loading';
import { Loading } from "./shared/loading/loading";
import { Router } from '@angular/router';
import { Error } from './core/services/error/error';
import { Chatbot } from './shared/chatbot/chatbot';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, Footer, Loading, Chatbot],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('carspec');
   loadingService = inject(LoadingService);
   errorService = inject(Error);
   private router = inject(Router);

   constructor() {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.loadingService.show();
      }
      if (event instanceof NavigationEnd || event instanceof NavigationCancel || event instanceof NavigationError) {
        this.loadingService.hide();
      }
    });
  }
}
