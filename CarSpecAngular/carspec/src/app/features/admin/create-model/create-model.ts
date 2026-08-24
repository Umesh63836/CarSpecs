import { Component, inject, OnInit, signal, Output, EventEmitter } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateServices } from '../../../core/services/AdminServices/create-services';
import { Brand } from '../../../core/services/brand/brand';
import { IBrand } from '../../../core/models/interfaces/brand';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-model',
  imports: [ReactiveFormsModule],
  templateUrl: './create-model.html',
  styleUrl: './create-model.css',
})
export class CreateModel implements OnInit{

  private fb = inject(FormBuilder);
  private createService = inject(CreateServices);
  private brandService = inject(Brand);

  @Output() close = new EventEmitter<void>();
  @Output() alert = new EventEmitter<{ type: 'success' | 'error'; title: string; message: string }>();

  showModal : boolean = true;

  showSuccessAlert = false;
  showErrorAlert = false;

  errorMessage = '';
   private router = inject(Router);

  brands = signal<IBrand[]>([]);

  modelForm = this.fb.group({
    modelName: ['', Validators.required],
    brandId: [null as number | null, Validators.required],
    isActive: [true],
    launchYear: [
      null as number | null,
      [Validators.required, Validators.min(1900)]
    ],
    discontinuedYear: [null as number | null],
    modelImageUrl: ['']
  });

  ngOnInit() {
    this.loadBrands();
    this.showModal = true;
  }

  loadBrands() {
    this.brandService.getBrands().subscribe((result : IBrand[]) => {
      this.brands.set(result);
      });
  }

createModel() {
      if (this.modelForm.invalid) {
      this.modelForm.markAllAsTouched();
      return;
  }

  const dto = {
    modelName: this.modelForm.value.modelName!,
    brandId: this.modelForm.value.brandId!,
    isActive: this.modelForm.value.isActive ?? true,
    launchYear: this.modelForm.value.launchYear!,
    discontinuedYear:
      this.modelForm.value.discontinuedYear ?? null,
    modelImageUrl:
      this.modelForm.value.modelImageUrl || null
  };

  this.createService.createModel(dto).subscribe({

  next: result => {

    console.log('Model created:', result);

    this.alert.emit({ type: 'success', title: 'Model created successfully', message: 'The new car model has been added successfully.' });
    this.showErrorAlert = false;

    this.showModal = false;

    this.modelForm.reset({
      isActive: true
    });

  },

  error: error => {

    console.error('Failed to create model:', error);

    this.showSuccessAlert = false;
    this.alert.emit({ type: 'error', title: 'Failed to create model', message: this.errorMessage });

    this.showModal = false;

    this.errorMessage =
      error?.error?.message ||
      error?.error?.title ||
      error?.message ||
      'Something went wrong while creating the model.';

  }

});}

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
