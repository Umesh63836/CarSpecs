import {
  CommonModule
} from '@angular/common';

import {
  Component,
  OnInit,
  inject,
  signal
} from '@angular/core';

import {
  FormsModule
} from '@angular/forms';

import {
  StagedDataService
} from '../../../core/services/AdminServices/StagedDataService/staged-data-service';

import {
  StagedModelListDto,
  StagedModelDetailDto,
  StagedVariantDetailDto,
  StagedFeatureDto,
  StagedSpecificationDto,
  StagedFuelEfficiencyDto,
  StagedWarrantyDto,
  StagedEnginePerformanceDto,
  StagedMotorPerformanceDto,
  UpdateStagedModelDto,
  UpdateStagedVariantDto,
  UpdateStagedEngineDto,
  UpdateStagedEnginePerformanceDto,
  UpdateStagedMotorPerformanceDto,
  UpdateStagedFeatureDto,
  UpdateStagedSpecificationDto,
  UpdateStagedFuelEfficiencyDto,
  UpdateStagedWarrantyDto,
  CreateStagedFuelEfficiencyDto,
  CreateStagedWarrantyDto,
  CreateStagedFeatureDto,
  CreateStagedSpecificationDto,
  CreateStagedPowertrainDto,
  CreateStagedTransmissionDto,
  CreateStagedDrivetrainDto,
  CreateStagedEngineDto,
  CreateStagedEnginePerformanceDto,
  CreateStagedMotorPerformanceDto,
  StagedPowertrainDto,
  StagedTransmissionDto,
  StagedDrivetrainDto,
  UpdateStagedDrivetrainDto,
  UpdateStagedPowertrainDto,
  UpdateStagedTransmissionDto,
  StagedEngineLookupDto,
  StagedEnginePerformanceLookupDto,
  StagedMotorPerformanceLookupDto,
  StagedFeatureCatalogDto,
  StagedSpecificationCatalogDto,
  StagedPowertrainLookupDto,
  StagedTransmissionLookupDto,
  StagedDrivetrainLookupDto,
  ReplaceStagedPowertrainReferenceDto,
  ReplaceStagedTransmissionReferenceDto,
  ReplaceStagedDrivetrainReferenceDto
} from '../../../core/models/interfaces/DataProcessingDtos/StagedDataDtos/staged-data-dto';
import { Observable } from 'rxjs';


type EditableType =
  | 'text'
  | 'number'
  | 'boolean'
  | 'nullable-number';


type EditTarget =
  | 'model'
  | 'dimensions'
  | 'variant'
  | 'engine'
  | 'enginePerformance'
  | 'motorPerformance'
  | 'feature'
  | 'specification'
  | 'fuelEfficiency'
  | 'warranty';


type ModalType =
  | 'feature'
  | 'specification'
  | 'fuelEfficiency'
  | 'warranty'
  | 'powertrain'
  | 'transmission'
  | 'drivetrain'
  | 'referencePowertrain'
  | 'referenceTransmission'
  | 'referenceDrivetrain'
  | null;


@Component({
  selector: 'app-staged-data',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './staged-data.html'
})
export class StagedData implements OnInit {

  private readonly service =
    inject(StagedDataService);


  // ============================================================
  // MODEL LIST
  // ============================================================

  models =
    signal<StagedModelListDto[]>([]);

  selectedModel =
    signal<StagedModelDetailDto | null>(null);

  currentPage =
    signal(1);

  pageSize =
    signal(10);

  totalPages =
    signal(0);

  totalRecords =
    signal(0);

  hasPreviousPage =
    signal(false);

  hasNextPage =
    signal(false);


  // ============================================================
  // VARIANT DETAIL PAGE
  // ============================================================

  selectedVariant =
    signal<StagedVariantDetailDto | null>(null);

  selectedParentName =
    signal<string | null>(null);


  selectedVariantId =
    signal<number | null>(null);

  existingEngines = signal<StagedEngineLookupDto[]>([]);
  existingEnginePerformances = signal<StagedEnginePerformanceLookupDto[]>([]);
  existingMotorPerformances = signal<StagedMotorPerformanceLookupDto[]>([]);
  powertrainDependencyLoading = signal(false);

  existingPowertrains = signal<StagedPowertrainLookupDto[]>([]);
  existingTransmissions = signal<StagedTransmissionLookupDto[]>([]);
  existingDrivetrains = signal<StagedDrivetrainLookupDto[]>([]);

  powertrainReferenceLoading = signal(false);
  transmissionReferenceLoading = signal(false);
  drivetrainReferenceLoading = signal(false);

  featureCatalog = signal<StagedFeatureCatalogDto[]>([]);
  specificationCatalog = signal<StagedSpecificationCatalogDto[]>([]);
  featureSpecificationCatalogLoading = signal(false);


  // ============================================================
  // UI
  // ============================================================

  loadingModels =
    signal(false);

  loadingModel =
    signal(false);

  saving =
    signal(false);

  searchTerm =
    signal('');

  expandedParents =
    signal<Set<number>>(new Set());

  showModal=
    signal(false);


  // ============================================================
  // EDIT
  // ============================================================

  editingKey =
    signal<string | null>(null);

  editingTarget =
    signal<EditTarget | null>(null);

  editingType =
    signal<EditableType>('text');

  editValue =
    signal<any>(null);


  // ============================================================
  // MODAL
  // ============================================================

  modalType =
    signal<ModalType>(null);


  modalTitle =
    signal('');

  modalData =
    signal<any>({});


  // ============================================================
  // ALERT
  // ============================================================

  alertMessage =
    signal('');

  alertType =
    signal<'success' | 'error'>('success');


  // ============================================================
  // INIT
  // ============================================================

  ngOnInit(): void {
    this.loadModels();
  }


  // ============================================================
  // MODEL LIST
  // ============================================================

  loadModels(): void {

    this.loadingModels.set(true);

    this.service
      .getModels(
        this.currentPage(),
        this.pageSize(),
        this.searchTerm()
      )
      .subscribe({

        next: result => {

          this.models.set(
            result.models ?? []
          );

          this.currentPage.set(
            result.pageNumber
          );

          this.pageSize.set(
            result.pageSize
          );

          this.totalRecords.set(
            result.totalRecords
          );

          this.totalPages.set(
            result.totalPages
          );

          this.hasPreviousPage.set(
            result.hasPreviousPage
          );

          this.hasNextPage.set(
            result.hasNextPage
          );

          this.loadingModels.set(false);
        },

        error: error => {

          this.loadingModels.set(false);

          this.showError(
            this.getApiError(
              error,
              'Unable to load staged models.'
            )
          );
        }
      });
  }


  search(): void {

    this.currentPage.set(1);

    this.closeModel();

    this.loadModels();
  }


  clearSearch(): void {

    this.searchTerm.set('');

    this.currentPage.set(1);

    this.closeModel();

    this.loadModels();
  }


  previousPage(): void {

    if (!this.hasPreviousPage()) {
      return;
    }

    this.currentPage.update(
      x => x - 1
    );

    this.closeModel();

    this.loadModels();
  }


  nextPage(): void {

    if (!this.hasNextPage()) {
      return;
    }

    this.currentPage.update(
      x => x + 1
    );

    this.closeModel();

    this.loadModels();
  }


  // ============================================================
  // MODEL
  // ============================================================

openModel(model: StagedModelListDto): void {

  this.loadingModel.set(true);
  this.selectedModel.set(null);
  this.selectedVariant.set(null);
  this.selectedParentName.set(null);

  this.expandedParents.set(new Set());
  this.cancelEdit();

  this.service.getModel(model.importModelId).subscribe({
    
    next: result => {

      console.log(
        'STAGED MODEL DETAIL:',
        result
      );

      console.log(
        'VARIANT PARENTS:',
        result.parentVariants
      );

      console.log(
        'PARENT COUNT:',
        result.parentVariants?.length
      );
      
      this.selectedModel.set({
        ...result,

        parentVariants:
          result.parentVariants ?? []
      });

      this.loadingModel.set(false);
    },

    error: error => {

      this.loadingModel.set(false);

      this.showError(
        this.getApiError(
          error,
          'Unable to load staged model.'
        )
      );
    }
  });
}


  closeModel(): void {

    this.selectedModel.set(null);

    this.selectedVariant.set(null);

    this.selectedVariantId.set(null);

    this.selectedParentName.set(null);

    this.expandedParents.set(
      new Set()
    );

    this.cancelEdit();
  }


  // ============================================================
  // PARENT
  // ============================================================

  toggleParent(
    parentId: number
  ): void {

    const next =
      new Set(
        this.expandedParents()
      );

    if (next.has(parentId)) {

      next.delete(parentId);

    } else {

      next.add(parentId);

    }

    this.expandedParents.set(next);
  }


  isParentExpanded(
    parentId: number
  ): boolean {

    return this.expandedParents()
      .has(parentId);
  }


  expandAllParents(): void {

    const model =
      this.selectedModel();

    if (!model) {
      return;
    }

    this.expandedParents.set(
      new Set(
        model.parentVariants.map(
          x => x.importVariantParentId
        )
      )
    );
  }


  collapseAllParents(): void {

    this.expandedParents.set(
      new Set()
    );
  }


  // ============================================================
  // SUBVARIANT
  // ============================================================

  openVariantPage(
    variant: StagedVariantDetailDto,
    parentName: string
  ): void {

    /*
     * Parent rows are displayed but are not detail pages.
     */
    if (
      variant.variantType.toLowerCase() ===
      'parent'
    ) {
      return;
    }

    this.cancelEdit();

    this.selectedParentName.set(
      parentName
    );

    this.selectedVariant.set(
      variant
    );

    this.selectedVariantId.set(
      variant.importVariantId
    );

    this.selectedVariantId.set(
      variant.importVariantId
    );

    /*
     * Start with all important sections expanded.
     * The admin can collapse individual sections.
     */
    this.expandedSections.set(
      new Set([
        'basic',
        'powertrain',
        'engine',
        'enginePerformance',
        'transmission',
        'drivetrain',
        'motorPerformance',
        'features',
        'specifications',
        'fuelEfficiency',
        'warranty'
      ])
    );

    setTimeout(() => {

      window.scrollTo({
        top: 0,
        behavior: 'smooth'
      });

    });
  }


  backToModel(): void {

    this.cancelEdit();

    this.selectedVariant.set(
      null
    );

    this.selectedVariantId.set(
      null
    );

    this.selectedParentName.set(
      null
    );

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }


  // ============================================================
  // SECTION STATE
  // ============================================================

  expandedSections =
    signal<Set<string>>(
      new Set()
    );


  toggleSection(
    section: string
  ): void {

    const next =
      new Set(
        this.expandedSections()
      );

    if (next.has(section)) {

      next.delete(section);

    } else {

      next.add(section);

    }

    this.expandedSections.set(next);
  }


  isSectionExpanded(
    section: string
  ): boolean {

    return this.expandedSections()
      .has(section);
  }


  // ============================================================
  // EDIT
  // ============================================================

  beginEdit(
    key: string,
    value: any,
    target: EditTarget,
    type: EditableType = 'text'
  ): void {

    this.editingKey.set(key);

    this.editingTarget.set(
      target
    );

    this.editingType.set(
      type
    );

    /*
     * Empty string is intentional here.
     * It allows a null database value to become editable.
     */
    this.editValue.set(
      value === null ||
      value === undefined
        ? ''
        : value
    );
  }


  cancelEdit(): void {

    this.editingKey.set(null);

    this.editingTarget.set(null);

    this.editingType.set('text');

    this.editValue.set(null);
  }


  isEditing(
    key: string
  ): boolean {

    return this.editingKey() === key;
  }


  private convertEditedValue(): any {

    const value =
      this.editValue();

    const type =
      this.editingType();

    if (
      value === null ||
      value === undefined ||
      value === ''
    ) {

      return null;
    }


    if (type === 'number' ||
        type === 'nullable-number') {

      const numberValue =
        Number(value);

      return Number.isNaN(numberValue)
        ? null
        : numberValue;
    }


    if (type === 'boolean') {

      if (value === true ||
          value === 'true') {

        return true;
      }

      if (value === false ||
          value === 'false') {

        return false;
      }

      return null;
    }


    return String(value);
  }


  // ============================================================
  // MODEL SAVE
  // ============================================================

  saveModelField(
    field:
      'brandName' |
      'modelName' |
      'category' |
      'bodyType' |
      'modelImageUrl'
  ): void {

    const model =
      this.selectedModel();

    if (!model) {
      return;
    }

    const dto: UpdateStagedModelDto = {

      brandName:
        model.brandName,

      modelName:
        model.modelName,

      category:
        model.category,

      bodyType:
        model.bodyType,

      modelImageUrl:
        model.modelImageUrl,

      dimensions:
        this.buildDimensionsDto()
    };

    (dto as any)[field] =
      this.convertEditedValue();

    this.saving.set(true);

    this.service
      .updateModel(
        model.importModelId,
        dto
      )
      .subscribe({

        next: () => {

          this.saving.set(false);

          this.cancelEdit();

          this.showSuccess(
            'Model updated successfully.'
          );

          this.reloadSelectedModel();
        },

        error: error => {

          this.saving.set(false);

          this.showError(
            this.getApiError(
              error,
              'Unable to update model.'
            )
          );
        }
      });
  }


  private buildDimensionsDto(): any {

    const dimensions =
      this.selectedModel()?.dimensions;

    return {

      lengthMm:
        dimensions?.lengthMm ?? null,

      widthMm:
        dimensions?.widthMm ?? null,

      heightMm:
        dimensions?.heightMm ?? null,

      wheelbaseMm:
        dimensions?.wheelbaseMm ?? null,

      groundClearanceMm:
        dimensions?.groundClearanceMm ?? null,

      bootSpaceLitres:
        dimensions?.bootSpaceLitres ?? null,

      fuelTankCapacityLitres:
        dimensions?.fuelTankCapacityLitres ?? null,

      sourceId:
        dimensions?.sourceId ?? null,

      evidenceText:
        dimensions?.evidenceText ?? null,

      pageNumber:
        dimensions?.pageNumber ?? null,

      confidence:
        dimensions?.confidence ?? null
    };
  }


  // ============================================================
  // DIMENSION SAVE
  // ============================================================

  saveDimensionField(
    field:
      'lengthMm' |
      'widthMm' |
      'heightMm' |
      'wheelbaseMm' |
      'groundClearanceMm' |
      'bootSpaceLitres' |
      'fuelTankCapacityLitres'
  ): void {

    const model =
      this.selectedModel();

    if (!model) {
      return;
    }

    const dimensions =
      this.buildDimensionsDto();

    dimensions[field] =
      this.convertEditedValue();

    const dto: UpdateStagedModelDto = {

      brandName:
        model.brandName,

      modelName:
        model.modelName,

      category:
        model.category,

      bodyType:
        model.bodyType,

      modelImageUrl:
        model.modelImageUrl,

      dimensions
    };

    this.saving.set(true);

    this.service
      .updateModel(
        model.importModelId,
        dto
      )
      .subscribe({

        next: () => {

          this.saving.set(false);

          this.cancelEdit();

          this.showSuccess(
            'Dimensions updated successfully.'
          );

          this.reloadSelectedModel();
        },

        error: error => {

          this.saving.set(false);

          this.showError(
            this.getApiError(
              error,
              'Unable to update dimensions.'
            )
          );
        }
      });
  }


  // ============================================================
  // VARIANT SAVE
  // ============================================================

  saveVariantField(
    variant: StagedVariantDetailDto,
    field:
      'variantName' |
      'baseVariantName' |
      'exShowroomPrice' |
      'kerbWeight' |
      'seatingCapacity' |
      'powertrainRef' |
      'transmissionRef' |
      'drivetrainRef'
  ): void {

    const dto: UpdateStagedVariantDto = {

      variantName:
        variant.variantName,

      baseVariantName:
        variant.baseVariantName,

      exShowroomPrice:
        variant.exShowroomPrice,

      kerbWeight:
        variant.kerbWeight,

      seatingCapacity:
        variant.seatingCapacity,

      powertrainRef:
        variant.powertrainRef,

      transmissionRef:
        variant.transmissionRef,

      drivetrainRef:
        variant.drivetrainRef
    };

    (dto as any)[field] =
      this.convertEditedValue();

    this.saving.set(true);

    this.service
      .updateVariant(
        variant.importVariantId,
        dto
      )
      .subscribe({

        next: () => {

          this.saving.set(false);

          this.cancelEdit();

          this.showSuccess(
            'Variant updated successfully.'
          );

          this.reloadSelectedModel(
            true
          );
        },

        error: error => {

          this.saving.set(false);

          this.showError(
            this.getApiError(
              error,
              'Unable to update variant.'
            )
          );
        }
      });
  }


  // ============================================================
  // ENGINE
  // ============================================================

  saveEngineField(
    engine: any,
    field:
      'engineName' |
      'numberOfCylinders' |
      'numberOfValves' |
      'displacement' |
      'isTurbocharged' |
      'emissionStandard' |
      'aspiration' |
      'engineType'
  ): void {

    const dto: UpdateStagedEngineDto = {

      engineName:
        engine.engineName,

      numberOfCylinders:
        engine.numberOfCylinders,

      numberOfValves:
        engine.numberOfValves,

      displacement:
        engine.displacement,

      isTurbocharged:
        engine.isTurbocharged,

      emissionStandard:
        engine.emissionStandard,

      aspiration:
        engine.aspiration,

      engineType:
        engine.engineType
    };

    (dto as any)[field] =
      this.convertEditedValue();

    this.saving.set(true);

    this.service
      .updateEngine(
        engine.importEngineId,
        dto
      )
      .subscribe({

        next: () => {

          this.saving.set(false);

          this.cancelEdit();

          this.showSuccess(
            'Engine updated successfully.'
          );

          this.reloadSelectedModel(
            true
          );
        },

        error: error => {

          this.saving.set(false);

          this.showError(
            this.getApiError(
              error,
              'Unable to update engine.'
            )
          );
        }
      });
  }


  // ============================================================
  // ENGINE PERFORMANCE
  // ============================================================

saveEnginePerformanceField(
  performance: StagedEnginePerformanceDto,
  field: keyof UpdateStagedEnginePerformanceDto
): void {

  const value = this.editValue();

  const payload: UpdateStagedEnginePerformanceDto = {
    modeName: performance.modeName,
    fuelTypeName: performance.fuelTypeName,

    maxPower: performance.maxPower,
    maxPowerUnit: performance.maxPowerUnit,

    maxTorque: performance.maxTorque,
    maxTorqueUnit: performance.maxTorqueUnit,

    maxPowerRPM: performance.maxPowerRPM,
    maxPowerRPMMin: performance.maxPowerRPMMin,
    maxPowerRPMMax: performance.maxPowerRPMMax,

    maxTorqueRPM: performance.maxTorqueRPM,
    maxTorqueRPMMin: performance.maxTorqueRPMMin,
    maxTorqueRPMMax: performance.maxTorqueRPMMax
  };

  (payload as any)[field] =
    this.normalizeEditValue(field, value);

  this.saving.set(true);

  this.service
    .updateEnginePerformance(
      performance.importEnginePerformanceId,
      payload
    )
    .subscribe({
      next: () => {
        this.saving.set(false);
        this.cancelEdit();

        this.showSuccess(
          'Engine performance updated.'
        );

        this.reloadSelectedModel();
      },

      error: error => {
        this.saving.set(false);

        this.showError(
          this.getApiError(
            error,
            'Unable to update engine performance.'
          )
        );
      }
    });
}

private normalizeEditValue(
  field: string,
  value: any
): any {

  if (value === '' || value === undefined) {
    return null;
  }

  const numericFields = new Set([
    'maxPower',
    'maxTorque',
    'maxPowerRPM',
    'maxPowerRPMMin',
    'maxPowerRPMMax',
    'maxTorqueRPM',
    'maxTorqueRPMMin',
    'maxTorqueRPMMax'
  ]);

  if (numericFields.has(field)) {
    return value === null
      ? null
      : Number(value);
  }

  return value;
}


  // ============================================================
  // MOTOR PERFORMANCE
  // ============================================================

  saveMotorPerformanceField(
    performance: StagedMotorPerformanceDto,
    field: keyof UpdateStagedMotorPerformanceDto
  ): void {

    const payload: UpdateStagedMotorPerformanceDto = {
      motorName: performance.motorName,

      maxPower: performance.maxPower,
      maxPowerUnit: performance.maxPowerUnit,

      maxTorque: performance.maxTorque,
      maxTorqueUnit: performance.maxTorqueUnit,

      maxPowerRPM: performance.maxPowerRPM,
      maxPowerRPMMin: performance.maxPowerRPMMin,
      maxPowerRPMMax: performance.maxPowerRPMMax,

      maxTorqueRPM: performance.maxTorqueRPM,
      maxTorqueRPMMin: performance.maxTorqueRPMMin,
      maxTorqueRPMMax: performance.maxTorqueRPMMax
    };

    (payload as any)[field] =
      this.normalizeEditValue(
        field,
        this.editValue()
      );

    this.saveSimpleUpdate(
      this.service.updateMotorPerformance(
        performance.importMotorPerformanceId,
        payload
      ),
      'Motor performance updated.'
    );
  }



  // ============================================================
  // ADD MODALS
  // ============================================================

  openFeatureModal(): void {

    const model =
      this.selectedModel();

    const variant =
      this.selectedVariant();

    if (!model || !variant) {
      return;
    }

    this.modalType.set(
      'feature'
    );

    this.modalTitle.set(
      'Add Feature'
    );

  this.loadFeatureSpecificationCatalog();

  this.showModal.set(true);

    this.modalData.set({

      importModelId:
        model.importModelId,

      importVariantId:
        variant.importVariantId,

      importFeatureId:
        null,

      featureCode:
        '',

      available:
        true,

      value:
        '',

      valueOptionIds:
        [],

      sourceColumn:
        '',

      pageNumber:
        null,

      evidence:
        '',

      confidence:
        null
    });
  }


  openSpecificationModal(): void {

    const model =
      this.selectedModel();

    const variant =
      this.selectedVariant();

    if (!model || !variant) {
      return;
    }

    this.loadFeatureSpecificationCatalog();

    this.showModal.set(true);

    this.modalType.set(
      'specification'
    );

    this.modalTitle.set(
      'Add Specification'
    );

    this.modalData.set({

      importModelId:
        model.importModelId,

      importVariantId:
        variant.importVariantId,

      importSpecificationId:
        null,

      specificationCode:
        '',

      numericValue:
        null,

      textValue:
        null,

      booleanValue:
        null,

      unit:
        null,

      sourceColumn:
        null,

      pageNumber:
        null,

      evidence:
        null,

      confidence:
        null
    });



  }


  openFuelEfficiencyModal(): void {

    const model =
      this.selectedModel();

    const variant =
      this.selectedVariant();

    if (!model || !variant) {
      return;
    }

    this.modalType.set(
      'fuelEfficiency'
    );

    this.modalTitle.set(
      'Add Fuel Efficiency'
    );

    this.modalData.set({

      importModelId:
        model.importModelId,

      fuelEfficiency:
        null,

      fuelEfficiencyUnit:
        'km/L',

      sourceColumn:
        null,

      pageNumber:
        null,

      evidence:
        null,

      confidence:
        null,

      importVariantIds:
        [
          variant.importVariantId
        ]
    });
  }


  openWarrantyModal(): void {

    const model =
      this.selectedModel();

    const variant =
      this.selectedVariant();

    if (!model || !variant) {
      return;
    }

    this.modalType.set(
      'warranty'
    );

    this.modalTitle.set(
      'Add Warranty'
    );

    this.modalData.set({

      importModelId:
        model.importModelId,

      importVariantId:
        variant.importVariantId,

      warrantyType:
        '',

      durationYears:
        null,

      kilometres:
        null,

      maximumDurationYears:
        null,

      maximumKilometres:
        null,

      sourceId:
        null,

      evidenceText:
        null,

      pageNumber:
        null,

      confidence:
        null
    });
  }


setModalField(
  field: string,
  value: any
): void {

  const integerFields = new Set([
    'numberOfGears',
    'pageNumber',
    'sourceId',
    'numberOfCylinders',
    'numberOfValves'
  ]);

  const decimalFields = new Set([
    'batteryCapacityKWh',
    'combinedMaxPower',
    'combinedMaxTorque',
    'maxPower',
    'maxTorque',
    'confidence',
    'displacement',
    'durationYears',
    'kilometres',
    'maximumDurationYears',
    'maximumKilometres',
    'fuelEfficiency'
  ]);

  if (integerFields.has(field)) {
    value =
      value === '' ||
      value === null ||
      value === undefined
        ? null
        : Number(value);
  }

  if (decimalFields.has(field)) {
    value =
      value === '' ||
      value === null ||
      value === undefined
        ? null
        : Number(value);
  }

  const current = this.modalData();

  if (field === 'engineMode' && value === 'new' && !current.newEngine) {
    current.newEngine = {
      engineRef: '',
      engineName: '',
      numberOfCylinders: null,
      numberOfValves: null,
      displacement: null,
      isTurbocharged: null,
      emissionStandard: null,
      aspiration: null,
      engineType: null
    };
  }

  if (field === 'enginePerformanceMode' && value === 'new' && !current.newEnginePerformance) {
    current.newEnginePerformance = {
      modeName: '',
      fuelTypeId: null,
      maxPower: null,
      maxTorque: null,
      maxPowerRPM: null,
      maxPowerRPMMin: null,
      maxPowerRPMMax: null,
      maxTorqueRPM: null,
      maxTorqueRPMMin: null,
      maxTorqueRPMMax: null
    };
  }

  this.modalData.set({
    ...current,
    [field]: value
  });
}


  getConcreteVariants(): StagedVariantDetailDto[] {

    const model = this.selectedModel();

    if (!model) {
      return [];
    }

    return (model.parentVariants ?? [])
      .flatMap(parent =>
        parent.subVariants ?? []
      )
      .filter(variant =>
        variant.variantType?.toLowerCase() !== 'parent'
      );
  }


  setModalVariantChecked(
    variantId: number,
    checked: boolean
  ): void {

    const currentIds =
      Array.isArray(this.modalData()?.importVariantIds)
        ? [...this.modalData().importVariantIds]
        : [];

    const nextIds = new Set<number>(
      currentIds
        .map((id: any) => Number(id))
        .filter((id: number) =>
          Number.isInteger(id) && id > 0
        )
    );

    if (checked) {
      nextIds.add(variantId);
    } else {
      nextIds.delete(variantId);
    }

    this.setModalField(
      'importVariantIds',
      [...nextIds]
    );
  }


  closeModal(): void {

    this.modalType.set(null);

    this.modalTitle.set('');

    this.modalData.set({});
  }


  private toNullableString(value: any): string | null {
    if (value === null || value === undefined || value === '') {
      return null;
    }

    return String(value);
  }


  private toNullableNumber(value: any): number | null {
    if (value === null || value === undefined || value === '') {
      return null;
    }

    const numberValue = Number(value);
    return Number.isFinite(numberValue) ? numberValue : null;
  }


  private toNullableBoolean(value: any): boolean | null {
    if (value === true || value === 'true') {
      return true;
    }

    if (value === false || value === 'false') {
      return false;
    }

    return null;
  }


  private isPositiveInteger(value: any): boolean {
    const numberValue = Number(value);
    return Number.isInteger(numberValue) && numberValue > 0;
  }


  saveModal(): void {

    const type = this.modalType();
    const data = this.modalData();

    if (!type) {
      return;
    }

    if (type === 'fuelEfficiency') {
      const model = this.selectedModel();
      const variant = this.selectedVariant();

      if (!model || !variant) {
        this.showError('Select a variant before adding fuel efficiency.');
        return;
      }

      const importVariantIds: number[] = Array.isArray(data.importVariantIds)
        ? data.importVariantIds
            .map((id: unknown) => Number(id))
            .filter((id: number) => Number.isInteger(id) && id > 0)
        : [variant.importVariantId];

      const fuelEfficiency =
        data.fuelEfficiency === '' ||
        data.fuelEfficiency === null ||
        data.fuelEfficiency === undefined
          ? null
          : Number(data.fuelEfficiency);

      if (fuelEfficiency === null || !Number.isFinite(fuelEfficiency)) {
        this.showError('Enter a valid fuel efficiency.');
        return;
      }

      const payload: CreateStagedFuelEfficiencyDto = {
        importModelId: model.importModelId,
        fuelEfficiency,
        fuelEfficiencyUnit: String(data.fuelEfficiencyUnit ?? '').trim(),
        sourceColumn: this.toNullableString(data.sourceColumn),
        pageNumber: this.toNullableNumber(data.pageNumber),
        evidence: this.toNullableString(data.evidence),
        confidence: this.toNullableNumber(data.confidence),
        importVariantIds
      };

      this.saving.set(true);
      this.service.createFuelEfficiency(payload).subscribe({
        next: () => {
          this.saving.set(false);
          this.closeModal();
          this.showSuccess('Fuel efficiency added.');
          this.reloadSelectedModel(true);
        },
        error: error => {
          this.saving.set(false);
          this.showError(this.getApiError(error, 'Unable to add fuel efficiency.'));
        }
      });
      return;
    }

    if (type === 'warranty') {
      const model = this.selectedModel();
      const variant = this.selectedVariant();

      if (!model || !variant) {
        this.showError('Select a variant before adding warranty.');
        return;
      }

      const warrantyType = String(data.warrantyType ?? '').trim();
      if (!warrantyType) {
        this.showError('Enter a warranty type.');
        return;
      }

      const payload: CreateStagedWarrantyDto = {
        importModelId: model.importModelId,
        importVariantId: this.toNullableNumber(data.importVariantId) ?? variant.importVariantId,
        warrantyType,
        durationYears: this.toNullableNumber(data.durationYears),
        kilometres: this.toNullableNumber(data.kilometres),
        maximumDurationYears: this.toNullableNumber(data.maximumDurationYears),
        maximumKilometres: this.toNullableNumber(data.maximumKilometres),
        sourceId: this.toNullableNumber(data.sourceId),
        evidenceText: this.toNullableString(data.evidenceText),
        pageNumber: this.toNullableNumber(data.pageNumber),
        confidence: this.toNullableNumber(data.confidence)
      };

      this.saving.set(true);
      this.service.createWarranty(payload).subscribe({
        next: () => {
          this.saving.set(false);
          this.closeModal();
          this.showSuccess('Warranty added.');
          this.reloadSelectedModel(true);
        },
        error: error => {
          this.saving.set(false);
          this.showError(this.getApiError(error, 'Unable to add warranty.'));
        }
      });
      return;
    }

    if (type === 'specification') {
      const model = this.selectedModel();
      const variantId = this.selectedVariantId();

      if (!model) {
        this.showError('No staged model is selected.');
        return;
      }
      if (variantId === null) {
        this.showError('Select a variant before adding a specification.');
        return;
      }

      const specificationCode = String(data.specificationCode ?? '').trim();
      if (!specificationCode) {
        this.showError('Select a specification.');
        return;
      }

      const payload: CreateStagedSpecificationDto = {
        importModelId: model.importModelId,
        importVariantId: variantId,
        importSpecificationId: this.toNullableNumber(data.importSpecificationId),
        specificationCode,
        numericValue: this.toNullableNumber(data.numericValue),
        textValue: this.toNullableString(data.textValue),
        booleanValue: this.toNullableBoolean(data.booleanValue),
        unit: this.toNullableString(data.unit),
        sourceColumn: this.toNullableString(data.sourceColumn),
        pageNumber: this.toNullableNumber(data.pageNumber),
        evidence: this.toNullableString(data.evidence),
        confidence: this.toNullableNumber(data.confidence)
      };

      this.saving.set(true);
      this.service.createSpecification(payload).subscribe({
        next: () => {
          this.saving.set(false);
          this.closeModal();
          this.showSuccess('Specification added.');
          this.reloadSelectedModel(true);
        },
        error: error => {
          this.saving.set(false);
          this.showError(this.getApiError(error, 'Unable to add specification.'));
        }
      });
      return;
    }

    if (type === 'feature') {
      const model = this.selectedModel();
      const variantId = this.selectedVariantId();

      if (!model) {
        this.showError('No staged model is selected.');
        return;
      }
      if (variantId === null) {
        this.showError('Select a variant before adding a feature.');
        return;
      }

      const featureCode = String(data.featureCode ?? '').trim();
      if (!featureCode) {
        this.showError('Select a feature.');
        return;
      }

      const payload: CreateStagedFeatureDto = {
        importModelId: model.importModelId,
        importVariantId: variantId,
        importFeatureId: this.toNullableNumber(data.importFeatureId),
        featureCode,
        available: this.toNullableBoolean(data.available),
        value: this.toNullableString(data.value),
        valueOptionIds: Array.isArray(data.valueOptionIds)
          ? data.valueOptionIds
              .map((id: unknown) => Number(id))
              .filter((id: number) => Number.isInteger(id) && id > 0)
          : [],
        sourceColumn: this.toNullableString(data.sourceColumn),
        pageNumber: this.toNullableNumber(data.pageNumber),
        evidence: this.toNullableString(data.evidence),
        confidence: this.toNullableNumber(data.confidence)
      };

      this.saving.set(true);
      this.service.createFeature(payload).subscribe({
        next: () => {
          this.saving.set(false);
          this.closeModal();
          this.showSuccess('Feature added.');
          this.reloadSelectedModel(true);
        },
        error: error => {
          this.saving.set(false);
          this.showError(this.getApiError(error, 'Unable to add feature.'));
        }
      });
      return;
    }


    // ============================================================
    // REFERENCE EXISTING POWERTRAIN
    // ============================================================

    if (type === 'referencePowertrain') {
      const variantId = this.selectedVariantId();
      const powertrainId = this.toNullableNumber(
        data.importPowertrainId
      );

      if (variantId === null) {
        this.showError(
          'Select a variant before changing its powertrain reference.'
        );
        return;
      }

      if (powertrainId === null || !this.isPositiveInteger(powertrainId)) {
        this.showError(
          'Select a powertrain to reference.'
        );
        return;
      }

      const currentPowertrainId =
        this.selectedVariant()?.powertrain?.importPowertrainId ?? null;

      if (currentPowertrainId === powertrainId) {
        this.showError(
          'The selected powertrain is already referenced by this variant.'
        );
        return;
      }

      this.saving.set(true);

      const payload: ReplaceStagedPowertrainReferenceDto = {
        importPowertrainId: powertrainId
      };

      this.service
        .replaceVariantPowertrainReference(
          variantId,
          payload
        )
        .subscribe({
          next: result => {
            this.saving.set(false);
            this.closeModal();

            this.showSuccess(
              result.message ??
              'Powertrain reference replaced successfully.'
            );

            this.reloadSelectedModel(true);
          },
          error: error => {
            this.saving.set(false);

            this.showError(
              this.getApiError(
                error,
                'Unable to replace powertrain reference.'
              )
            );
          }
        });

      return;
    }


    // ============================================================
    // REFERENCE EXISTING TRANSMISSION
    // ============================================================

    if (type === 'referenceTransmission') {
      const variantId = this.selectedVariantId();
      const transmissionId = this.toNullableNumber(
        data.importTransmissionId
      );

      if (variantId === null) {
        this.showError(
          'Select a variant before changing its transmission reference.'
        );
        return;
      }

      if (
        transmissionId === null ||
        !this.isPositiveInteger(transmissionId)
      ) {
        this.showError(
          'Select a transmission to reference.'
        );
        return;
      }

      const currentTransmissionId =
        this.selectedVariant()?.transmission?.importTransmissionId ?? null;

      if (currentTransmissionId === transmissionId) {
        this.showError(
          'The selected transmission is already referenced by this variant.'
        );
        return;
      }

      this.saving.set(true);

      const payload: ReplaceStagedTransmissionReferenceDto = {
        importTransmissionId: transmissionId
      };

      this.service
        .replaceVariantTransmissionReference(
          variantId,
          payload
        )
        .subscribe({
          next: result => {
            this.saving.set(false);
            this.closeModal();

            this.showSuccess(
              result.message ??
              'Transmission reference replaced successfully.'
            );

            this.reloadSelectedModel(true);
          },
          error: error => {
            this.saving.set(false);

            this.showError(
              this.getApiError(
                error,
                'Unable to replace transmission reference.'
              )
            );
          }
        });

      return;
    }


    // ============================================================
    // REFERENCE EXISTING DRIVETRAIN
    // ============================================================

    if (type === 'referenceDrivetrain') {
      const variantId = this.selectedVariantId();
      const drivetrainId = this.toNullableNumber(
        data.importDrivetrainId
      );

      if (variantId === null) {
        this.showError(
          'Select a variant before changing its drivetrain reference.'
        );
        return;
      }

      if (
        drivetrainId === null ||
        !this.isPositiveInteger(drivetrainId)
      ) {
        this.showError(
          'Select a drivetrain to reference.'
        );
        return;
      }

      const currentDrivetrainId =
        this.selectedVariant()?.drivetrain?.importDrivetrainId ?? null;

      if (currentDrivetrainId === drivetrainId) {
        this.showError(
          'The selected drivetrain is already referenced by this variant.'
        );
        return;
      }

      this.saving.set(true);

      const payload: ReplaceStagedDrivetrainReferenceDto = {
        importDrivetrainId: drivetrainId
      };

      this.service
        .replaceVariantDrivetrainReference(
          variantId,
          payload
        )
        .subscribe({
          next: result => {
            this.saving.set(false);
            this.closeModal();

            this.showSuccess(
              result.message ??
              'Drivetrain reference replaced successfully.'
            );

            this.reloadSelectedModel(true);
          },
          error: error => {
            this.saving.set(false);

            this.showError(
              this.getApiError(
                error,
                'Unable to replace drivetrain reference.'
              )
            );
          }
        });

      return;
    }


    if (type === 'powertrain') {
      const model = this.selectedModel();

      if (!model) {
        this.showError('No staged model is selected.');
        return;
      }

      const powertrainRef = String(data.powertrainRef ?? '').trim();
      const powertrainType = String(data.powertrainType ?? '').trim();

      if (!powertrainRef) {
        this.showError('Powertrain Ref is required.');
        return;
      }
      if (!powertrainType) {
        this.showError('Powertrain Type is required.');
        return;
      }

      const engineMode = data.engineMode ?? 'none';
      if (!['none', 'existing', 'new'].includes(engineMode)) {
        this.showError('Invalid engine dependency mode.');
        return;
      }

      if (engineMode === 'existing' && !this.isPositiveInteger(data.existingEngineId)) {
        this.showError('Select an existing engine.');
        return;
      }

      const engine = data.newEngine;
      if (engineMode === 'new') {
        if (!engine) {
          this.showError('Enter the new engine details.');
          return;
        }
        if (!String(engine.engineRef ?? '').trim()) {
          this.showError('Engine Ref is required.');
          return;
        }
        if (!String(engine.engineName ?? '').trim()) {
          this.showError('Engine Name is required.');
          return;
        }
      }

      const enginePerformanceMode = data.enginePerformanceMode ?? 'none';
      if (!['none', 'existing', 'new'].includes(enginePerformanceMode)) {
        this.showError('Invalid engine performance dependency mode.');
        return;
      }

      if (enginePerformanceMode === 'existing' && !this.isPositiveInteger(data.existingEnginePerformanceId)) {
        this.showError('Select an existing engine performance.');
        return;
      }

      const enginePerformance = data.newEnginePerformance;
      if (enginePerformanceMode === 'new') {
        if (!enginePerformance) {
          this.showError('Enter the new engine performance details.');
          return;
        }
        if (!String(enginePerformance.modeName ?? '').trim()) {
          this.showError('Engine Performance Mode Name is required.');
          return;
        }
      }

      const existingMotorPerformanceIds: number[] =
        Array.isArray(data.existingMotorPerformanceIds)
          ? data.existingMotorPerformanceIds
              .map((id: unknown) => Number(id))
              .filter((id: number) => Number.isInteger(id) && id > 0)
          : [];

      const newMotorPerformances: CreateStagedMotorPerformanceDto[] =
        Array.isArray(data.newMotorPerformances)
          ? data.newMotorPerformances.map((motor: any) => ({
              motorName: this.toNullableString(motor.motorName),
              maxPower: this.toNullableNumber(motor.maxPower),
              maxTorque: this.toNullableNumber(motor.maxTorque),
              maxPowerRPM: this.toNullableNumber(motor.maxPowerRPM),
              maxPowerRPMMin: this.toNullableNumber(motor.maxPowerRPMMin),
              maxPowerRPMMax: this.toNullableNumber(motor.maxPowerRPMMax),
              maxTorqueRPM: this.toNullableNumber(motor.maxTorqueRPM),
              maxTorqueRPMMin: this.toNullableNumber(motor.maxTorqueRPMMin),
              maxTorqueRPMMax: this.toNullableNumber(motor.maxTorqueRPMMax)
            }))
          : [];

      const normalizedEngine: CreateStagedEngineDto | null =
        engineMode === 'new' && engine
          ? {
              engineRef: String(engine.engineRef ?? '').trim(),
              engineName: String(engine.engineName ?? '').trim(),
              numberOfCylinders: this.toNullableNumber(engine.numberOfCylinders),
              numberOfValves: this.toNullableNumber(engine.numberOfValves),
              displacement: this.toNullableNumber(engine.displacement),
              isTurbocharged: this.toNullableBoolean(engine.isTurbocharged),
              emissionStandard: this.toNullableString(engine.emissionStandard),
              aspiration: this.toNullableString(engine.aspiration),
              engineType: this.toNullableString(engine.engineType)
            }
          : null;

      const normalizedEnginePerformance: CreateStagedEnginePerformanceDto | null =
        enginePerformanceMode === 'new' && enginePerformance
          ? {
              modeName: String(enginePerformance.modeName ?? '').trim(),
              fuelTypeId: this.toNullableNumber(enginePerformance.fuelTypeId),
              maxPower: this.toNullableNumber(enginePerformance.maxPower),
              maxTorque: this.toNullableNumber(enginePerformance.maxTorque),
              maxPowerRPM: this.toNullableNumber(enginePerformance.maxPowerRPM),
              maxPowerRPMMin: this.toNullableNumber(enginePerformance.maxPowerRPMMin),
              maxPowerRPMMax: this.toNullableNumber(enginePerformance.maxPowerRPMMax),
              maxTorqueRPM: this.toNullableNumber(enginePerformance.maxTorqueRPM),
              maxTorqueRPMMin: this.toNullableNumber(enginePerformance.maxTorqueRPMMin),
              maxTorqueRPMMax: this.toNullableNumber(enginePerformance.maxTorqueRPMMax)
            }
          : null;

      const payload: CreateStagedPowertrainDto = {
        importModelId: model.importModelId,
        powertrainRef,
        powertrainType,
        engineRef: this.toNullableString(data.engineRef),
        batteryCapacityKWh: this.toNullableNumber(data.batteryCapacityKWh),
        combinedMaxPower: this.toNullableNumber(data.combinedMaxPower),
        combinedMaxTorque: this.toNullableNumber(data.combinedMaxTorque),
        combinedMaxPowerRPM: this.toNullableNumber(data.combinedMaxPowerRPM),
        combinedMaxPowerRPMMin: this.toNullableNumber(data.combinedMaxPowerRPMMin),
        combinedMaxPowerRPMMax: this.toNullableNumber(data.combinedMaxPowerRPMMax),
        combinedMaxTorqueRPM: this.toNullableNumber(data.combinedMaxTorqueRPM),
        combinedMaxTorqueRPMMin: this.toNullableNumber(data.combinedMaxTorqueRPMMin),
        combinedMaxTorqueRPMMax: this.toNullableNumber(data.combinedMaxTorqueRPMMax),
        engineMode,
        existingEngineId: engineMode === 'existing' ? Number(data.existingEngineId) : null,
        newEngine: normalizedEngine,
        enginePerformanceMode,
        existingEnginePerformanceId: enginePerformanceMode === 'existing' ? Number(data.existingEnginePerformanceId) : null,
        newEnginePerformance: normalizedEnginePerformance,
        existingMotorPerformanceIds,
        newMotorPerformances
      };

      this.saving.set(true);
      this.service.createPowertrain(payload).subscribe({
        next: () => {
          this.saving.set(false);
          this.closeModal();
          this.showSuccess('Powertrain added.');
          this.reloadSelectedModel(true);
        },
        error: error => {
          this.saving.set(false);
          this.showError(this.getApiError(error, 'Unable to add powertrain.'));
        }
      });
      return;
    }

    if (type === 'transmission') {
      const model = this.selectedModel();

      if (!model) {
        this.showError('No staged model is selected.');
        return;
      }

      const transmissionRef = String(data.transmissionRef ?? '').trim();
      if (!transmissionRef) {
        this.showError('Transmission Ref is required.');
        return;
      }

      const payload: CreateStagedTransmissionDto = {
        importModelId: model.importModelId,
        transmissionRef,
        transmissionType: this.toNullableString(data.transmissionType),
        numberOfGears: this.toNullableNumber(data.numberOfGears),
        hasManualOverride: this.toNullableBoolean(data.hasManualOverride),
        hasPaddleShifters: this.toNullableBoolean(data.hasPaddleShifters)
      };

      this.saving.set(true);
      this.service.createTransmission(payload).subscribe({
        next: () => {
          this.saving.set(false);
          this.closeModal();
          this.showSuccess('Transmission added.');
          this.reloadSelectedModel(true);
        },
        error: error => {
          this.saving.set(false);
          this.showError(this.getApiError(error, 'Unable to add transmission.'));
        }
      });
      return;
    }

    if (type === 'drivetrain') {
      const model = this.selectedModel();

      if (!model) {
        this.showError('No staged model is selected.');
        return;
      }

      const drivetrainRef = String(data.drivetrainRef ?? '').trim();
      if (!drivetrainRef) {
        this.showError('Drivetrain Ref is required.');
        return;
      }

      const payload: CreateStagedDrivetrainDto = {
        importModelId: model.importModelId,
        drivetrainRef,
        drivetrainType: this.toNullableString(data.drivetrainType),
        differentialType: this.toNullableString(data.differentialType)
      };

      this.saving.set(true);
      this.service.createDrivetrain(payload).subscribe({
        next: () => {
          this.saving.set(false);
          this.closeModal();
          this.showSuccess('Drivetrain added.');
          this.reloadSelectedModel(true);
        },
        error: error => {
          this.saving.set(false);
          this.showError(this.getApiError(error, 'Unable to add drivetrain.'));
        }
      });
      return;
    }

    this.showError('Unsupported create operation.');
  }

  // ============================================================
  // RELOAD
  // ============================================================

  reloadSelectedModel(
    keepVariant: boolean = false
  ): void {

    const model =
      this.selectedModel();

    const variantId =
      this.selectedVariant()
        ?.importVariantId;

    if (!model) {
      return;
    }

    this.service
      .getModel(
        model.importModelId
      )
      .subscribe({
        
        next: result => {

          result.parentVariants =
            (result.parentVariants ?? [])
              .map(parent => ({

                ...parent,

                subVariants: parent.subVariants ?? []

              }));
              
          this.selectedModel.set(
            result
          );


          if (
            keepVariant &&
            variantId !== undefined
          ) {

            const found =
              this.findVariant(
                result,
                variantId
              );

            if (found) {

              this.selectedVariant.set(
                found
              );

              this.selectedVariantId.set(
                found.importVariantId
              );

            } else {

              this.selectedVariant.set(
                null
              );

              this.selectedVariantId.set(
                null
              );
            }

          }

        },

        error: error => {

          this.showError(
            this.getApiError(
              error,
              'Unable to refresh staged data.'
            )
          );
        }
      });
  }


private findVariant(
  model: StagedModelDetailDto,
  variantId: number
): StagedVariantDetailDto | null {

  for (const parent of model.parentVariants ?? []) {

    const found =
      (parent.subVariants ?? []).find(
        variant =>
          variant.importVariantId === variantId
      );

    if (found) {
      return found;
    }
  }

  return null;
}


  // ============================================================
  // COUNTS
  // ============================================================

  featureCount(
    variant: StagedVariantDetailDto
  ): number {

    return variant.features?.length ?? 0;
  }


  specificationCount(
    variant: StagedVariantDetailDto
  ): number {

    return variant.specifications?.length ?? 0;
  }


  fuelEfficiencyCount(
    variant: StagedVariantDetailDto
  ): number {

    return variant.fuelEfficiencies?.length ?? 0;
  }


  warrantyCount(
    variant: StagedVariantDetailDto
  ): number {

    return variant.warranties?.length ?? 0;
  }


  performanceCount(
    variant: StagedVariantDetailDto
  ): number {

    return (
      variant.enginePerformances?.length ?? 0
    ) +
    (
      variant.powertrain?.motorPerformances?.length ?? 0
    );
  }


  // ============================================================
  // DISPLAY
  // ============================================================

  displayValue(
    value: any
  ): string {

    if (
      value === null ||
      value === undefined ||
      value === ''
    ) {

      return '—';
    }

    if (
      typeof value === 'boolean'
    ) {

      return value
        ? 'Yes'
        : 'No';
    }

    return String(value);
  }


  formatNumber(
    value: number | null
  ): string {

    if (
      value === null ||
      value === undefined
    ) {

      return '—';
    }

    return value.toLocaleString(
      'en-IN'
    );
  }


  formatConfidence(
    value: number | null
  ): string {

    if (
      value === null ||
      value === undefined
    ) {

      return '—';
    }

    return `${Math.round(value * 100)}%`;
  }


  // ============================================================
  // ALERTS
  // ============================================================

  showSuccess(
    message: string
  ): void {

    this.alertType.set(
      'success'
    );

    this.alertMessage.set(
      message
    );

    setTimeout(() => {

      this.alertMessage.set('');

    }, 3500);
  }


  showError(
    message: string
  ): void {

    this.alertType.set(
      'error'
    );

    this.alertMessage.set(
      message
    );

    setTimeout(() => {

      this.alertMessage.set('');

    }, 5000);
  }


  getApiError(
    error: any,
    fallback: string
  ): string {

    return (
      error?.error?.message ??
      error?.error?.title ??
      error?.message ??
      fallback
    );
  }

  getDimensionValue(
  key:
    | 'lengthMm'
    | 'widthMm'
    | 'heightMm'
    | 'wheelbaseMm'
    | 'groundClearanceMm'
    | 'bootSpaceLitres'
    | 'fuelTankCapacityLitres'
): number | null | undefined {

  const dimensions =
    this.selectedModel()?.dimensions;

  if (!dimensions) {
    return null;
  }

  return dimensions[key];
}


saveFeatureField(
  feature: StagedFeatureDto,
  field: keyof UpdateStagedFeatureDto
): void {

  const payload: UpdateStagedFeatureDto = {
    available: feature.available,
    value: feature.value,

    valueOptionIds:
      feature.valueOptions?.map(
        option => option.valueOptionId
      ) ?? [],

    sourceColumn: feature.sourceColumn,
    pageNumber: feature.pageNumber,
    evidence: feature.evidence,
    confidence: feature.confidence
  };

  let value = this.editValue();

  if (field === 'valueOptionIds') {

    value = String(value ?? '')
      .split(',')
      .map(x => Number(x.trim()))
      .filter(
        x =>
          Number.isInteger(x) &&
          x > 0
      );
  }

  if (
    field === 'available'
  ) {
    if (
      value === true ||
      value === 'true'
    ) {
      value = true;
    }
    else if (
      value === false ||
      value === 'false'
    ) {
      value = false;
    }
    else {
      value = null;
    }
  }

  if (
    field === 'pageNumber' ||
    field === 'confidence'
  ) {
    value =
      value === '' ||
      value === null ||
      value === undefined
        ? null
        : Number(value);
  }

  (payload as any)[field] = value;

  this.saveSimpleUpdate(
    this.service.updateFeatureVariant(
      feature.importFeatureVariantId,
      payload
    ),
    'Feature updated.'
  );
}


saveSpecificationField(
  specification: StagedSpecificationDto,
  field: keyof UpdateStagedSpecificationDto
): void {

  const payload: UpdateStagedSpecificationDto = {
    numericValue:
      specification.numericValue,

    textValue:
      specification.textValue,

    booleanValue:
      specification.booleanValue,

    unit:
      specification.unit,

    sourceColumn:
      specification.sourceColumn,

    pageNumber:
      specification.pageNumber,

    evidence:
      specification.evidence,

    confidence:
      specification.confidence
  };

  (payload as any)[field] =
    this.normalizeEditValue(
      field,
      this.editValue()
    );

  this.saveSimpleUpdate(
    this.service.updateSpecificationVariant(
      specification.importSpecificationVariantId,
      payload
    ),
    'Specification updated.'
  );
}


saveFuelEfficiencyField(
  efficiency: StagedFuelEfficiencyDto,
  field: keyof UpdateStagedFuelEfficiencyDto
): void {

  const payload: UpdateStagedFuelEfficiencyDto = {
    fuelEfficiency:
      efficiency.fuelEfficiency,

    fuelEfficiencyUnit:
      efficiency.fuelEfficiencyUnit,

    sourceColumn:
      efficiency.sourceColumn,

    pageNumber:
      efficiency.pageNumber,

    evidence:
      efficiency.evidence,

    confidence:
      efficiency.confidence
  };

  (payload as any)[field] =
    this.normalizeEditValue(
      field,
      this.editValue()
    );

  this.saveSimpleUpdate(
    this.service.updateFuelEfficiency(
      efficiency.importFuelEfficiencyId,
      payload
    ),
    'Fuel efficiency updated.'
  );
}
  savePowertrainField(
    powertrain: StagedPowertrainDto,
    field: keyof UpdateStagedPowertrainDto
  ): void {

    const payload: UpdateStagedPowertrainDto = {
      powertrainType: powertrain.powertrainType,
      engineRef: powertrain.engineRef,

      batteryCapacityKWh:
        powertrain.batteryCapacityKWh,

      combinedMaxPower:
        powertrain.combinedMaxPower,

      combinedMaxPowerUnit:
        powertrain.combinedMaxPowerUnit,

      combinedMaxTorque:
        powertrain.combinedMaxTorque,

      combinedMaxTorqueUnit:
        powertrain.combinedMaxTorqueUnit,

      combinedMaxPowerRPM:
        powertrain.combinedMaxPowerRPM,

      combinedMaxPowerRPMMin:
        powertrain.combinedMaxPowerRPMMin,

      combinedMaxPowerRPMMax:
        powertrain.combinedMaxPowerRPMMax,

      combinedMaxTorqueRPM:
        powertrain.combinedMaxTorqueRPM,

      combinedMaxTorqueRPMMin:
        powertrain.combinedMaxTorqueRPMMin,

      combinedMaxTorqueRPMMax:
        powertrain.combinedMaxTorqueRPMMax
    };

    (payload as any)[field] =
      this.normalizeEditValue(
        field,
        this.editValue()
      );

    this.saveSimpleUpdate(
      this.service.updatePowertrain(
        powertrain.importPowertrainId,
        payload
      ),
      'Powertrain updated.'
    );
  }


  saveTransmissionField(
    transmission: StagedTransmissionDto,
    field: keyof UpdateStagedTransmissionDto
  ): void {

    const payload: UpdateStagedTransmissionDto = {
      transmissionType:
        transmission.transmissionType,

      numberOfGears:
        transmission.numberOfGears,

      hasManualOverride:
        transmission.hasManualOverride,

      hasPaddleShifters:
        transmission.hasPaddleShifters
    };

    (payload as any)[field] =
      this.normalizeEditValue(
        field,
        this.editValue()
      );

    this.saveSimpleUpdate(
      this.service.updateTransmission(
        transmission.importTransmissionId,
        payload
      ),
      'Transmission updated.'
    );
  }


  saveDrivetrainField(
    drivetrain: StagedDrivetrainDto,
    field: keyof UpdateStagedDrivetrainDto
  ): void {

    const payload: UpdateStagedDrivetrainDto = {
      drivetrainType:
        drivetrain.drivetrainType,

      differentialType:
        drivetrain.differentialType
    };

    (payload as any)[field] =
      this.normalizeEditValue(
        field,
        this.editValue()
      );

    this.saveSimpleUpdate(
      this.service.updateDrivetrain(
        drivetrain.importDrivetrainId,
        payload
      ),
      'Drivetrain updated.'
    );
  }


  private saveSimpleUpdate(
    request: Observable<void>,
    successMessage: string
  ): void {

    this.saving.set(true);

    const keepVariant =
      this.selectedVariant() !== null;

    request.subscribe({

      next: () => {

        this.saving.set(false);

        this.cancelEdit();

        this.showSuccess(
          successMessage
        );

        this.reloadSelectedModel(
          keepVariant
        );
      },

      error: error => {

        this.saving.set(false);

        this.showError(
          this.getApiError(
            error,
            'Unable to save staged data.'
          )
        );
      }
    });
  }


  saveWarrantyField(
    warranty: StagedWarrantyDto,
    field: keyof UpdateStagedWarrantyDto
  ): void {

    const payload: UpdateStagedWarrantyDto = {
      warrantyType:
        warranty.warrantyType,

      durationYears:
        warranty.durationYears,

      kilometres:
        warranty.kilometres,

      maximumDurationYears:
        warranty.maximumDurationYears,

      maximumKilometres:
        warranty.maximumKilometres,

      sourceId:
        warranty.sourceId,

      evidenceText:
        warranty.evidenceText,

      pageNumber:
        warranty.pageNumber,

      confidence:
        warranty.confidence
    };

    (payload as any)[field] =
      this.normalizeEditValue(
        field,
        this.editValue()
      );

    this.saveSimpleUpdate(
      this.service.updateWarranty(
        warranty.importWarrantyId,
        payload
      ),
      'Warranty updated.'
    );
  }


  openPowertrainReferenceModal(): void {
    const variant = this.selectedVariant();
    const model = this.selectedModel();

    if (!variant || !model) {
      this.showError(
        'Select a variant before changing its powertrain reference.'
      );
      return;
    }

    this.modalTitle.set('Reference Powertrain');
    this.modalType.set('referencePowertrain');

    this.modalData.set({
      importModelId: model.importModelId,
      importVariantId: variant.importVariantId,
      importPowertrainId: null
    });

    this.showModal.set(true);
    this.loadPowertrainsForReference(model.importModelId);
  }


  openTransmissionReferenceModal(): void {
    const variant = this.selectedVariant();
    const model = this.selectedModel();

    if (!variant || !model) {
      this.showError(
        'Select a variant before changing its transmission reference.'
      );
      return;
    }

    this.modalTitle.set('Reference Transmission');
    this.modalType.set('referenceTransmission');

    this.modalData.set({
      importModelId: model.importModelId,
      importVariantId: variant.importVariantId,
      importTransmissionId: null
    });

    this.showModal.set(true);
    this.loadTransmissionsForReference(model.importModelId);
  }


  openDrivetrainReferenceModal(): void {
    const variant = this.selectedVariant();
    const model = this.selectedModel();

    if (!variant || !model) {
      this.showError(
        'Select a variant before changing its drivetrain reference.'
      );
      return;
    }

    this.modalTitle.set('Reference Drivetrain');
    this.modalType.set('referenceDrivetrain');

    this.modalData.set({
      importModelId: model.importModelId,
      importVariantId: variant.importVariantId,
      importDrivetrainId: null
    });

    this.showModal.set(true);
    this.loadDrivetrainsForReference(model.importModelId);
  }


  private loadPowertrainsForReference(
    importModelId: number
  ): void {
    this.powertrainReferenceLoading.set(true);

    this.service
      .getPowertrainsForModel(importModelId)
      .subscribe({
        next: result => {
          this.existingPowertrains.set(
            result ?? []
          );

          this.powertrainReferenceLoading.set(false);
        },
        error: error => {
          this.powertrainReferenceLoading.set(false);

          this.showError(
            this.getApiError(
              error,
              'Unable to load existing powertrains.'
            )
          );
        }
      });
  }


  private loadTransmissionsForReference(
    importModelId: number
  ): void {
    this.transmissionReferenceLoading.set(true);

    this.service
      .getTransmissionsForModel(importModelId)
      .subscribe({
        next: result => {
          this.existingTransmissions.set(
            result ?? []
          );

          this.transmissionReferenceLoading.set(false);
        },
        error: error => {
          this.transmissionReferenceLoading.set(false);

          this.showError(
            this.getApiError(
              error,
              'Unable to load existing transmissions.'
            )
          );
        }
      });
  }


  private loadDrivetrainsForReference(
    importModelId: number
  ): void {
    this.drivetrainReferenceLoading.set(true);

    this.service
      .getDrivetrainsForModel(importModelId)
      .subscribe({
        next: result => {
          this.existingDrivetrains.set(
            result ?? []
          );

          this.drivetrainReferenceLoading.set(false);
        },
        error: error => {
          this.drivetrainReferenceLoading.set(false);

          this.showError(
            this.getApiError(
              error,
              'Unable to load existing drivetrains.'
            )
          );
        }
      });
  }


 openPowertrainModal(): void {
  this.modalTitle.set('Add Powertrain');
  this.modalType.set('powertrain');

  this.modalData.set({
    importModelId: this.selectedModel()?.importModelId ?? null,

    powertrainRef: '',
    powertrainType: 'ICE',
    engineRef: null,

    batteryCapacityKWh: null,
    combinedMaxPower: null,
    combinedMaxTorque: null,

    combinedMaxPowerRPM: null,
    combinedMaxPowerRPMMin: null,
    combinedMaxPowerRPMMax: null,

    combinedMaxTorqueRPM: null,
    combinedMaxTorqueRPMMin: null,
    combinedMaxTorqueRPMMax: null,

    engineMode: 'none',
    existingEngineId: null,
    newEngine: {
      engineRef: '',
      engineName: '',
      numberOfCylinders: null,
      numberOfValves: null,
      displacement: null,
      isTurbocharged: null,
      emissionStandard: null,
      aspiration: null,
      engineType: null
    },

    enginePerformanceMode: 'none',
    existingEnginePerformanceId: null,
    newEnginePerformance: {
      modeName: '',
      fuelTypeId: null,
      maxPower: null,
      maxTorque: null,
      maxPowerRPM: null,
      maxPowerRPMMin: null,
      maxPowerRPMMax: null,
      maxTorqueRPM: null,
      maxTorqueRPMMin: null,
      maxTorqueRPMMax: null
    },

    existingMotorPerformanceIds: [],
    newMotorPerformances: []
  });

  this.showModal.set(true);

  this.loadPowertrainDependencies();
}


toggleExistingMotorPerformance(id: number): void {
  const current =
    this.modalData().existingMotorPerformanceIds ?? [];

  const next = current.includes(id)
    ? current.filter((x : number) => x !== id)
    : [...current, id];

  this.setModalField(
    'existingMotorPerformanceIds',
    next
  );
}


openTransmissionModal(): void {

  const model =
    this.selectedModel();

  if (!model) {
    return;
  }

  this.modalType.set(
    'transmission'
  );

  this.modalTitle.set(
    'Add Transmission'
  );

  this.modalData.set({

    importModelId:
      model.importModelId,

    transmissionRef:
      '',

    transmissionType:
      '',

    numberOfGears:
      null,

    hasManualOverride:
      null,

    hasPaddleShifters:
      null
  });
}


openDrivetrainModal(): void {

  const model =
    this.selectedModel();

  if (!model) {
    return;
  }

  this.modalType.set(
    'drivetrain'
  );

  this.modalTitle.set(
    'Add Drivetrain'
  );

  this.modalData.set({

    importModelId:
      model.importModelId,

    drivetrainRef:
      '',

    drivetrainType:
      '',

    differentialType:
      ''
  });
}

private loadPowertrainDependencies(): void {
  this.powertrainDependencyLoading.set(true);

  this.service.getPowertrainDependencies().subscribe({
    next: result => {
      this.existingEngines.set(result.engines ?? []);
      this.existingEnginePerformances.set(
        result.enginePerformances ?? []
      );
      this.existingMotorPerformances.set(
        result.motorPerformances ?? []
      );

      this.powertrainDependencyLoading.set(false);
    },

    error: error => {
      this.powertrainDependencyLoading.set(false);

      this.showError(
        this.getApiError(
          error,
          'Unable to load existing powertrain dependencies.'
        )
      );
    }
  });
}

private loadFeatureSpecificationCatalog(): void {
  this.featureSpecificationCatalogLoading.set(true);

  this.service.getFeatureSpecificationCatalog().subscribe({
    next: result => {
      this.featureCatalog.set(result.features ?? []);
      this.specificationCatalog.set(result.specifications ?? []);
      this.featureSpecificationCatalogLoading.set(false);
    },
    error: error => {
      this.featureSpecificationCatalogLoading.set(false);

      this.showError(
        this.getApiError(
          error,
          'Unable to load feature and specification catalog.'
        )
      );
    }
  });
}

selectFeature(code: string): void {
  const feature = this.featureCatalog().find(
    x => x.featureCode === code
  );

  this.setModalField('featureCode', code);
  this.setModalField(
    'importFeatureId',
    feature?.importFeatureId ?? null
  );
}

selectSpecification(code: string): void {
  const specification = this.specificationCatalog().find(
    x => x.specificationCode === code
  );

  this.setModalField('specificationCode', code);
  this.setModalField(
    'importSpecificationId',
    specification?.importSpecificationId ?? null
  );
}

}