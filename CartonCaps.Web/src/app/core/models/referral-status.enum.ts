export enum ReferralStatus {
  Pending = 1,
  Completed = 2,
  Expired = 3
}

export const ReferralStatusLabels: Record<ReferralStatus, string> = {
  [ReferralStatus.Pending]: 'Pending',
  [ReferralStatus.Completed]: 'Complete',
  [ReferralStatus.Expired]: 'Expired'
};
