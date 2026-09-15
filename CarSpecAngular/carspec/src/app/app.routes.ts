import { Routes } from '@angular/router';
import { Brands } from './features/brands/brands';
import { Models } from './features/models/models';
import { Variants } from './features/variants/variants';
import { Specifications } from './features/specifications/specifications';
import { CreateBrand } from './features/admin/create-vehicle/create-brand/create-brand';
import { CreateModel } from './features/admin/create-vehicle/create-model/create-model';
import { CreateVariant } from './features/admin/create-vehicle/create-variant/create-variant';
import { AdminDashboard } from './features/admin/admin-dashboard/admin-dashboard';
import { AdminLogin } from './features/admin/admin-login/admin-login';
import { authguardGuard } from './core/services/guard/authguard-guard';
import { FilterCars } from './features/filter/filter-cars/filter-cars';
import { BrochureReview } from './features/admin/brochure-review/brochure-review';
import { CreateVehicle } from './features/admin/create-vehicle/create-vehicle';
import { Uploadbrochure } from './features/admin/upload-brochure/uploadbrochure';
import { BrochureImport } from './features/admin/brochure-import/brochure-import';
import { StagedData } from './features/admin/staged-data/staged-data';
import { Batches } from './features/admin/batches/batches';
import { CreateEngine } from './features/admin/create-vehicle/create-engine/create-engine';
import { CreateTransmission } from './features/admin/create-vehicle/create-transmission/create-transmission';
import { CreateDrivetrain } from './features/admin/create-vehicle/create-drivetrain/create-drivetrain';
import { CreateFuelType } from './features/admin/create-vehicle/create-fuel-type/create-fuel-type';

export const routes: Routes = [
    {
        path: "",
        component: Brands
    },
    {
        path: "brands",
        component: Brands
    },
    {
        path: "models/:brandId",
        component: Models
    },
    {
        path: "variants/:modelId",
        component: Variants
    },
    {
        path: "specifications/:variantId",
        component: Specifications
    },
    {
        path: 'cars',
        component: FilterCars
    },
    {
      path: 'admindashboard',
      component: AdminDashboard,
            canActivate: [authguardGuard],
            canActivateChild: [authguardGuard],

      children: [
        {
          path: '',
          redirectTo: 'staged-data',
          pathMatch: 'full'
        },
        {
          path: 'create-vehicle',
          component: CreateVehicle,
            children: [  
               {
                 path: 'create-brand',
                 component: CreateBrand
               },
           
               {
                 path: 'create-model',
                 component: CreateModel
               },
           
               {
                 path: 'create-variant',
                 component: CreateVariant
               },
           
               {
                 path: 'create-engine',
                 component: CreateEngine
               },
           
               {
                 path: 'create-transmission',
                 component: CreateTransmission
               },
           
               {
                 path: 'create-drivetrain',
                 component: CreateDrivetrain
               },
               {
                 path: 'create-fuel-type',
                 component: CreateFuelType
               }
             ]
        },

        {
          path: 'upload-brochure',
          component: Uploadbrochure
        },

        {
          path: 'import-brochure',
          component: BrochureImport
        },

        {
          path: 'staged-data',
          component: StagedData
        },
        {
          path: 'batches',
          component: Batches
        },
        {
          path: 'admin-review/:batchId',
          component: BrochureReview
        }
      ]
    },
    {
        path: "login",
        component: AdminLogin
    },
    {
        path: "**",
        redirectTo: ""
    }
];
