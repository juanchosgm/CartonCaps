import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApiService } from './api.service';
import { ReferralDto, ReferralRegisterDto, InvitationLink } from '../models/referral.model';

@Injectable({
  providedIn: 'root'
})
export class ReferralService {
  referrals = signal<ReferralDto[]>([]);
  isLoading = signal<boolean>(false);
  currentInvitationLink = signal<string | null>(null);

  constructor(private apiService: ApiService) { }

  getMyReferrals(): Observable<ReferralDto[]> {
    this.isLoading.set(true);
    return this.apiService.get<ReferralDto[]>('/referral/my-referrals')
      .pipe(
        tap(referrals => {
          this.referrals.set(referrals);
          this.isLoading.set(false);
        })
      );
  }

  generateInvitationLink(): Observable<InvitationLink> {
    this.isLoading.set(true);
    return this.apiService.post<InvitationLink>('/referral/invitation-link', {})
      .pipe(
        tap(data => {
          this.currentInvitationLink.set(data.link);
          this.isLoading.set(false);
        })
      );
  }

  registerReferral(data: ReferralRegisterDto): Observable<void> {
    this.isLoading.set(true);
    return this.apiService.post<void>('/referral/referral-registration', data)
      .pipe(tap(() => this.isLoading.set(false)));
  }
}
