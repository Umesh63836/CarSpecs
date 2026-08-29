import { Component, inject, Inject, OnInit, signal } from '@angular/core';
import { Brand } from '../../core/services/brand/brand';
import { ActivatedRoute, RouterLink } from "@angular/router";
import { FormsModule } from '@angular/forms';
import { IBrand } from '../../core/models/interfaces/brand';
import { AuthService } from '../../core/services/AdminServices/AuthService/authService';
import { ShowFilter } from "../filter/show-filter/show-filter";

@Component({
  selector: 'app-brands',
  imports: [RouterLink, FormsModule, ShowFilter],
  templateUrl: './brands.html',
  styleUrl: './brands.css',
})
export class Brands implements OnInit{
  private brandService = inject(Brand)
  private route = inject(ActivatedRoute)
  authService = inject(AuthService);

  brands = signal<IBrand[]>([]);

  brandsExpanded = signal(false);
  
  ngOnInit() {
    this.brandService.getBrands().subscribe((result: IBrand[]) => {
      this.brands.set(result);
    })
    const brandId = this.route.snapshot.paramMap.get('brandId');
  }

}
