import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, Category, CreateCategoryRequest, UpdateCategoryRequest } from '../../core/api.service';

import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatTableModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatSnackBarModule, MatProgressBarModule,
    MatIconModule, MatTooltipModule
  ],
  templateUrl: './categories.html',
  styleUrls: ['./categories.scss']
})
export class CategoriesComponent {
  private api = inject(ApiService);
  private snack = inject(MatSnackBar);

  displayedColumns = ['id','name','actions'];
  rows = signal<Category[]>([]);
  loading = signal(false);

  name: string = '';
  selectedId: number | null = null;

  ngOnInit() { this.refresh(); }

  refresh() {
    this.loading.set(true);
    this.api.listCategories().subscribe({
      next: d => this.rows.set(d),
      error: err => { console.error(err); this.snack.open('Failed to load categories', 'Close', { duration: 2500 }); },
      complete: () => this.loading.set(false)
    });
  }

  clearForm() { this.name = ''; this.selectedId = null; }

  add() {
    const n = this.name.trim();
    if (!n) { this.snack.open('Name is required', 'Close', { duration: 2000 }); return; }
    this.loading.set(true);
    const body: CreateCategoryRequest = { name: n };
    this.api.createCategory(body).subscribe({
      next: _ => { this.snack.open('Category added', 'Close', { duration: 1500 }); this.clearForm(); this.refresh(); },
      error: err => { console.error(err); this.snack.open('Add failed', 'Close', { duration: 3000 }); this.loading.set(false); }
    });
  }

  edit(row: Category) {
    this.selectedId = row.id;
    this.name = row.name ?? '';
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() { this.clearForm(); }

  update() {
    if (this.selectedId == null) return;
    const n = this.name.trim();
    if (!n) { this.snack.open('Name is required', 'Close', { duration: 2000 }); return; }
    this.loading.set(true);
    const body: UpdateCategoryRequest = { id: this.selectedId, name: n };
    this.api.updateCategory(body).subscribe({
      next: _ => { this.snack.open('Category updated', 'Close', { duration: 1500 }); this.clearForm(); this.refresh(); },
      error: err => { console.error(err); this.snack.open('Update failed', 'Close', { duration: 3000 }); this.loading.set(false); }
    });
  }

  remove(row: Category) {
    if (!confirm(`Delete category "${row.name}"?`)) return;
    this.loading.set(true);
    this.api.deleteCategory(row.id).subscribe({
      next: _ => { this.snack.open('Category deleted', 'Close', { duration: 1500 }); if (this.selectedId === row.id) this.clearForm(); this.refresh(); },
      error: err => { console.error(err); this.snack.open('Delete failed', 'Close', { duration: 3000 }); this.loading.set(false); }
    });
  }
}
