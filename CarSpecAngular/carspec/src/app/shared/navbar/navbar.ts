import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { Search } from '../../core/services/search/search';
import { FormsModule } from '@angular/forms';
import { ISearch } from '../../core/models/interfaces/search';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/AdminServices/AuthService/authService';

@Component({
  selector: 'app-navbar',
  imports: [FormsModule, RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar implements OnInit{
 private router = inject(Router);
 private searchService = inject(Search);
 private authService = inject(AuthService);

 isAdminLoggedIn = signal<boolean>(false);

 searchText = signal<string>("");

 searchResults = signal<ISearch[]>([]);

 ngOnInit(): void {
   this.isAdminLoggedIn = this.authService.isloggedIn;
 }

 onSearch(){
  if (this.searchText().trim().length < 2) {
    this.searchResults.set([]);
    return;
  }

  this.searchService
    .search(this.searchText())
    .subscribe({
      next: results => {
        this.searchResults.set(results);
      },
      error: err => {
        console.error(err);
      }
    });
 }

 onSearchResultClick(result: ISearch){
  if (result.resultType === 'Model') {

    this.router.navigate(['/variants', result.id]);
    console.log(result.id)

  }
  else if (result.resultType === 'Variant') {

    this.router.navigate(['/specifications', result.id]);

  }

  // Hide dropdown after selection
  this.searchResults.set([]);

  // Clear search box
  this.searchText.set("");
 }

 routeToDashboard(){
  this.router.navigate(['/admindashboard']);
 }

 logout() {

  const refreshToken = this.authService.getRefreshToken();

  if (refreshToken) {

    this.authService.logout(refreshToken).subscribe({
      next: () => {
        this.authService.clearTokens();
        this.router.navigate(['/']);
      },
      error: () => {
        this.authService.clearTokens();
        this.router.navigate(['/']);
      }
    });

  } else {

    this.authService.clearTokens();
    this.router.navigate(['/']);

  }
  this.authService.isloggedIn.set(false);
}

}
