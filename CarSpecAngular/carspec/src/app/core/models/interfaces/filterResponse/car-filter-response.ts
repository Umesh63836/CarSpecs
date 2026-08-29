import { CarModelFilterResponse } from "./car-model-filter-response";

export interface CarFilterResponse {
  totalModels: number;

  totalVariants: number;

  models: CarModelFilterResponse[];
}
