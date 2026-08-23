import { Component, inject, OnInit, signal } from '@angular/core';
import { Model } from '../../core/services/model/model';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { IModel } from '../../core/models/interfaces/model';

@Component({
  selector: 'app-models',
  imports: [FormsModule,RouterLink],
  templateUrl: './models.html',
  styleUrl: './models.css',
})
export class Models implements OnInit{
  modelService = inject(Model);
  private route = inject(ActivatedRoute);

  brandId = signal<number>(0);

  models = signal<IModel[]>([]);

  ngOnInit(): void {
    // Subscribe to route parameter changes to handle navigation within the same component
    this.route.paramMap.subscribe((paramMap) => {
      const id = Number(paramMap.get('brandId'));
      this.brandId.set(id);
      this.modelService.getModels(id).subscribe({
        next: (result: IModel[]) => this.models.set(result),
        error: (error) => console.error('Error loading models:', error)
      });
    });
  }

}
