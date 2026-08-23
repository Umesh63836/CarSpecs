export interface IVariant {
  variantId: number;
  variantName: string;
  fuelType: string | null;
  cubicCapacity: number | null;
  isTurbocharged: boolean | null;
  transmissionType: string | null;
  maxPower: number | null;
  maxTorque: number | null;
  exShowroomPrice: number | null;
}
