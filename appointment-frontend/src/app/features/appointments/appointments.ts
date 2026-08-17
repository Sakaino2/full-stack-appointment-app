import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Appointment, AppointmentDto, CalendarEventDto } from '../../core/services/appointment';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './appointments.html',
  styleUrl: './appointments.scss',
})
export class Appointments implements OnInit {
  protected readonly appointmentService = inject(Appointment);
  private readonly fb = inject(FormBuilder);

  readonly editingId = signal<string | null>(null);

  readonly appointmentForm = this.fb.nonNullable.group({
    summary: ['', [Validators.required]],
    description: [''],
    location: ['Consultation Room / Online'],
    startUtc: ['', [Validators.required]],
    endUtc: ['', [Validators.required]],
    clientEmail: ['', [Validators.required, Validators.email]],
  });

  ngOnInit(): void {
    this.appointmentService.getAppointments().subscribe();
  }

  onSubmit(): void {
    if (this.appointmentForm.invalid) return;

    const rawValue = this.appointmentForm.getRawValue();

    const payload: CalendarEventDto = {
      ...rawValue,
      startUtc: new Date(rawValue.startUtc).toISOString(),
      endUtc: new Date(rawValue.endUtc).toISOString(),
      timeZone: 'UTC',
    };

    const currentEditingId = this.editingId();

    if (currentEditingId) {
      this.appointmentService.updateAppointment(currentEditingId, payload).subscribe({
        next: () => this.resetForm(),
      });
    } else {
      this.appointmentService.createAppointment(payload).subscribe({
        next: () => this.resetForm(),
      });
    }
  }

  onEdit(item: AppointmentDto): void {
    this.editingId.set(item.id);

    const startFormatted = new Date(item.startTimeUtc).toISOString().slice(0, 16);
    const endFormatted = new Date(item.endTimeUtc).toISOString().slice(0, 16);

    this.appointmentForm.patchValue({
      summary: item.title,
      description: item.notes ?? '',
      clientEmail: item.clientEmail,
      startUtc: startFormatted,
      endUtc: endFormatted,
    });
  }

  onDelete(id: string): void {
    if (confirm('Are you sure you want to delete this appointment?')) {
      this.appointmentService.deleteAppointment(id).subscribe();
    }
  }

  resetForm(): void {
    this.editingId.set(null);
    this.appointmentForm.reset({
      location: 'Consultation Room / Online',
    });
  }
}
