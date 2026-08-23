export class CreateVariantDto {
  variantName: string;
  engineId: number;
  transmissionId: number;
  drivetrainId: number;
  exShowroomPrice: number;
  variantImageUrl: string | null;

  constructor(
    variantName: string,
    engineId: number,
    transmissionId: number,
    drivetrainId: number,
    exShowroomPrice: number,
    variantImageUrl: string | null = null
  ) {
    this.variantName = variantName;
    this.engineId = engineId;
    this.transmissionId = transmissionId;
    this.drivetrainId = drivetrainId;
    this.exShowroomPrice = exShowroomPrice;
    this.variantImageUrl = variantImageUrl;
  }
}

export class VariantDto {
  variantId: number;
  variantName: string;
  fuelType: string;
  cubicCapacity: number;
  isTurbocharged: boolean;
  transmissionType: string;
  maxPower: number;
  maxTorque: number;
  exShowroomPrice: number;

  constructor(
    variantId: number,
    variantName: string,
    fuelType: string,
    cubicCapacity: number,
    isTurbocharged: boolean,
    transmissionType: string,
    maxPower: number,
    maxTorque: number,
    exShowroomPrice: number
  ) {
    this.variantId = variantId;
    this.variantName = variantName;
    this.fuelType = fuelType;
    this.cubicCapacity = cubicCapacity;
    this.isTurbocharged = isTurbocharged;
    this.transmissionType = transmissionType;
    this.maxPower = maxPower;
    this.maxTorque = maxTorque;
    this.exShowroomPrice = exShowroomPrice;
  }
}