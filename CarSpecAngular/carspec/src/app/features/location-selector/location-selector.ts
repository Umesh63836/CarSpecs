import { Component, inject, signal } from '@angular/core';
import { UsersState } from '../../core/services/usersState/users-state';
import { LocationsearchResponse } from '../../core/models/interfaces/location-search-response';
import { Subject} from 'rxjs';
import { Location } from '../../core/services/location/location';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-location-selector',
  imports: [FormsModule],
  templateUrl: './location-selector.html',
  styleUrl: './location-selector.css',
})
export class LocationSelector {
  locationService = inject(Location);
  private locationState = inject(UsersState);

  locationterm = signal<string>("");
  locations = signal<LocationsearchResponse[]>([]);
  private searchSubject = new Subject<string>();
  isSearching = signal(false);

  onLocationSearch(){
  if (this.locationterm().trim().length < 2) {
    this.locations.set([]);
    return;
  }
  this.locationService.searchLocations(this.locationterm(), 10).subscribe({next: results => { 
    this.locations.set(results);
      },
      error: err => {
        console.error(err);
      }
    });
 }

  close() {
    this.locationService.openLocationSearch.set(false);
  }

  clearSearch() {
    this.locationterm.set('');
    this.locations.set([]);
  }

  selectPopularCity(city: string) {
    this.locationterm.set(city);
    this.onLocationSearch();
  }


  selectLocation(location: LocationsearchResponse) {
    this.locationState.setLocation(location);
    this.locationterm.set(location.name);
    this.locations.set([]);
    this.locationService.openLocationSearch.set(false);
  }
}
