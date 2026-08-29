export class CarFilterRequest {
  brand?: string;
  model?: string;

  displacement?: number;

  minPower?: number;
  maxPower?: number;

  minTorque?: number;
  maxTorque?: number;

  isTurbocharged?: boolean;

  emissionStandard?: string;

  transmissionType?: string;
  numberOfGears?: number;

  drivetrainType?: string;

  fuelType?: string;

  minPrice?: number;
  maxPrice?: number;
}