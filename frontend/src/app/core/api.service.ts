import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';


export interface Category { id: number; name: string; }

export interface Book {
  id: number;
  title: string;
  author: string;
  isbn?: string | null;
  publishedDate?: string | null; 
  quantity: number;
  categoryIds?: number[];
}


export interface CreateCategoryRequest { name: string; }
export interface UpdateCategoryRequest { id: number; name: string; }

export interface CreateBookRequest {
  title: string;
  author: string;
  isbn?: string;
  publishedDate?: string;
  quantity: number;
  categoryIds: number[];
}
export interface UpdateBookRequest extends CreateBookRequest { id: number; }

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private base = environment.apiBase; 

 
  listCategories() { return this.http.get<Category[]>(`${this.base}/api/categories`); }
  getCategory(id: number) { return this.http.get<Category>(`${this.base}/api/categories/${id}`); }
  createCategory(body: CreateCategoryRequest) { return this.http.post<Category>(`${this.base}/api/categories`, body); }
  updateCategory(body: UpdateCategoryRequest) { return this.http.put(`${this.base}/api/categories/${body.id}`, body); }
  deleteCategory(id: number) { return this.http.delete(`${this.base}/api/categories/${id}`); }

 
  listBooks() { return this.http.get<Book[]>(`${this.base}/api/books`); }
  getBook(id: number) { return this.http.get<Book>(`${this.base}/api/books/${id}`); }
  createBook(body: CreateBookRequest) { return this.http.post(`${this.base}/api/books`, body); }
  updateBook(body: UpdateBookRequest) { return this.http.put(`${this.base}/api/books/${body.id}`, body); }
  deleteBook(id: number) { return this.http.delete(`${this.base}/api/books/${id}`); }

  /* ===== ADO.NET + Stored Procedure ===== */
  listBooksWithCategoriesViaSp() { return this.http.get<any[]>(`${this.base}/api/books/with-categories-sp`); }
}
