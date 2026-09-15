export interface LocationsearchResponse {
  type: 'State' | 'District' | 'City';
  id: number;
  name: string;

  stateCode: number;
  stateName: string;

  districtCode?: number;
  districtName?: string;
}
