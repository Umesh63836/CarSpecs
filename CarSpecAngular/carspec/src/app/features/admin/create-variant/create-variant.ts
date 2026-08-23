import { Component, inject, OnInit, signal, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateServices } from '../../../core/services/AdminServices/create-services';
import { Brand } from '../../../core/services/brand/brand';
import { Model } from '../../../core/services/model/model';
import { EngineDto } from '../../../core/models/models/engineDto';
import { TransmissionDto } from '../../../core/models/models/transmissionDto';
import { DrivetrainDto } from '../../../core/models/models/drivetrainDto';
import { VariantDto } from '../../../core/models/models/variantDto';
import { SelectEngineDto } from '../../../core/models/interfaces/selectDtos/select-engine-dto';
import { SelectTransmissionDto } from '../../../core/models/interfaces/selectDtos/select-transmission-dto';
import { SelectDrivetrainDto } from '../../../core/models/interfaces/selectDtos/select-drivetrain-dto';
import { IBrand } from '../../../core/models/interfaces/brand';
import { IModel } from '../../../core/models/interfaces/model';

@Component({
  selector: 'app-create-variant',
  imports: [ReactiveFormsModule,FormsModule],
  templateUrl: './create-variant.html',
  styleUrl: './create-variant.css',
})
export class CreateVariant implements OnInit {
  private createService = inject(CreateServices);
  private brandService = inject(Brand);
  private modelService = inject(Model);
  private fb = inject(FormBuilder);

  @Output() close = new EventEmitter<void>();

  showModal : boolean = true;
  showSuccessAlert = false;
  showErrorAlert = false;
  errorMessage = '';

  brands = signal<IBrand[]>([]);
  models = signal<IModel[]>([]);
  engines = signal<SelectEngineDto[]>([]);
  transmissions = signal<SelectTransmissionDto[]>([]);
  drivetrains = signal<SelectDrivetrainDto[]>([]);

  ngOnInit() {
    this.loadBrands();
    this.loadEngines();
    this.loadTransmissions();
    this.loadDrivetrains();

    // Subscribe to brand changes to load models
    this.variantForm.get('brandId')?.valueChanges.subscribe((brandId: number | null) => {
      if (brandId) {
        this.loadModels(brandId);
      }
    });
  }

  loadBrands() {
    this.brandService.getBrands().subscribe((result: IBrand[]) =>
      this.brands.set(result)
    );
  }

  loadModels(brandId: number) {
    // Clear models immediately when brand changes
    this.models.set([]);
    // Reset model selection when brand changes
    this.variantForm.patchValue({ modelId: null });
    
    if (brandId) {
      this.modelService.getModels(brandId).subscribe({
        next: (result: IModel[]) => {
          this.models.set(result);
          console.log('Models loaded:', result);
        },
        error: (error) => {
          console.error('Error loading models:', error);
          this.models.set([]);
        }
      });
    }
  }

  loadEngines(){
    this.createService.getAllEngines().subscribe((result: SelectEngineDto[]) =>
    this.engines.set(result))
  }

  loadTransmissions(){
    this.createService.getAllTransmission().subscribe((result: SelectTransmissionDto[]) =>
    this.transmissions.set(result))
  }

  loadDrivetrains(){
    this.createService.getAllDrivetrain().subscribe((result: SelectDrivetrainDto[]) =>
    this.drivetrains.set(result))
  }

  variantForm = this.fb.group({
  brandId: [
    null as number | null,
    Validators.required
  ],
  modelId: [
    null as number | null,
    Validators.required
  ],
  variantName: ['', Validators.required],
  engineId: [
    null as number | null,
    Validators.required
  ],
  transmissionId: [
    null as number | null,
    Validators.required
  ],
  drivetrainId: [
    null as number | null,
    Validators.required
  ],
  exShowroomPrice: [
    null as number | null,
    [
      Validators.required,
      Validators.min(0)
    ]
  ],
  variantImageUrl: ['']
  });


  createVariant() {
    if (this.variantForm.invalid) {
    this.variantForm.markAllAsTouched();
    return;
    }

    const modelId = this.variantForm.value.modelId;

    const dto = {
    variantName: this.variantForm.value.variantName!,
    engineId: this.variantForm.value.engineId!,
    transmissionId: this.variantForm.value.transmissionId!,
    drivetrainId: this.variantForm.value.drivetrainId!,
    exShowroomPrice: this.variantForm.value.exShowroomPrice!,
    variantImageUrl:
      this.variantForm.value.variantImageUrl || null
    };

    this.createService.createVariant(modelId!, dto).subscribe({
      next: (result: VariantDto) => {
        console.log('Variant created:', result);
        this.showSuccessAlert = true;
        this.showErrorAlert = false;
        this.showModal = false;
        this.variantForm.reset();
      },
      error: (error) => {
        console.error('Failed to create variant:', error);
        this.showSuccessAlert = false;
        this.showErrorAlert = true;
        this.showModal = false;
        this.errorMessage =
          error?.error?.message ||
          error?.error?.title ||
          error?.message ||
          'Something went wrong while creating the variant.';
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
