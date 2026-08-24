import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CreateBrand } from "../create-brand/create-brand";
import { CreateModel } from "../create-model/create-model";
import { CreateVariant } from "../create-variant/create-variant";
import { CreateEngine } from "../create-engine/create-engine";
import { CreateTransmission } from "../create-transmission/create-transmission";
import { CreateDrivetrain } from "../create-drivetrain/create-drivetrain";
import { CreateFuelType } from "../create-fuel-type/create-fuel-type";

type AdminAlert = {
  type: 'success' | 'error';
  title: string;
  message: string;
};

@Component({
  selector: 'app-admin-dashboard',
  imports: [FormsModule, CreateBrand, CreateModel, CreateVariant, CreateEngine, CreateTransmission, CreateDrivetrain, CreateFuelType],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard {
 private router = inject(Router);
 alert: AdminAlert | null = null;
 showBrandModal = signal(false);
  showModelModal = signal(false);
  showVariantModal = signal(false);
  showEngineModal = signal(false);
  showTransmissionModal = signal(false);
  showDrivetrainModal = signal(false);
  showFuelTypeModal = signal(false);


  openBrandModal() {
    this.closeAllModals();
    this.alert = null;
    this.showBrandModal.set(true);
  }

  openModelModal() {
    this.closeAllModals();
    this.alert = null;
    this.showModelModal.set(true);
  }

  openVariantModal() {
    this.closeAllModals();
    this.alert = null;
    this.showVariantModal.set(true);
  }

  openEngineModal() {
    this.closeAllModals();
    this.alert = null;
    this.showEngineModal.set(true);
  }

  openTransmissionModal() {
    this.closeAllModals();
    this.alert = null;
    this.showTransmissionModal.set(true);
  }

  openDrivetrainModal() {
    this.closeAllModals();
    this.alert = null;
    this.showDrivetrainModal.set(true);
  }

  openFuelTypeModal() {
    this.closeAllModals();
    this.alert = null;
    this.showFuelTypeModal.set(true);
  }

  dismissAlert() {
    this.alert = null;
  }

  viewAllBrands() {
    this.router.navigate(['/brands']);
  }

  closeAllModals() {
    this.showBrandModal.set(false);
    this.showModelModal.set(false);
    this.showVariantModal.set(false);
    this.showEngineModal.set(false);
    this.showTransmissionModal.set(false);
    this.showDrivetrainModal.set(false);
    this.showFuelTypeModal.set(false);
  }

}
