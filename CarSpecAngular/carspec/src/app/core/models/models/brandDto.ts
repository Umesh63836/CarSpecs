export class CreateBrandDto {
  brandName: string;
  logoUrl: string | null;

  constructor(brandName: string, logoUrl: string | null = null) {
    this.brandName = brandName;
    this.logoUrl = logoUrl;
  }
}

export class BrandDto {
  brandId: number;
  brandName: string;
  logoUrl: string | null;

  constructor(brandId: number, brandName: string, logoUrl: string | null = null) {
    this.brandId = brandId;
    this.brandName = brandName;
    this.logoUrl = logoUrl;
   }
}