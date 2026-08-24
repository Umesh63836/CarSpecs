import { Component, inject, Output, EventEmitter } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateServices } from '../../../core/services/AdminServices/create-services';
import { Router } from '@angular/router';
import { CreateTransmissionDto } from '../../../core/models/models/transmissionDto';

@Component({
  selector: 'app-create-transmission',
  imports: [ReactiveFormsModule],
  templateUrl: './create-transmission.html',
  styleUrl: './create-transmission.css',
})
export class CreateTransmission {

  private fb = inject(FormBuilder);
  private createService = inject(CreateServices);
  private router = inject(Router);

  @Output() close = new EventEmitter<void>();
  @Output() alert = new EventEmitter<{ type: 'success' | 'error'; title: string; message: string }>();

  transmissionForm = this.fb.nonNullable.group({
    transmissionName: ['', Validators.required],
    gears: [0]
  });

  showModal = true;

  showSuccessAlert = false;
  showErrorAlert = false;

  errorMessage = '';


  createTransmission() {

    if (this.transmissionForm.invalid) 
    {
      this.transmissionForm.markAllAsTouched();
      return;
    }

    const dto = new CreateTransmissionDto(
      this.transmissionForm.value.transmissionName!,
      this.transmissionForm.value.gears || 0,
    );

    this.createService.createTransmission(dto).subscribe({next: response => {
    // Show success alert
      this.alert.emit({ type: 'success', title: 'Transmission created successfully', message: 'The new transmission has been added successfully.' });
      this.showModal =false;

      // Hide error alert
      this.showErrorAlert = false;
    this.transmissionForm.reset();
    },
    error: error => {
     this.showSuccessAlert = false;

      // Show error alert
      this.alert.emit({ type: 'error', title: 'Failed to create transmission', message: this.errorMessage });
      this.showModal =false;

      // Try to get useful error message from API
      this.errorMessage =
        error?.error?.message ||
        error?.error?.title ||
        error?.message ||
        'Something went wrong while creating the transmission.';

    }
    });
  }

  dismissSuccess() {
    this.showSuccessAlert = false;
  }

  dismissError() {
    this.showErrorAlert = false;
  }

  closeModal() {
    this.showModal = false;
    this.close.emit();
  }
}

