import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateBrand } from "../create-brand/create-brand";
import { CreateModel } from "../create-model/create-model";
import { CreateVariant } from "../create-variant/create-variant";
import { CreateEngine } from "../create-engine/create-engine";
import { CreateTransmission } from "../create-transmission/create-transmission";
import { CreateDrivetrain } from "../create-drivetrain/create-drivetrain";

@Component({
  selector: 'app-admin-dashboard',
  imports: [FormsModule, CreateBrand, CreateModel, CreateVariant, CreateEngine, CreateTransmission, CreateDrivetrain],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard {
 showBrandModal = signal(false);
  showModelModal = signal(false);
  showVariantModal = signal(false);
  showEngineModal = signal(false);
  showTransmissionModal = signal(false);
  showDrivetrainModal = signal(false);


  openBrandModal() {
    this.closeAllModals();
    this.showBrandModal.set(true);
  }

  openModelModal() {
    this.closeAllModals();
    this.showModelModal.set(true);
  }

  openVariantModal() {
    this.closeAllModals();
    this.showVariantModal.set(true);
  }

  openEngineModal() {
    this.closeAllModals();
    this.showEngineModal.set(true);
  }

  openTransmissionModal() {
    this.closeAllModals();
    this.showTransmissionModal.set(true);
  }

  openDrivetrainModal() {
    this.closeAllModals();
    this.showDrivetrainModal.set(true);
  }

  closeAllModals() {
    this.showBrandModal.set(false);
    this.showModelModal.set(false);
    this.showVariantModal.set(false);
    this.showEngineModal.set(false);
    this.showTransmissionModal.set(false);
    this.showDrivetrainModal.set(false);
  }

}
