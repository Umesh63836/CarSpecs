export interface IModel {
  modelId: number;
  modelName: string;
  minPrice: number | null;
  maxPrice: number | null;
  minPower: number | null;
  maxPower: number | null;
  safetyRating: number | null;
  safetyRatingSource: string | null;
  safetyTestYear: number | null;
  engineCC: number[];
  fuelTypes: string[];
  fuelEfficiency: number | null;
  fuelEfficiencyUnit: string | null;
  fuelEfficiencySource: string | null;
  modelImageUrl: string | null;
}
