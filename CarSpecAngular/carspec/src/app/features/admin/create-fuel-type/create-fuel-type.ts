import { Component, EventEmitter, Output, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateServices } from '../../../core/services/AdminServices/create-services';

@Component({
  selector: 'app-create-fuel-type',
  imports: [ReactiveFormsModule],
  templateUrl: './create-fuel-type.html',
  styleUrl: './create-fuel-type.css',
})
export class CreateFuelType {
  private fb = inject(FormBuilder);
  private createService = inject(CreateServices);

  @Output() close = new EventEmitter<void>();
  @Output() alert = new EventEmitter<{ type: 'success' | 'error'; title: string; message: string }>();

  showModal = true;

  fuelTypeForm = this.fb.nonNullable.group({
    fuelType: ['', Validators.required]
  });

  createFuelType() {
    if (this.fuelTypeForm.invalid) {
      this.fuelTypeForm.markAllAsTouched();
      return;
    }

    this.createService.createFuelType(this.fuelTypeForm.getRawValue()).subscribe({
      next: () => {
        this.alert.emit({
          type: 'success',
          title: 'Fuel type created successfully',
          message: 'The new fuel type has been added successfully.'
        });
        this.showModal = false;
        this.fuelTypeForm.reset();
      },
      error: error => {
        this.alert.emit({
          type: 'error',
          title: 'Failed to create fuel type',
          message: error?.error?.message || error?.error?.title || error?.message || 'Something went wrong while creating the fuel type.'
        });
        this.showModal = false;
      }
    });
  }

  closeModal() {
    this.showModal = false;
    this.close.emit();
  }
}
