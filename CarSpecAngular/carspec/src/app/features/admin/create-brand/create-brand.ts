import { Component, Output, EventEmitter, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateServices } from '../../../core/services/AdminServices/create-services';
import { Router, RouterEvent, RouterLink } from '@angular/router';

@Component({
  selector: 'app-create-brand',
  imports: [ReactiveFormsModule],
  templateUrl: './create-brand.html',
  styleUrl: './create-brand.css',
})
export class CreateBrand {
  private fb = inject(FormBuilder);
  private createService = inject(CreateServices);
  private router = inject(Router);

  @Output() close = new EventEmitter<void>();

  brandForm = this.fb.nonNullable.group({
    brandName: ['', Validators.required],
    logoUrl: ['']
  });

  showModal = true;

  showSuccessAlert = false;
  showErrorAlert = false;

  errorMessage = '';


  createBrand() {

    if (this.brandForm.invalid) 
    {
      this.brandForm.markAllAsTouched();
      return;
    }

    const dto = {brandName: this.brandForm.value.brandName!,logoUrl: this.brandForm.value.logoUrl || null};

    this.createService.createBrand(dto).subscribe({next: response => {
    // Show success alert
      this.showSuccessAlert = true;
      this.showModal =false;

      // Hide error alert
      this.showErrorAlert = false;
    this.brandForm.reset();
    },
    error: error => {
     this.showSuccessAlert = false;

      // Show error alert
      this.showErrorAlert = true;
      this.showModal =false;

      // Try to get useful error message from API
      this.errorMessage =
        error?.error?.message ||
        error?.error?.title ||
        error?.message ||
        'Something went wrong while creating the brand.';
    }
    });
  }

  dismissSuccess() {
    this.showSuccessAlert = false;
  }

  dismissError() {
  this.showErrorAlert = false;
}

  viewAllBrands() {
    this.router.navigate(['/brands']);
  }

  closeModal() {
    this.showModal = false;
    this.close.emit();
  }
}
