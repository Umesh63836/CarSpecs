export interface IModel {
  modelId: number;
  modelName: string;
  minPrice: number | null;
  maxPrice: number | null;
  minPower: number | null;
  maxPower: number | null;
  modelImageUrl: string | null;
}
