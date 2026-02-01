import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/referrer/components/invite-friends/invite-friends.component').then(
        m => m.InviteFriendsComponent
      )
  },
  {
    path: 'signup',
    loadComponent: () =>
      import('./features/referee/components/complete-profile/complete-profile.component').then(
        m => m.CompleteProfileComponent
      )
  },
  {
    path: '**',
    redirectTo: ''
  }
];
