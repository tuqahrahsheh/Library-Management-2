import { Routes } from '@angular/router';
import { BooksComponent } from './pages/books/books';
import { CategoriesComponent } from './pages/categories/categories';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'books' },
  { path: 'books', component: BooksComponent, title: 'Books' },
  { path: 'categories', component: CategoriesComponent, title: 'Categories' },
  { path: '**', redirectTo: 'books' }
];
