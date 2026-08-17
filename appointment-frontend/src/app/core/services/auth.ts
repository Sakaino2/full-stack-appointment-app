import { Service, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}

@Service()
export class Auth {
  private http = inject(HttpClient);
  private readonly tokenKey = 'jwt_token';

  private isBrowser(): boolean {
    return typeof window !== 'undefined' && typeof localStorage !== 'undefined';
  }

  readonly isAuthenticated = signal<boolean>(
    this.isBrowser() ? !!localStorage.getItem(this.tokenKey) : false,
  );
  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, credentials).pipe(
      tap((res) => {
        if (this.isBrowser()) {
          localStorage.setItem(this.tokenKey, res.token);
        }
        this.isAuthenticated.set(true);
      }),
    );
  }

  logout(): void {
    if (this.isBrowser()) {
      localStorage.removeItem(this.tokenKey);
    }
    this.isAuthenticated.set(false);
  }

  getToken(): string | null {
    return this.isBrowser() ? localStorage.getItem(this.tokenKey) : null;
  }
}
