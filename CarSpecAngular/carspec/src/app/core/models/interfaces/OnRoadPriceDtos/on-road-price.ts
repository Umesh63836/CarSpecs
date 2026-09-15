import { IRegistrationDto } from "./registrationDto";

export interface IOnRoadPriceDto {
  variantId: number;
  exShowroomPrice: number;
  registration: IRegistrationDto;
  insurance: number;
  tcs: number;
  fastag: number;
  totalOnRoadPrice: number;
}
