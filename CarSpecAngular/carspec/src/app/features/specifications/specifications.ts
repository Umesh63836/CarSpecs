import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { Specs } from '../../core/services/specification/specs';
import { ActivatedRoute } from '@angular/router';
import { ISpecs } from '../../core/models/interfaces/specs';
import { OnRoadPrice } from '../../core/services/onRoadPrice/on-road-price';
import { IOnRoadPriceDto } from '../../core/models/interfaces/OnRoadPriceDtos/on-road-price';
import { LocationsearchResponse } from '../../core/models/interfaces/location-search-response';
import { UsersState } from '../../core/services/usersState/users-state';
import { Location } from '../../core/services/location/location';
import { CommonModule, DecimalPipe } from '@angular/common';
import { LocationSelector } from '../location-selector/location-selector';

@Component({
  selector: 'app-specifications',
  imports: [CommonModule, LocationSelector, DecimalPipe],
  templateUrl: './specifications.html',
  styleUrl: './specifications.css',
})
export class Specifications implements OnInit{
  specsService = inject(Specs);
  private route = inject(ActivatedRoute);
  onRoadPriceService = inject(OnRoadPrice);
  userLocationService = inject(UsersState);
  locationService = inject(Location);

  variantId!: number;

  expandedSpecification = signal<string | null>('engine');
  onRoadPrice = signal<IOnRoadPriceDto | null>(null);
  selectedLocation = this.userLocationService.selectedLocation;
  isLoadingOnRoadPrice = signal(false);
  showPriceBreakup = signal(false);
  specifications = signal<ISpecs | null>(null);
  activePriceInfo = signal<string | null>(null);
  activeChargeInfo = signal<'exShowroom' | 'rto' | 'insurance' | 'tcs' | 'fastag' | null>(null);
  activeInfo = signal<string | null>(null);

  constructor() {
    effect(() => {const location = this.userLocationService.selectedLocation();
      if (!location) 
        return;
      if (!this.variantId) 
        return;
      this.loadOnRoadPrice(location.stateCode);
    });
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((paramMap) => {
      this.variantId = Number(paramMap.get('variantId'));
      this.specsService.getSpecs(this.variantId).subscribe({
        next: (result: ISpecs) => this.specifications.set(result),
        error: (error) => console.error('Error loading specifications:', error)
      });
    });
  }

  toggleSpecification(section: string): void {
  this.expandedSpecification.update(
    current => current === section ? null : section
  );
  }

  toggleInfo(info: string): void {
  this.activeInfo.update(current =>
    current === info ? null : info
  );
  }

  togglePriceInfo(type: string): void {
  this.activePriceInfo.update(current =>
    current === type ? null : type
  );
  }

  toggleChargeInfo(
  type: 'exShowroom' | 'rto' | 'insurance' | 'tcs' | 'fastag'
  ) {
  this.activeChargeInfo.update(current =>
    current === type ? null : type
  );
  }

  // OPEN LOCATION POPUP
  openLocationSearch(): void {
    this.locationService.openLocationSearch.set(true);
  }
  
  // CALL API
  private loadOnRoadPrice(stateCode: number): void {
    this.isLoadingOnRoadPrice.set(true);
    this.onRoadPriceService.getOnRoadPrice(this.variantId, stateCode).subscribe({ next: (result) => {
          this.onRoadPrice.set(result);
          this.isLoadingOnRoadPrice.set(false);
        },
        error: (error) => {
          console.error(
            'Error calculating on-road price:',
            error
          );
          this.isLoadingOnRoadPrice.set(false);
        }
      });
  }

  // PRICE BREAKUP
  togglePriceBreakup(): void {
    this.showPriceBreakup.update( value => !value
    );
  }

  closePriceBreakup(): void {
    this.showPriceBreakup.set(false);
    this.activeChargeInfo.set(null);
  }

  closeInfo(): void {
  this.activeInfo.set(null);
  }
}
