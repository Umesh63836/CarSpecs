export interface ISpecs {
  variantId: number
  brand: string ;
  model: string ;
  variant: string ;
  engine: string ; 
  exShowroomPrice: number | null;
  noOfCyl: number | null;
  noOfValves: number | null;
  displacement: number | null;
  maxPower: number | null;
  maxTorque: number | null;
  isTurbocharged: boolean | null;
  emmissionStandard: string | null;
  transmissionType: string | null;
  fuelType: string | null;
  noOfGears: number | null;
  drivetrain: string | null;
  varientImageURL: string | null;
}
