import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'appointments',
    loadComponent: () => import('./features/appointments/appointments').then((m) => m.Appointments),
    canActivate: [authGuard],
  },
  {
    path: '',
    redirectTo: 'appointments',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'appointments',
  },
];
