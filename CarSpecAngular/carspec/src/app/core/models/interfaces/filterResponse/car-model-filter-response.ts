import { CarVariantFilterResponse } from "./car-variant-filter-response";

export interface CarModelFilterResponse {
  modelId: number;
  brand: string;
  model: string;
  modelImageUrl: string;
  variants: CarVariantFilterResponse[];
}
