import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ReferralService } from '../../../../core/services/referral.service';
import { ClipboardService } from '../../../../core/services/clipboard.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-invite-friends',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './invite-friends.component.html',
  styleUrls: ['./invite-friends.component.scss']
})
export class InviteFriendsComponent implements OnInit {
  userCode = environment.hardcodedUser.referralCode;
  hasReferrals = signal(false);
  errorMessage = signal<string | null>(null);
  isCopying = signal(false);

  constructor(
    public referralService: ReferralService,
    private clipboardService: ClipboardService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.referralService.getMyReferrals().subscribe({
      next: (referrals) => {
        this.hasReferrals.set(referrals.length > 0);
      },
      error: (error) => {
        this.errorMessage.set(error || 'Error loading referrals');
      }
    });
  }

  onCopyCode(): void {
    this.isCopying.set(true);
    this.referralService.generateInvitationLink().subscribe({
      next: (invitationLink) => {
        this.clipboardService.copyToClipboard(
          invitationLink.link,
          'Invitation link copied!'
        );
        this.isCopying.set(false);
      },
      error: (error) => {
        this.errorMessage.set(error || 'Error generating invitation link');
        this.isCopying.set(false);
      }
    });
  }

  getStatusLabel(status: number): string {
    return status === 2 ? 'Complete' : status === 1 ? 'Pending' : 'Expired';
  }
}
