import { computed, Service, signal } from '@angular/core';
import { LocationsearchResponse } from '../../models/interfaces/location-search-response';

@Service()
export class UsersState {
    private selectedLocationSignal = signal<LocationsearchResponse | null>(null);

    selectedLocation = this.selectedLocationSignal.asReadonly();

    stateCode = computed(() => this.selectedLocationSignal()?.stateCode ?? null
    );

    stateName = computed(() => this.selectedLocationSignal()?.stateName ?? null
    );

    setLocation(location: LocationsearchResponse) { 
        this.selectedLocationSignal.set(location);
    }

    clearLocation() { this.selectedLocationSignal.set(null);
    }
}
