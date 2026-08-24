import { Component, inject, Output, EventEmitter } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateServices } from '../../../core/services/AdminServices/create-services';
import { Router } from '@angular/router';
import { CreateDrivetrainDto } from '../../../core/models/models/drivetrainDto';

@Component({
  selector: 'app-create-drivetrain',
  imports: [ReactiveFormsModule],
  templateUrl: './create-drivetrain.html',
  styleUrl: './create-drivetrain.css',
})
export class CreateDrivetrain {
  private fb = inject(FormBuilder);
  private createService = inject(CreateServices);
  private router = inject(Router);

  @Output() close = new EventEmitter<void>();
  @Output() alert = new EventEmitter<{ type: 'success' | 'error'; title: string; message: string }>();

  drivetrainForm = this.fb.nonNullable.group({
    drivetrainName: ['', Validators.required]
  });

  showModal = true;

  showSuccessAlert = false;
  showErrorAlert = false;

  errorMessage = '';


  createdrivetrain() {

    if (this.drivetrainForm.invalid) 
    {
      this.drivetrainForm.markAllAsTouched();
      return;
    }

    const dto = new CreateDrivetrainDto(
      this.drivetrainForm.value.drivetrainName!
    );

    this.createService.createDrivetrain(dto).subscribe({next: response => {
    // Show success alert
      this.alert.emit({ type: 'success', title: 'Drivetrain created successfully', message: 'The new drivetrain has been added successfully.' });
      this.showModal =false;

      // Hide error alert
      this.showErrorAlert = false;
    this.drivetrainForm.reset();
    },
    error: error => {
     this.showSuccessAlert = false;

      // Show error alert
      this.alert.emit({ type: 'error', title: 'Failed to create drivetrain', message: this.errorMessage });
      this.showModal =false;

      // Try to get useful error message from API
      this.errorMessage =
        error?.error?.message ||
        error?.error?.title ||
        error?.message ||
        'Something went wrong while creating the drivetrain.';

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

