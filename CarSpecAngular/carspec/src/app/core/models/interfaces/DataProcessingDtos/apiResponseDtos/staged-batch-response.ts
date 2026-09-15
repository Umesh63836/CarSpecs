export interface StagedBatchResponse {
  importBatchId: number
  status: string
  importModelId: number
  powertrainsInserted: number
  enginesInserted: number
  enginePerformancesInserted: number
  motorPerformancesInserted: number
  transmissionsInserted: number
  drivetrainsInserted: number
  variantParentsInserted: number
  variantsInserted: number
  featureVariantsInserted: number
  specificationVariantsInserted: number
  fuelEfficienciesInserted: number
  fuelEfficiencyVariantsInserted: number
  warningsInserted: number
  warnings: string[]
}
