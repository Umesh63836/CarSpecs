import { Routes } from '@angular/router';
import { Brands } from './features/brands/brands';
import { Models } from './features/models/models';
import { Variants } from './features/variants/variants';
import { Specifications } from './features/specifications/specifications';
import { CreateBrand } from './features/admin/create-brand/create-brand';
import { CreateModel } from './features/admin/create-model/create-model';
import { CreateVariant } from './features/admin/create-variant/create-variant';
import { AdminDashboard } from './features/admin/admin-dashboard/admin-dashboard';
import { AdminLogin } from './features/admin/admin-login/admin-login';
import { authguardGuard } from './core/services/guard/authguard-guard';

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
        path: "createbrand",
        component: CreateBrand,
        canActivate: [authguardGuard]
    },
    {
        path: "createmodel",
        component: CreateModel,
        canActivate: [authguardGuard]
    },
    {
        path: "createvariant",
        component: CreateVariant,
        canActivate: [authguardGuard]
    },
    {
        path: "admindashboard",
        component: AdminDashboard,
        canActivate: [authguardGuard]
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
