import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';

import { Filterservice } from '../../../core/services/FilterServices/filterservice';
import { CarFilterResponse } from '../../../core/models/interfaces/filterResponse/car-filter-response';
import { CarFilterRequest } from '../../../core/models/models/carFilterRequest';
import { CarModelFilterResponse } from '../../../core/models/interfaces/filterResponse/car-model-filter-response';

@Component({
  selector: 'app-filter-cars',
  imports: [DecimalPipe, RouterLink],
  templateUrl: './filter-cars.html',
  styleUrl: './filter-cars.css',
})
export class FilterCars implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private filterService = inject(Filterservice);

  results: CarFilterResponse = {
    totalModels: 0,
    totalVariants: 0,
    models: []
  };

  // Currently selected filters
  filters = signal<CarFilterRequest>({});

  currentVariantIndex = signal<Record<number, number>>({});

  expandedModelId = signal<number | null>(null);

  isLoading = signal<boolean>(false);

  // ---------------------------------------------------------
  // INIT
  // ---------------------------------------------------------

  ngOnInit(): void {

    this.route.queryParams.subscribe(params => {

      const filters = this.createFiltersFromQueryParams(params);

      // IMPORTANT:
      // Set filters BEFORE calling API.
      // Therefore filters remain visible even when API returns 0 results.
      this.filters.set(filters);

      console.log('Filters from URL:', filters);

      this.searchCars(filters);
    });
  }

  // ---------------------------------------------------------
  // CREATE FILTER OBJECT FROM URL
  // ---------------------------------------------------------

  private createFiltersFromQueryParams(params: any): CarFilterRequest {

    const filters: CarFilterRequest = {};

    if (params['brand']) {
      filters.brand = params['brand'];
    }

    if (params['model']) {
      filters.model = params['model'];
    }

    if (params['minPrice'] !== undefined && params['minPrice'] !== '') {
      filters.minPrice = Number(params['minPrice']);
    }

    if (params['maxPrice'] !== undefined && params['maxPrice'] !== '') {
      filters.maxPrice = Number(params['maxPrice']);
    }

    if (params['minPower'] !== undefined && params['minPower'] !== '') {
      filters.minPower = Number(params['minPower']);
    }

    if (params['maxPower'] !== undefined && params['maxPower'] !== '') {
      filters.maxPower = Number(params['maxPower']);
    }

    if (params['minTorque'] !== undefined && params['minTorque'] !== '') {
      filters.minTorque = Number(params['minTorque']);
    }

    if (params['maxTorque'] !== undefined && params['maxTorque'] !== '') {
      filters.maxTorque = Number(params['maxTorque']);
    }

    if (params['displacement'] !== undefined && params['displacement'] !== '') {
      filters.displacement = Number(params['displacement']);
    }

    if (params['isTurbocharged'] !== undefined && params['isTurbocharged'] !== '') {
      filters.isTurbocharged =
        params['isTurbocharged'] === 'true';
    }

    if (params['emissionStandard']) {
      filters.emissionStandard = params['emissionStandard'];
    }

    if (params['transmissionType']) {
      filters.transmissionType = params['transmissionType'];
    }

    if (
      params['numberOfGears'] !== undefined &&
      params['numberOfGears'] !== ''
    ) {
      filters.numberOfGears = Number(params['numberOfGears']);
    }

    if (params['drivetrainType']) {
      filters.drivetrainType = params['drivetrainType'];
    }

    if (params['fuelType']) {
      filters.fuelType = params['fuelType'];
    }

    return filters;
  }

  // ---------------------------------------------------------
  // SEARCH CARS
  // ---------------------------------------------------------

  private searchCars(filters: CarFilterRequest): void {

    this.isLoading.set(true);

    this.filterService.FilterCars(filters).subscribe({

      next: response => {

        this.results = response;

        this.isLoading.set(false);

        console.log('Filter response:', response);
        console.log('Results:', this.results);
      },

      error: error => {

        console.error('Filter API error:', error);

        // IMPORTANT:
        // Do NOT clear filters here.
        // Selected filters should remain visible even if
        // the API returns an error/no result.
        this.isLoading.set(false);

        this.results = {
          totalModels: 0,
          totalVariants: 0,
          models: []
        };
      }
    });
  }

  // ---------------------------------------------------------
  // CHECK WHETHER FILTERS ARE APPLIED
  // ---------------------------------------------------------

  hasAppliedFilters(): boolean {

    const f = this.filters();

    return Object.values(f).some(
      value =>
        value !== undefined &&
        value !== null &&
        value !== ''
    );
  }

  // ---------------------------------------------------------
  // APPLY FILTER
  // ---------------------------------------------------------

  applyFilter(filter: Partial<CarFilterRequest>): void {

    const updatedFilters: CarFilterRequest = {
      ...this.filters(),
      ...filter
    };

    // Update local filter state immediately
    this.filters.set(updatedFilters);

    // Update URL and search
    this.updateUrlAndSearch(updatedFilters);
  }

  // ---------------------------------------------------------
  // UPDATE URL + SEARCH
  // ---------------------------------------------------------

  updateUrlAndSearch(filters: CarFilterRequest): void {

    const queryParams: Record<string, any> = {};

    Object.entries(filters).forEach(([key, value]) => {

      if (
        value !== undefined &&
        value !== null &&
        value !== ''
      ) {
        queryParams[key] = value;
      }

    });

    // Update URL.
    //
    // queryParamsHandling: ''
    // means replace the existing query parameters
    // with the current filters.
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: ''
    });

    // Search immediately.
    this.searchCars(filters);
  }

  // ---------------------------------------------------------
  // REMOVE FILTER
  // ---------------------------------------------------------

  removeFilter(key: keyof CarFilterRequest): void {

    const updatedFilters: CarFilterRequest = {
      ...this.filters()
    };

    delete updatedFilters[key];

    // Update selected filters immediately
    this.filters.set(updatedFilters);

    // Update URL and search
    this.updateUrlAndSearch(updatedFilters);
  }

  // ---------------------------------------------------------
  // CHECK MODEL VARIANTS
  // ---------------------------------------------------------

  toggleMatchingVariants(modelId: number): void {

    if (this.expandedModelId() === modelId) {

      this.expandedModelId.set(null);

    } else {

      this.expandedModelId.set(modelId);
    }
  }

  // ---------------------------------------------------------
  // MIN PRICE
  // ---------------------------------------------------------

  getMinPrice(car: CarModelFilterResponse): number {

    if (!car.variants?.length) {
      return 0;
    }

    return Math.min(
      ...car.variants.map(v => v.exShowroomPrice)
    );
  }

  // ---------------------------------------------------------
  // MAX PRICE
  // ---------------------------------------------------------

  getMaxPrice(car: CarModelFilterResponse): number {

    if (!car.variants?.length) {
      return 0;
    }

    return Math.max(
      ...car.variants.map(v => v.exShowroomPrice)
    );
  }

  // ---------------------------------------------------------
  // NEXT VARIANT
  // ---------------------------------------------------------

  nextVariant(
    modelId: number,
    totalVariants: number
  ): void {

    if (totalVariants <= 0) {
      return;
    }

    this.currentVariantIndex.update(current => {

      const currentIndex = current[modelId] ?? 0;

      return {
        ...current,
        [modelId]:
          (currentIndex + 1) % totalVariants
      };
    });
  }

  // ---------------------------------------------------------
  // PREVIOUS VARIANT
  // ---------------------------------------------------------

  previousVariant(
    modelId: number,
    totalVariants: number
  ): void {

    if (totalVariants <= 0) {
      return;
    }

    this.currentVariantIndex.update(current => {

      const currentIndex = current[modelId] ?? 0;

      return {
        ...current,
        [modelId]:
          currentIndex === 0
            ? totalVariants - 1
            : currentIndex - 1
      };
    });
  }
}
