import { Component, inject, OnInit, signal } from '@angular/core';
import { Variant } from '../../core/services/variant/variant';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { IVariant } from '../../core/models/interfaces/variant';
import { Model } from '../../core/services/model/model';
import { IModel } from '../../core/models/interfaces/model';
import { UpperCasePipe } from '@angular/common';
import { IVariantModel } from '../../core/models/interfaces/variant-model';

@Component({
  selector: 'app-variants',
  imports: [RouterLink, UpperCasePipe],
  templateUrl: './variants.html',
  styleUrl: './variants.css',
})
export class Variants implements OnInit{
  variantsService = inject(Variant);
  modelsService = inject(Model);
  private route = inject(ActivatedRoute);

  modelId!: number;

  model = signal<IVariantModel | null>(null);
  variants = signal<IVariant[]>([]);
  variantsExpanded = signal(false);

  expandedVariantId = signal<number | null>(null);

  ngOnInit(): void {
    // Subscribe to route parameter changes to handle navigation within the same component
    this.route.paramMap.subscribe((paramMap) => {
      this.modelId = Number(paramMap.get('modelId'));
      this.loadVariantsData();
    });
  }

  private loadVariantsData(): void {
    // Reset expanded variant when loading new model's variants
    this.expandedVariantId.set(null);

    // Load model details
    this.modelsService.getModelByModelId(this.modelId).subscribe({
      next: (result: IVariantModel) => this.model.set(result),
      error: (error) => console.error('Error loading model:', error)
    });

    // Load variants
    this.variantsService.getVariants(this.modelId).subscribe({
      next: (result: IVariant[]) => this.variants.set(result),
      error: (error) => console.error('Error loading variants:', error)
    });
  }

  getEngineSize(cc: number | null): string {
  return cc !== null ? (Math.ceil(cc / 100) / 10).toFixed(1) : 'N/A';
  }

  toggleVariant(variantId: number): void {

  if (this.expandedVariantId() === variantId) {
    // Collapse currently open variant
    this.expandedVariantId.set(null);
  } else {
    // Expand selected variant
    this.expandedVariantId.set(variantId);
  }
  }

  

}
