import { ReferralStatus } from './referral-status.enum';

export interface ReferralDto {
  name: string;
  status: ReferralStatus;
}

export interface ReferralRegisterDto {
  name: string;
  email: string;
  referralCode: string;
  referralId?: string;
}

export interface InvitationLink {
  link: string;
}
