import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CreateBrand } from './create-brand/create-brand';
import { CreateModel } from './create-model/create-model';
import { CreateVariant } from './create-variant/create-variant';
import { CreateEngine } from './create-engine/create-engine';
import { CreateTransmission } from './create-transmission/create-transmission';
import { CreateDrivetrain } from './create-drivetrain/create-drivetrain';
import { CreateFuelType } from './create-fuel-type/create-fuel-type';

type AdminAlert = {
  type: 'success' | 'error';
  title: string;
  message: string;
};

@Component({
  selector: 'app-create-vehicle',
  standalone: true,

  imports: [
    CreateBrand,
    CreateModel,
    CreateVariant,
    CreateEngine,
    CreateTransmission,
    CreateDrivetrain,
    CreateFuelType
  ],

  templateUrl: './create-vehicle.html',
  styleUrl: './create-vehicle.css'
})
export class CreateVehicle {

  private router = inject(Router);

  alert: AdminAlert | null = null;

  showBrandModal = signal(false);
  showModelModal = signal(false);
  showVariantModal = signal(false);
  showEngineModal = signal(false);
  showTransmissionModal = signal(false);
  showDrivetrainModal = signal(false);
  showFuelTypeModal = signal(false);


  // =========================================================
  // BRAND
  // =========================================================

  openBrandModal(): void {

    this.closeAllModals();

    this.alert = null;

    this.showBrandModal.set(true);
  }


  // =========================================================
  // MODEL
  // =========================================================

  openModelModal(): void {

    this.closeAllModals();

    this.alert = null;

    this.showModelModal.set(true);
  }


  // =========================================================
  // VARIANT
  // =========================================================

  openVariantModal(): void {

    this.closeAllModals();

    this.alert = null;

    this.showVariantModal.set(true);
  }


  // =========================================================
  // ENGINE
  // =========================================================

  openEngineModal(): void {

    this.closeAllModals();

    this.alert = null;

    this.showEngineModal.set(true);
  }


  // =========================================================
  // TRANSMISSION
  // =========================================================

  openTransmissionModal(): void {

    this.closeAllModals();

    this.alert = null;

    this.showTransmissionModal.set(true);
  }


  // =========================================================
  // DRIVETRAIN
  // =========================================================

  openDrivetrainModal(): void {

    this.closeAllModals();

    this.alert = null;

    this.showDrivetrainModal.set(true);
  }


  // =========================================================
  // FUEL TYPE
  // =========================================================

  openFuelTypeModal(): void {

    this.closeAllModals();

    this.alert = null;

    this.showFuelTypeModal.set(true);
  }


  // =========================================================
  // ALERT
  // =========================================================

  dismissAlert(): void {

    this.alert = null;

  }


  // =========================================================
  // CLOSE ALL MODALS
  // =========================================================

  closeAllModals(): void {

    this.showBrandModal.set(false);

    this.showModelModal.set(false);

    this.showVariantModal.set(false);

    this.showEngineModal.set(false);

    this.showTransmissionModal.set(false);

    this.showDrivetrainModal.set(false);

    this.showFuelTypeModal.set(false);

  }

  viewAllBrands(): void {
  this.router.navigate(['/brands']);
  }
}