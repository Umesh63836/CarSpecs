import { Component, inject, OnInit, signal } from '@angular/core';
import { Specs } from '../../core/services/specification/specs';
import { ActivatedRoute } from '@angular/router';
import { ISpecs } from '../../core/models/interfaces/specs';

@Component({
  selector: 'app-specifications',
  imports: [],
  templateUrl: './specifications.html',
  styleUrl: './specifications.css',
})
export class Specifications implements OnInit{
  specsService = inject(Specs);
  private route = inject(ActivatedRoute);

  variantId!: number;
  expandedSpecification = signal<string | null>('engine');


  specifications = signal<ISpecs | null>(null);

  ngOnInit(): void {
    // Subscribe to route parameter changes to handle navigation within the same component
    this.route.paramMap.subscribe((paramMap) => {
      this.variantId = Number(paramMap.get('variantId'));
      this.specsService.getSpecs(this.variantId).subscribe({
        next: (result: ISpecs) => this.specifications.set(result),
        error: (error) => console.error('Error loading specifications:', error)
      });
    });
  }

toggleSpecification(section: string): void {
  this.expandedSpecification.update(
    current => current === section ? null : section
  );
  }

}
