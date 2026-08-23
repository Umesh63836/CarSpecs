import { Component, inject, OnInit, signal, Output, EventEmitter } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateServices } from '../../../core/services/AdminServices/create-services';
import { SelectFueltypeDto } from '../../../core/models/interfaces/selectDtos/select-fueltype-dto';

@Component({
  selector: 'app-create-engine',
  imports: [ReactiveFormsModule],
  templateUrl: './create-engine.html',
  styleUrl: './create-engine.css',
})
export class CreateEngine implements OnInit{
   private fb = inject(FormBuilder);
  private createService = inject(CreateServices);

  @Output() close = new EventEmitter<void>();

  showModal = true;
  showSuccessAlert = false;
  showErrorAlert = false;
  errorMessage = '';

  fuelTypes = signal<SelectFueltypeDto[]>([])

  ngOnInit(): void {
    this.loadFuelTypes();
  }

  engineForm = this.fb.group({

    engineName: [
      '',
      Validators.required
    ],

    fuelTypeId: [
      null as number | null,
      Validators.required
    ],

    numberOfCylinders: [
      null as number | null,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    numberOfValves: [
      null as number | null,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    displacement: [
      null as number | null,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],

    maxPower: [
      null as number | null,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],

    maxTorque: [
      null as number | null,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],

    isTurbocharged: [
            false
    ],

    emissionStandard: [
      '',
      Validators.required
    ]

  });

  loadFuelTypes(){
      this.createService.getAllFueltype().subscribe((result: SelectFueltypeDto[]) =>
      this.fuelTypes.set(result))
    }


  createEngine() {

    if (this.engineForm.invalid) {
      this.engineForm.markAllAsTouched();
      return;
    }

    const dto = {
      engineName: this.engineForm.value.engineName!,
      fuelTypeId: this.engineForm.value.fuelTypeId!,
      numberOfCylinders:
        this.engineForm.value.numberOfCylinders!,
      numberOfValves:
        this.engineForm.value.numberOfValves!,
              displacement:
        this.engineForm.value.displacement!,
      maxPower:
        this.engineForm.value.maxPower!,
      maxTorque:
        this.engineForm.value.maxTorque!,
      isTurbocharged:
        this.engineForm.value.isTurbocharged ?? false,
      emissionStandard:
        this.engineForm.value.emissionStandard!
    };

    this.createService
      .createEngine(dto)
      .subscribe({

        next: response => {

          console.log('Engine created:', response);

          this.showSuccessAlert = true;
          this.showErrorAlert = false;
          this.showModal = false;

          this.engineForm.reset({
            isTurbocharged: false
          });

        },

        error: error => {
          console.error('Failed to create engine:', error);

          this.showSuccessAlert = false;
          this.showErrorAlert = true;
          this.showModal = false;

          this.errorMessage =
            error?.error?.message ||
            error?.error?.title ||
            error?.message ||
            'Something went wrong while creating the engine.';
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

