import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TemplateIndexItem } from '../../../models/Template-Hub/TemplateIndexItem';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TemplateHub {
 private readonly baseUrl = `${environment.apiBaseUrl}/approvals/api`;
  private readonly jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });
  
  constructor(private http: HttpClient) {}

  listIndex(): Observable<TemplateIndexItem[]> {
    return this.http.get<TemplateIndexItem[]>(`${this.baseUrl}/template-hub`);
  }

  getTemplateDetails(key: string) {
    return this.http.get(`${this.baseUrl}/template-hub/${encodeURIComponent(key)}`);
  }

  downloadTemplate(key: string, filename?: string): void {
    const url = `${this.baseUrl}/template-hub/${encodeURIComponent(key)}/download`; // backend endpoint should provide a file
    this.http.get(url, { responseType: 'blob' }).subscribe({
      next: (blob) => {
        const name = filename ?? key.replace(/\//g, '-') + '.zip';
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = name;
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Download failed', err);
      }
    });
  }
}