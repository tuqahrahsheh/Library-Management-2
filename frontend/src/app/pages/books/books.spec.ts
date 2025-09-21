import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/api.service';

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
  selector: 'app-books',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatTableModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatSnackBarModule, MatProgressBarModule,
    MatIconModule, MatTooltipModule
  ],
  templateUrl: './books.html',
  styleUrls: ['./books.scss']
})
export class BooksComponent {
  private api = inject(ApiService);
  private snack = inject(MatSnackBar);

  displayedColumns = ['id','title','author','isbn','publishedDate','categoryIds','actions'];
  rows = signal<any[]>([]);
  loading = signal(false);

  title = '';
  author = '';
  isbn = '';
  publishedDate = '';   // YYYY-MM-DD (input date)
  quantity: number = 1; // رقم
  categoryIdsCsv = '';

  selectedId: number | null = null;

  ngOnInit() { this.refresh(); }

  refresh() {
    this.loading.set(true);
    this.api.listBooks().subscribe({
      next: d => this.rows.set(d),
      error: err => {
        console.error('GET /books failed', err);
        this.snack.open('Failed to load books', 'Close', { duration: 2500 });
      },
      complete: () => this.loading.set(false)
    });
  }

  clearForm() {
    this.title = this.author = this.isbn = this.publishedDate = this.categoryIdsCsv = '';
    this.quantity = 1;
    this.selectedId = null;
  }

  private parseIds(csv: string): number[] {
    return csv.split(',')
      .map(s => parseInt(s.trim(), 10))
      .filter(n => !isNaN(n));
  }

  private toIsoOrUndef(d: string | undefined) {
    return d ? new Date(d).toISOString() : undefined;
  }

  add() {
    const title = this.title.trim();
    const author = this.author.trim();
    if (!title || !author) {
      this.snack.open('Title & Author are required', 'Close', { duration: 2000 });
      return;
    }
    const ids = this.parseIds(this.categoryIdsCsv);
    const pub = this.toIsoOrUndef(this.publishedDate);

    this.loading.set(true);
    this.api.createBook({
      title, author,
      isbn: this.isbn.trim() || undefined,
      publishedDate: pub,
      quantity: Number(this.quantity) || 1,
      categoryIds: ids
    }).subscribe({
      next: _ => {
        this.snack.open('Book added', 'Close', { duration: 1500 });
        this.clearForm();
        this.refresh();
      },
      error: err => {
        console.error('POST /books failed', err);
        this.snack.open('Add failed (check Category Ids & data)', 'Close', { duration: 3000 });
        this.loading.set(false);
      }
    });
  }

  edit(row: any) {
    this.selectedId = row.id;
    this.title = row.title ?? '';
    this.author = row.author ?? '';
    this.isbn = row.isbn ?? '';

    if (row.publishedDate) {
      const d = new Date(row.publishedDate);
      this.publishedDate = `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`;
    } else {
      this.publishedDate = '';
    }

    this.quantity = row.quantity ?? 1;
    this.categoryIdsCsv = Array.isArray(row.categoryIds) ? row.categoryIds.join(',') : '';
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.clearForm();
  }

  update() {
    if (this.selectedId == null) return;

    const title = this.title.trim();
    const author = this.author.trim();
    if (!title || !author) {
      this.snack.open('Title & Author are required', 'Close', { duration: 2000 });
      return;
    }
    const ids = this.parseIds(this.categoryIdsCsv);
    const pub = this.toIsoOrUndef(this.publishedDate);

    this.loading.set(true);
    this.api.updateBook({
      id: this.selectedId,
      title, author,
      isbn: this.isbn.trim() || undefined,
      publishedDate: pub,
      quantity: Number(this.quantity) || 1,
      categoryIds: ids
    }).subscribe({
      next: _ => {
        this.snack.open('Book updated', 'Close', { duration: 1500 });
        this.clearForm();
        this.refresh();
      },
      error: err => {
        console.error('PUT /books/{id} failed', err);
        this.snack.open('Update failed', 'Close', { duration: 3000 });
        this.loading.set(false);
      }
    });
  }

  remove(row: any) {
    if (!row?.id) return;
    if (!confirm(`Delete book "${row.title}"?`)) return;

    this.loading.set(true);
    this.api.deleteBook(row.id).subscribe({
      next: _ => {
        this.snack.open('Book deleted', 'Close', { duration: 1500 });
        if (this.selectedId === row.id) this.clearForm();
        this.refresh();
      },
      error: err => {
        console.error('DELETE /books/{id} failed', err);
        this.snack.open('Delete failed', 'Close', { duration: 3000 });
        this.loading.set(false);
      }
    });
  }
}
