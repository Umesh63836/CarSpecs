import { AfterViewInit, Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { Model } from '../../core/services/model/model';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { IModel } from '../../core/models/interfaces/model';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-models',
  imports: [FormsModule, RouterLink, DecimalPipe],
  templateUrl: './models.html',
  styleUrl: './models.css',
})
export class Models implements OnInit, AfterViewInit {
  modelService = inject(Model);
  private route = inject(ActivatedRoute);

  @ViewChild('modelsContainer') modelsContainer?: ElementRef<HTMLDivElement>;

  brandId = signal<number>(0);
  models = signal<IModel[]>([]);
  showLeftArrow = signal(false);
  showRightArrow = signal(false);
  private lastRenderedModelCount = 0;

  ngOnInit(): void {
    this.route.paramMap.subscribe((paramMap) => {
      const id = Number(paramMap.get('brandId'));
      this.brandId.set(id);
      this.modelService.getModels(id).subscribe({
        next: (result: IModel[]) => {
          this.models.set(result);
          this.refreshScrollStateAfterLayout();
        },
        error: (error) => console.error('Error loading models:', error)
      });
    });
  }

  ngAfterViewInit(): void {
    this.refreshScrollStateAfterLayout();
  }

  private refreshScrollStateAfterLayout(): void {
    requestAnimationFrame(() => this.updateScrollState());
    setTimeout(() => this.updateScrollState(), 50);
  }

  onModelsScroll(): void {
    this.updateScrollState();
  }

  onModelsResize(): void {
    this.updateScrollState();
  }

  updateScrollState(): void {
    const container = this.modelsContainer?.nativeElement;

    if (!container) {
      return;
    }

    const maxScrollLeft = container.scrollWidth - container.clientWidth;
    const canScroll = maxScrollLeft > 2;

    this.showLeftArrow.set(canScroll && container.scrollLeft > 2);
    this.showRightArrow.set(canScroll && container.scrollLeft < maxScrollLeft - 2);
  }

  scrollModels(direction: 'prev' | 'next'): void {
    const container = this.modelsContainer?.nativeElement;

    if (!container) {
      return;
    }

    const firstCard = container.querySelector('.model-card') as HTMLElement | null;
    const cardWidth = firstCard ? firstCard.getBoundingClientRect().width + 16 : container.clientWidth * 0.78;
    const delta = direction === 'next' ? cardWidth : -cardWidth;

    container.scrollBy({
      left: delta,
      behavior: 'smooth'
    });
  }

}
