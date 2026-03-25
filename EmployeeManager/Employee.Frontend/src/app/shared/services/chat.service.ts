import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ChatService {

  private api = `${environment.apiUrl}/chat`;

  constructor(private http: HttpClient) {}

  sendMessage(data: any) {
    return this.http.post(this.api + '/send', data);
  }

  getMessages(userId: number) {
    return this.http.get<any[]>(this.api + '/get/' + userId);
  }

  uploadFile(file: File) {
  const formData = new FormData();
  formData.append('file', file);

  return this.http.post<any>(this.api + '/upload', formData);
}
}