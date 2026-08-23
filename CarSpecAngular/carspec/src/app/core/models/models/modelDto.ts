export class CreateModelDto {
  modelName: string;
  brandId: number;
  isActive: boolean;
  launchYear: number;
  discontinuedYear: number | null;
  modelImageUrl: string | null;

  constructor(
    modelName: string,
    brandId: number,
    isActive: boolean,
    launchYear: number,
    discontinuedYear: number | null = null,
    modelImageUrl: string | null = null
  ) {
    this.modelName = modelName;
    this.brandId = brandId;
    this.isActive = isActive;
    this.launchYear = launchYear;
    this.discontinuedYear = discontinuedYear;
    this.modelImageUrl = modelImageUrl;
  }
}

export class ModelDto {
  modelId: number;
  modelName: string;
  brandId: number;
  isActive: boolean;
  launchYear: number;
  discontinuedYear: number | null;
  modelImageUrl: string | null;

  constructor(
    modelId: number,
    modelName: string,
    brandId: number,
    isActive: boolean,
    launchYear: number,
    discontinuedYear: number | null = null,
    modelImageUrl: string | null = null
  ) {
    this.modelId = modelId;
    this.modelName = modelName;
    this.brandId = brandId;
    this.isActive = isActive;
    this.launchYear = launchYear;
    this.discontinuedYear = discontinuedYear;
    this.modelImageUrl = modelImageUrl;
  }
}