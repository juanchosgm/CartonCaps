import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ReferralService } from '../../../../core/services/referral.service';
import { ReferralRegisterDto } from '../../../../core/models/referral.model';

@Component({
  selector: 'app-complete-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './complete-profile.component.html',
  styleUrls: ['./complete-profile.component.scss']
})
export class CompleteProfileComponent implements OnInit {
  profileForm!: FormGroup;
  isSubmitting = signal<boolean>(false);
  isSuccess = signal<boolean>(false);
  referralId?: string;
  referralCode?: string;
  registeredName = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private referralService: ReferralService,
    private snackBar: MatSnackBar
  ) {
    this.initForm();
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.referralId = params['ref'];
      this.referralCode = params['code'];
      if (this.referralCode) {
        this.profileForm.patchValue({
          referralCode: this.referralCode
        });
        this.profileForm.get('referralCode')?.setValidators([Validators.required, Validators.minLength(3)]);
        this.profileForm.get('referralCode')?.updateValueAndValidity();
        this.profileForm.get('referralCode')?.disable();
      }
    });
  }

  private initForm(): void {
    this.profileForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      referralCode: ['']
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) {
      this.markFormGroupTouched(this.profileForm);
      return;
    }
    const formValue = this.profileForm.getRawValue();
    const registerDto: ReferralRegisterDto = {
      name: formValue.name.trim(),
      email: formValue.email.trim(),
      referralCode: formValue.referralCode?.trim() || '',
      referralId: this.referralId
    };
    this.isSubmitting.set(true);
    this.referralService.registerReferral(registerDto).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.registeredName.set(formValue.name);
        this.isSuccess.set(true);
      },
      error: (error) => {
        this.isSubmitting.set(false);
        this.snackBar.open(error || 'Registration failed. Please try again.', 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom',
          panelClass: ['error-snackbar']
        });
      }
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  getErrorMessage(fieldName: string): string {
    const control = this.profileForm.get(fieldName);

    if (control?.hasError('required')) {
      return 'This field is required';
    }

    if (control?.hasError('email')) {
      return 'Please enter a valid email address';
    }

    if (control?.hasError('minlength')) {
      const minLength = control.errors?.['minlength'].requiredLength;
      return `Minimum length is ${minLength} characters`;
    }

    if (control?.hasError('pattern')) {
      return 'Invalid format';
    }

    return '';
  }
}
