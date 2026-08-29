import { Component, inject, Pipe, signal } from '@angular/core';
import { Filterservice } from '../../../core/services/FilterServices/filterservice';
import { CarFilterResponse } from '../../../core/models/interfaces/filterResponse/car-filter-response';
import { CarFilterRequest } from '../../../core/models/models/carFilterRequest';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-show-filter',
  imports: [FormsModule],
  templateUrl: './show-filter.html',
  styleUrl: './show-filter.css',
})
export class ShowFilter {

  private router = inject(Router);

  filters: CarFilterRequest = {};
  showFilterPopup = false;

  openFilterPopup(): void {
    this.showFilterPopup = true;
  }


  closeFilterPopup(): void {
    this.showFilterPopup = false;
  }


  applyManualFilters(): void {

    this.showFilterPopup = false;

    this.navigateToResults(this.filters);
  }


  applyQuickFilter(filter: CarFilterRequest): void {
    this.filters = {
      ...filter};
    this.navigateToResults(this.filters);
  }

  clearFilters(): void {
    this.filters = {};
  }

  private navigateToResults(filters: CarFilterRequest): void {
    this.router.navigate(['/cars'],{ queryParams: filters }
    );

  }
}
