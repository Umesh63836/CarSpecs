export class CreateTransmissionDto {
  transmissionType: string;
  numberOfGears: number;

  constructor(
    transmissionType: string,
    numberOfGears: number
  ) {
    this.transmissionType = transmissionType;
    this.numberOfGears = numberOfGears;
  }
}

export class TransmissionDto {
  transmissionId: number;
  transmissionType: string;
  numberOfGears: number;

  constructor(
    transmissionId: number,
    transmissionType: string,
    numberOfGears: number
  ) {
    this.transmissionId = transmissionId;
    this.transmissionType = transmissionType;
    this.numberOfGears = numberOfGears;
  }
}