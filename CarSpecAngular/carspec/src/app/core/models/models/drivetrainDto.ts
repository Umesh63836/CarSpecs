export class CreateDrivetrainDto {
  drivetrainType: string;

  constructor(drivetrainType: string) {
    this.drivetrainType = drivetrainType;
  }
}

export class DrivetrainDto {
  drivetrainId: number;
  drivetrainType: string;

  constructor(
    drivetrainId: number,
    drivetrainType: string
  ) {
    this.drivetrainId = drivetrainId;
    this.drivetrainType = drivetrainType;
  }
}