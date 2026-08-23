export interface ISpecs {
  engineId: number;
  engine: string ; 
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
