import { Service, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CalendarEventDto {
  summary: string;
  description: string;
  location: string;
  startUtc: string;
  endUtc: string;
  clientEmail: string;
  timeZone?: string;
}

export interface AppointmentDto {
  id: string;
  title: string;
  notes?: string;
  startTimeUtc: string;
  endTimeUtc: string;
  clientEmail: string;
  googleCalendarEventId?: string;
}

@Service()
export class Appointment {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/appointments`;

  readonly appointments = signal<AppointmentDto[]>([]);
  readonly isLoading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  getAppointments(): Observable<AppointmentDto[]> {
    this.isLoading.set(true);
    this.error.set(null);

    return this.http.get<AppointmentDto[]>(`${this.baseUrl}/upcoming`).pipe(
      tap({
        next: (data) => {
          this.appointments.set(data);
          this.isLoading.set(false);
        },
        error: () => {
          this.error.set('Failed to load appointments.');
          this.isLoading.set(false);
        },
      }),
    );
  }

  createAppointment(dto: CalendarEventDto): Observable<{ id: string }> {
    this.isLoading.set(true);

    return this.http.post<{ id: string }>(this.baseUrl, dto).pipe(
      tap({
        next: () => this.getAppointments().subscribe(),
        error: () => {
          this.error.set('Failed to create appointment.');
          this.isLoading.set(false);
        },
      }),
    );
  }

  updateAppointment(id: string, dto: CalendarEventDto): Observable<boolean> {
    this.isLoading.set(true);

    return this.http.put<boolean>(`${this.baseUrl}/${id}`, dto).pipe(
      tap({
        next: () => this.getAppointments().subscribe(),
        error: () => {
          this.error.set('Failed to update appointment.');
          this.isLoading.set(false);
        },
      }),
    );
  }

  deleteAppointment(id: string): Observable<boolean> {
    this.isLoading.set(true);

    return this.http.delete<boolean>(`${this.baseUrl}/${id}`).pipe(
      tap({
        next: () => {
          this.appointments.update((prev) => prev.filter((item) => item.id !== id));
          this.isLoading.set(false);
        },
        error: () => {
          this.error.set('Failed to delete appointment.');
          this.isLoading.set(false);
        },
      }),
    );
  }
}
