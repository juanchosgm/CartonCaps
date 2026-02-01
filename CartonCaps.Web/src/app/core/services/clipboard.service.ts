import { Injectable } from '@angular/core';
import { Clipboard } from '@angular/cdk/clipboard';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class ClipboardService {
  constructor(
    private clipboard: Clipboard,
    private snackBar: MatSnackBar
  ) {}

  copyToClipboard(text: string, successMessage: string = 'Copied to clipboard!'): void {
    const success = this.clipboard.copy(text);

    if (success) {
      this.snackBar.open(successMessage, 'Close', {
        duration: 3000,
        horizontalPosition: 'center',
        verticalPosition: 'bottom'
      });
    } else {
      this.snackBar.open('Failed to copy', 'Close', {
        duration: 3000,
        horizontalPosition: 'center',
        verticalPosition: 'bottom'
      });
    }
  }
}
