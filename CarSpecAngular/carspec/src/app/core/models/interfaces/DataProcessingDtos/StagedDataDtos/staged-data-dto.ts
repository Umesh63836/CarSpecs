/* ============================================================
   STAGED MODEL LIST
   ============================================================ */

export interface StagedModelListDto {
  importModelId: number;
  brandName: string;
  modelName: string;
  category: string | null;
  bodyType: string | null;
  modelImageUrl: string | null;

  importBatchId: number;
  stagedAt: string | null;

  parentVariantCount: number;
  variantCount: number;
  warningCount: number;
}

export interface PaginatedStagedModelResponseDto {
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;

  hasPreviousPage: boolean;
  hasNextPage: boolean;

  models: StagedModelListDto[];
}


/* ============================================================
   MODEL DETAIL
   ============================================================ */

export interface StagedModelDetailDto {
  importModelId: number;
  importRecordId: number;

  brandName: string | null;
  modelName: string | null;
  category: string | null;
  bodyType: string | null;
  modelImageUrl: string | null;

  importBatchId: number;
  stagedAt: string | null;

  dimensions: StagedModelDimensionsDto | null;

  powertrains: StagedPowertrainDto[];
  transmissions: StagedTransmissionDto[];
  drivetrains: StagedDrivetrainDto[];

  parentVariants: StagedVariantParentDto[];
  subvariants: StagedVariantDetailDto[];

  features: StagedFeatureDto[];
  specifications: StagedSpecificationDto[];

  fuelEfficiencies: StagedFuelEfficiencyDto[];
  warranties: StagedWarrantyDto[];

  warnings: StagedWarningDto[];
}


/* ============================================================
   DIMENSIONS
   ============================================================ */

export interface StagedModelDimensionsDto {
  importModelDimensionId: number;
  importModelId: number;

  lengthMm: number | null;
  widthMm: number | null;
  heightMm: number | null;
  wheelbaseMm: number | null;
  groundClearanceMm: number | null;
  bootSpaceLitres: number | null;
  fuelTankCapacityLitres: number | null;

  sourceId: number | null;
  evidenceText: string | null;
  pageNumber: number | null;
  confidence: number | null;
}



/* ============================================================
   VARIANT
   ============================================================ */

export interface StagedVariantDetailDto {
  importVariantId: number;

  variantName: string;
  variantType: string;

  baseVariantName: string | null;

  powertrainRef: string | null;
  transmissionRef: string | null;
  drivetrainRef: string | null;

  exShowroomPrice: number | null;
  kerbWeight: number | null;
  seatingCapacity: number | null;

  powertrain: StagedPowertrainDto | null;
  transmission: StagedTransmissionDto | null;
  drivetrain: StagedDrivetrainDto | null;

  engine: StagedEngineDto | null;

  features: StagedFeatureDto[];
  specifications: StagedSpecificationDto[];

  fuelEfficiencies: StagedFuelEfficiencyDto[];
  warranties: StagedWarrantyDto[];

  parentVariantName: string | null;
}


/* ============================================================
   POWERTRAIN
   ============================================================ */

export interface StagedPowertrainDto {
  importPowertrainId: number;

  powertrainRef: string;
  powertrainType: string;

  engineRef: string | null;

  batteryCapacityKWh: number | null;

  combinedMaxPower: number | null;
  combinedMaxPowerUnit: string | null;

  combinedMaxTorque: number | null;
  combinedMaxTorqueUnit: string | null;

  combinedMaxPowerRPM: number | null;
  combinedMaxPowerRPMMin: number | null;
  combinedMaxPowerRPMMax: number | null;

  combinedMaxTorqueRPM: number | null;
  combinedMaxTorqueRPMMin: number | null;
  combinedMaxTorqueRPMMax: number | null;

  motorPerformances: StagedMotorPerformanceDto[];
}


/* ============================================================
   ENGINE
   ============================================================ */

export interface StagedEngineDto {
  importEngineId: number;

  engineRef: string;
  engineName: string;

  numberOfCylinders: number | null;
  numberOfValves: number | null;

  displacement: number | null;

  isTurbocharged: boolean | null;

  emissionStandard: string | null;
  aspiration: string | null;
  engineType: string | null;

  enginePerformances: StagedEnginePerformanceDto[];
}


/* ============================================================
   ENGINE PERFORMANCE
   ============================================================ */

export interface StagedEnginePerformanceDto {
  importEnginePerformanceId: number;

  modeName: string;

  fuelTypeId: number | null;
  fuelTypeName: string | null;

  maxPower: number | null;
  maxPowerUnit: string | null;

  maxTorque: number | null;
  maxTorqueUnit: string | null;

  maxPowerRPM: number | null;
  maxPowerRPMMin: number | null;
  maxPowerRPMMax: number | null;

  maxTorqueRPM: number | null;
  maxTorqueRPMMin: number | null;
  maxTorqueRPMMax: number | null;
}


/* ============================================================
   MOTOR PERFORMANCE
   ============================================================ */

export interface StagedMotorPerformanceDto {
  importMotorPerformanceId: number;

  motorName: string | null;

  maxPower: number | null;
  maxPowerUnit: string | null;

  maxTorque: number | null;
  maxTorqueUnit: string | null;

  maxPowerRPM: number | null;
  maxPowerRPMMin: number | null;
  maxPowerRPMMax: number | null;

  maxTorqueRPM: number | null;
  maxTorqueRPMMin: number | null;
  maxTorqueRPMMax: number | null;
}


/* ============================================================
   TRANSMISSION
   ============================================================ */

export interface StagedTransmissionDto {
  importTransmissionId: number;

  transmissionRef: string;
  transmissionType: string | null;

  numberOfGears: number | null;

  hasManualOverride: boolean | null;
  hasPaddleShifters: boolean | null;
}


/* ============================================================
   DRIVETRAIN
   ============================================================ */

export interface StagedDrivetrainDto {
  importDrivetrainId: number;

  drivetrainRef: string;

  drivetrainType: string | null;
  differentialType: string | null;
}


/* ============================================================
   FEATURE
   ============================================================ */

export interface StagedFeatureDto {
  importFeatureVariantId: number;

  importFeatureId: number;

  featureId: number | null;
  featureCode: string;

  featureName: string | null;
  valueType: string | null;
  isMultiValue: boolean | null;

  available: boolean | null;
  value: string | null;

  valueOptionIds: number[];

  valueOptions: StagedFeatureValueOptionDto[];

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;
}

export interface StagedFeatureValueOptionDto {
  valueOptionId: number;
  value: string;
  displayName: string;
}


/* ============================================================
   SPECIFICATION
   ============================================================ */

export interface StagedSpecificationDto {
  importSpecificationVariantId: number;

  importSpecificationId: number;

  specificationId: number | null;
  specificationCode: string;

  specificationName: string | null;
  dataType: string | null;
  masterUnit: string | null;

  numericValue: number | null;
  textValue: string | null;
  booleanValue: boolean | null;
  unit: string | null;

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;
}


/* ============================================================
   FUEL EFFICIENCY
   ============================================================ */

export interface StagedFuelEfficiencyDto {
  importFuelEfficiencyId: number;

  fuelEfficiency: number;
  fuelEfficiencyUnit: string;

  sourceType: string | null;
  isActive: boolean;

  sourceId: number | null;

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;

  importVariantIds: number[];
}


/* ============================================================
   WARRANTY
   ============================================================ */

export interface StagedWarrantyDto {
  importWarrantyId: number;

  importModelId: number;
  importVariantId: number | null;

  warrantyType: string;

  durationYears: number | null;
  kilometres: number | null;

  maximumDurationYears: number | null;
  maximumKilometres: number | null;

  sourceId: number | null;
  evidenceText: string | null;
  pageNumber: number | null;
  confidence: number | null;
}


/* ============================================================
   WARNING
   ============================================================ */

export interface StagedWarningDto {
  importWarningId: number;

  importRecordId: number | null;

  warningText: string;
  status: string;

  reviewedBy: number | null;
  reviewedAt: string | null;

  notes: string | null;
}


/* ============================================================
   UPDATE DTOs
   ============================================================ */

export interface UpdateStagedModelDto {
  brandName?: string | null;
  modelName?: string | null;
  category?: string | null;
  bodyType?: string | null;
  modelImageUrl?: string | null;

  dimensions: UpdateStagedModelDimensionsDto;
}

export interface UpdateStagedModelDimensionsDto {
  lengthMm: number | null;
  widthMm: number | null;
  heightMm: number | null;
  wheelbaseMm: number | null;
  groundClearanceMm: number | null;
  bootSpaceLitres: number | null;
  fuelTankCapacityLitres: number | null;

  sourceId: number | null;
  evidenceText: string | null;
  pageNumber: number | null;
  confidence: number | null;
}

export interface UpdateStagedVariantDto {
  variantName?: string | null;
  baseVariantName?: string | null;

  exShowroomPrice: number | null;
  kerbWeight: number | null;
  seatingCapacity: number | null;

  powertrainRef: string | null;
  transmissionRef: string | null;
  drivetrainRef: string | null;
}



export interface UpdateStagedEngineDto {
  engineName: string | null;
  numberOfCylinders: number | null;
  numberOfValves: number | null;
  displacement: number | null;
  isTurbocharged: boolean | null;
  emissionStandard: string | null;
  aspiration: string | null;
  engineType: string | null;
}

export interface UpdateStagedEnginePerformanceDto {
  modeName: string | null;
  fuelTypeName: string | null;

  maxPower: number | null;
  maxPowerUnit: string | null;

  maxTorque: number | null;
  maxTorqueUnit: string | null;

  maxPowerRPM: number | null;
  maxPowerRPMMin: number | null;
  maxPowerRPMMax: number | null;

  maxTorqueRPM: number | null;
  maxTorqueRPMMin: number | null;
  maxTorqueRPMMax: number | null;
}

export interface UpdateStagedMotorPerformanceDto {
  motorName: string | null;

  maxPower: number | null;
  maxPowerUnit: string | null;

  maxTorque: number | null;
  maxTorqueUnit: string | null;

  maxPowerRPM: number | null;
  maxPowerRPMMin: number | null;
  maxPowerRPMMax: number | null;

  maxTorqueRPM: number | null;
  maxTorqueRPMMin: number | null;
  maxTorqueRPMMax: number | null;
}

export interface UpdateStagedFeatureDto {
  available: boolean | null;
  value: string | null;
  valueOptionIds: number[];

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;
}

export interface UpdateStagedPowertrainDto {
  powertrainType: string | null;
  engineRef: string | null;
  batteryCapacityKWh: number | null;
  combinedMaxPower: number | null;
  combinedMaxPowerUnit: string | null;
  combinedMaxTorque: number | null;
  combinedMaxTorqueUnit: string | null;
  combinedMaxPowerRPM: number | null;
  combinedMaxPowerRPMMin: number | null;
  combinedMaxPowerRPMMax: number | null;
  combinedMaxTorqueRPM: number | null;
  combinedMaxTorqueRPMMin: number | null;
  combinedMaxTorqueRPMMax: number | null;
}

export interface UpdateStagedTransmissionDto {
  transmissionType: string | null;
  numberOfGears: number | null;
  hasManualOverride: boolean | null;
  hasPaddleShifters: boolean | null;
}

export interface UpdateStagedDrivetrainDto {
  drivetrainType: string | null;
  differentialType: string | null;
}

export interface UpdateStagedSpecificationDto {
  numericValue: number | null;
  textValue: string | null;
  booleanValue: boolean | null;
  unit: string | null;

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;
}

export interface UpdateStagedFuelEfficiencyDto {
  fuelEfficiency: number;
  fuelEfficiencyUnit: string;

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;
}

export interface UpdateStagedWarrantyDto {
  warrantyType: string | null;

  durationYears: number | null;
  kilometres: number | null;

  maximumDurationYears: number | null;
  maximumKilometres: number | null;

  sourceId: number | null;
  evidenceText: string | null;
  pageNumber: number | null;
  confidence: number | null;
}


/* ============================================================
   CREATE DTOs
   ============================================================ */

export interface CreateStagedSpecificationDto {
  importModelId: number;
  importVariantId: number;
  importSpecificationId: number | null;
  specificationCode: string;
  numericValue: number | null;
  textValue: string | null;
  booleanValue: boolean | null;
  unit: string | null;
  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;
}


export interface CreateStagedModelDimensionsDto {
  importModelId: number;

  lengthMM: number | null;
  widthMM: number | null;
  heightMM: number | null;
  wheelbaseMM: number | null;
  groundClearanceMM: number | null;
  bootSpaceLitres: number | null;
  fuelTankCapacityLitres: number | null;

  sourceId: number | null;
  evidenceText: string | null;
  pageNumber: number | null;
  confidence: number | null;
}


export interface CreateStagedFuelEfficiencyDto {
  importModelId: number;

  fuelEfficiency: number;
  fuelEfficiencyUnit: string;

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;

  importVariantIds: number[];
}

export interface CreateStagedWarrantyDto {
  importModelId: number;
  importVariantId: number | null;

  warrantyType: string;

  durationYears: number | null;
  kilometres: number | null;

  maximumDurationYears: number | null;
  maximumKilometres: number | null;

  sourceId: number | null;
  evidenceText: string | null;
  pageNumber: number | null;
  confidence: number | null;
}

export interface StagedVariantDetailDto {

  importVariantId: number;

  variantName: string;
  variantType: string;

  baseVariantName: string | null;

  powertrainRef: string | null;
  transmissionRef: string | null;
  drivetrainRef: string | null;

  exShowroomPrice: number | null;
  kerbWeight: number | null;
  seatingCapacity: number | null;

  powertrain: StagedPowertrainDto | null;
  transmission: StagedTransmissionDto | null;
  drivetrain: StagedDrivetrainDto | null;

  engine: StagedEngineDto | null;

  enginePerformances: StagedEnginePerformanceDto[];

  features: StagedFeatureDto[];
  specifications: StagedSpecificationDto[];

  fuelEfficiencies: StagedFuelEfficiencyDto[];
  warranties: StagedWarrantyDto[];

  parentVariantName: string | null;
}

export interface StagedVariantParentDto {

  importVariantParentId: number;

  importModelId: number;

  parentVariantName: string;

  subVariants: StagedVariantDetailDto[];
}

export interface UpdateStagedPowertrainDto {
  powertrainType: string | null;
  engineRef: string | null;

  batteryCapacityKWh: number | null;

  combinedMaxPower: number | null;
  combinedMaxPowerUnit: string | null;

  combinedMaxTorque: number | null;
  combinedMaxTorqueUnit: string | null;

  combinedMaxPowerRPM: number | null;
  combinedMaxPowerRPMMin: number | null;
  combinedMaxPowerRPMMax: number | null;

  combinedMaxTorqueRPM: number | null;
  combinedMaxTorqueRPMMin: number | null;
  combinedMaxTorqueRPMMax: number | null;
}


export interface UpdateStagedTransmissionDto {
  transmissionType: string | null;
  numberOfGears: number | null;
  hasManualOverride: boolean | null;
  hasPaddleShifters: boolean | null;
}


export interface UpdateStagedDrivetrainDto {
  drivetrainType: string | null;
  differentialType: string | null;
}


export interface CreateStagedSpecificationDto {
  importModelId: number;
  importVariantId: number;
  importSpecificationId: number | null;
  specificationCode: string;

  numericValue: number | null;
  textValue: string | null;
  booleanValue: boolean | null;

  unit: string | null;

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;
}


export interface CreateStagedFeatureDto {
  importModelId: number;
  importVariantId: number;
  importFeatureId: number | null;
  featureCode: string;

  available: boolean | null;
  value: string | null;
  valueOptionIds: number[];

  sourceColumn: string | null;
  pageNumber: number | null;
  evidence: string | null;
  confidence: number | null;
}

export interface CreateStagedEngineDto {
  engineRef: string;
  engineName: string;

  numberOfCylinders?: number | null;
  numberOfValves?: number | null;
  displacement?: number | null;
  isTurbocharged?: boolean | null;

  emissionStandard?: string | null;
  aspiration?: string | null;
  engineType?: string | null;
}

export interface CreateStagedEnginePerformanceDto {
  modeName: string;

  fuelTypeId?: number | null;

  maxPower?: number | null;
  maxTorque?: number | null;

  maxPowerRPM?: number | null;
  maxPowerRPMMin?: number | null;
  maxPowerRPMMax?: number | null;

  maxTorqueRPM?: number | null;
  maxTorqueRPMMin?: number | null;
  maxTorqueRPMMax?: number | null;
}

export interface CreateStagedMotorPerformanceDto {
  motorName?: string | null;

  maxPower?: number | null;
  maxTorque?: number | null;

  maxPowerRPM?: number | null;
  maxPowerRPMMin?: number | null;
  maxPowerRPMMax?: number | null;

  maxTorqueRPM?: number | null;
  maxTorqueRPMMin?: number | null;
  maxTorqueRPMMax?: number | null;
}

export interface CreateStagedPowertrainDto {

  importModelId: number;

  // Admin supplied reference.
  powertrainRef: string;

  powertrainType: string;

  engineRef?: string | null;

  batteryCapacityKWh?: number | null;

  combinedMaxPower?: number | null;
  combinedMaxTorque?: number | null;

  combinedMaxPowerRPM?: number | null;
  combinedMaxPowerRPMMin?: number | null;
  combinedMaxPowerRPMMax?: number | null;

  combinedMaxTorqueRPM?: number | null;
  combinedMaxTorqueRPMMin?: number | null;
  combinedMaxTorqueRPMMax?: number | null;


  // ------------------------------------------------------------
  // ENGINE
  // ------------------------------------------------------------

  engineMode: StagedDependencyMode;

  /*
   * When engineMode === 'existing',
   * this is the existing staged ImportEngineId.
   *
   * Backend clones the existing engine data into the
   * newly created powertrain.
   */
  existingEngineId?: number | null;

  /*
   * When engineMode === 'new'.
   */
  newEngine?: CreateStagedEngineDto | null;


  // ------------------------------------------------------------
  // ENGINE PERFORMANCE
  // ------------------------------------------------------------

  enginePerformanceMode: StagedDependencyMode;

  /*
   * Existing staged performance to use as template.
   */
  existingEnginePerformanceId?: number | null;

  /*
   * New performance to create.
   */
  newEnginePerformance?: CreateStagedEnginePerformanceDto | null;


  // ------------------------------------------------------------
  // MOTOR PERFORMANCE
  // ------------------------------------------------------------

  /*
   * Multiple motors can exist on one powertrain.
   *
   * Existing IDs are cloned into this new powertrain.
   */
  existingMotorPerformanceIds?: number[];

  /*
   * New motor performances to create.
   */
  newMotorPerformances?: CreateStagedMotorPerformanceDto[];
}

export type StagedDependencyMode =
  | 'none'
  | 'existing'
  | 'new';



export interface CreateStagedTransmissionDto {

  importModelId: number;

  transmissionRef: string;

  transmissionType?: string | null;

  numberOfGears?: number | null;

  hasManualOverride?: boolean | null;

  hasPaddleShifters?: boolean | null;
}


export interface CreateStagedDrivetrainDto {

  importModelId: number;

  drivetrainRef: string;

  drivetrainType?: string | null;

  differentialType?: string | null;
}

export interface StagedEngineLookupDto {
  importEngineId: number;
  importPowertrainId: number;

  engineRef: string;
  engineName: string;

  numberOfCylinders?: number | null;
  numberOfValves?: number | null;
  displacement?: number | null;
  isTurbocharged?: boolean | null;

  emissionStandard?: string | null;
  aspiration?: string | null;
  engineType?: string | null;

  productionEngineId?: number | null;
}


export interface StagedEnginePerformanceLookupDto {
  importEnginePerformanceId: number;
  importEngineId: number;

  engineRef?: string | null;
  engineName?: string | null;

  modeName: string;

  fuelTypeId?: number | null;
  fuelTypeName?: string | null;

  maxPower?: number | null;
  maxTorque?: number | null;

  maxPowerRPM?: number | null;
  maxPowerRPMMin?: number | null;
  maxPowerRPMMax?: number | null;

  maxTorqueRPM?: number | null;
  maxTorqueRPMMin?: number | null;
  maxTorqueRPMMax?: number | null;

  productionEnginePerformanceId?: number | null;
}


export interface StagedMotorPerformanceLookupDto {
  importMotorPerformanceId: number;
  importPowertrainId: number;

  motorName?: string | null;

  maxPower?: number | null;
  maxTorque?: number | null;

  maxPowerRPM?: number | null;
  maxPowerRPMMin?: number | null;
  maxPowerRPMMax?: number | null;

  maxTorqueRPM?: number | null;
  maxTorqueRPMMin?: number | null;
  maxTorqueRPMMax?: number | null;

  productionMotorPerformanceId?: number | null;
}


export interface StagedPowertrainDependenciesDto {
  engines: StagedEngineLookupDto[];
  enginePerformances: StagedEnginePerformanceLookupDto[];
  motorPerformances: StagedMotorPerformanceLookupDto[];
}

export interface StagedFeatureCatalogDto {
  importFeatureId: number;
  featureCode: string;
  featureName?: string | null;
}

export interface StagedSpecificationCatalogDto {
  importSpecificationId: number;
  specificationCode: string;
  specificationName?: string | null;
}

export interface StagedFeatureSpecificationCatalogDto {
  features: StagedFeatureCatalogDto[];
  specifications: StagedSpecificationCatalogDto[];
}

export interface StagedPowertrainLookupDto {
  importPowertrainId: number;
  importModelId: number;

  powertrainRef: string;
  powertrainType: string;

  engineRef?: string | null;

  batteryCapacityKWh?: number | null;

  combinedMaxPower?: number | null;
  combinedMaxPowerUnit?: string | null;

  combinedMaxTorque?: number | null;
  combinedMaxTorqueUnit?: string | null;

  combinedMaxPowerRPM?: number | null;
  combinedMaxPowerRPMMin?: number | null;
  combinedMaxPowerRPMMax?: number | null;

  combinedMaxTorqueRPM?: number | null;
  combinedMaxTorqueRPMMin?: number | null;
  combinedMaxTorqueRPMMax?: number | null;

  productionPowertrainId?: number | null;
}

export interface StagedDrivetrainLookupDto {
  importDrivetrainId: number;
  importModelId: number;

  drivetrainRef: string;
  drivetrainType?: string | null;
  differentialType?: string | null;

  productionDrivetrainId?: number | null;
}

export interface StagedTransmissionLookupDto {
  importTransmissionId: number;
  importModelId: number;

  transmissionRef: string;
  transmissionType?: string | null;

  numberOfGears?: number | null;

  hasManualOverride?: boolean | null;
  hasPaddleShifters?: boolean | null;

  productionTransmissionId?: number | null;
}

export interface ReplaceStagedPowertrainReferenceDto {
  importPowertrainId: number;
}

export interface ReplaceStagedTransmissionReferenceDto {
  importTransmissionId: number;
}

export interface ReplaceStagedDrivetrainReferenceDto {
  importDrivetrainId: number;
}

export interface ReplaceStagedPowertrainReferenceResultDto {
  importVariantId: number;
  previousImportPowertrainId?: number | null;
  newImportPowertrainId: number;
  previousPowertrainRef?: string | null;
  newPowertrainRef: string;
  message: string;
}

export interface ReplaceStagedTransmissionReferenceResultDto {
  importVariantId: number;
  previousImportTransmissionId?: number | null;
  newImportTransmissionId: number;
  previousTransmissionRef?: string | null;
  newTransmissionRef: string;
  message: string;
}

export interface ReplaceStagedDrivetrainReferenceResultDto {
  importVariantId: number;
  previousImportDrivetrainId?: number | null;
  newImportDrivetrainId: number;
  previousDrivetrainRef?: string | null;
  newDrivetrainRef: string;
  message: string;
}