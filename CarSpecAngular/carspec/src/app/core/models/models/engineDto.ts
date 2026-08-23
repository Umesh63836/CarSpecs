export class CreateEngineDto {
  engineName: string;
  fuelTypeId: number;
  numberOfCylinders: number;
  numberOfValves: number;
  displacement: number;
  maxPower: number;
  maxTorque: number;
  isTurbocharged: boolean;
  emissionStandard: string;

  constructor(
    engineName: string,
    fuelTypeId: number,
    numberOfCylinders: number,
    numberOfValves: number,
    displacement: number,
    maxPower: number,
    maxTorque: number,
    isTurbocharged: boolean,
    emissionStandard: string
  ) {
    this.engineName = engineName;
    this.fuelTypeId = fuelTypeId;
    this.numberOfCylinders = numberOfCylinders;
    this.numberOfValves = numberOfValves;
    this.displacement = displacement;
    this.maxPower = maxPower;
    this.maxTorque = maxTorque;
    this.isTurbocharged = isTurbocharged;
    this.emissionStandard = emissionStandard;
  }
}

export class EngineDto {
  engineId: number;
  engineName: string;
  fuelTypeId: number;
  numberOfCylinders: number;
  numberOfValves: number;
  displacement: number;
  maxPower: number;
  maxTorque: number;
  isTurbocharged: boolean;
  emissionStandard: string;

  constructor(
    engineId: number,
    engineName: string,
    fuelTypeId: number,
    numberOfCylinders: number,
    numberOfValves: number,
    displacement: number,
    maxPower: number,
    maxTorque: number,
    isTurbocharged: boolean,
    emissionStandard: string
  ) {
    this.engineId = engineId;
    this.engineName = engineName;
    this.fuelTypeId = fuelTypeId;
    this.numberOfCylinders = numberOfCylinders;
    this.numberOfValves = numberOfValves;
    this.displacement = displacement;
    this.maxPower = maxPower;
    this.maxTorque = maxTorque;
    this.isTurbocharged = isTurbocharged;
    this.emissionStandard = emissionStandard;
  }
}