import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class SignalrService implements OnDestroy {
  private hub?: HubConnection;
  messageReceived$ = new Subject<any>();
  messageSent$     = new Subject<any>();
  connected$       = new Subject<boolean>();

  constructor(private auth: AuthService) {}

  async startConnection(): Promise<void> {
    const token = this.auth.getToken();
    if (!token) return;
    this.hub = new HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}/hubs/chat`, { accessTokenFactory: () => token })
      .withAutomaticReconnect().configureLogging(LogLevel.Warning).build();
    this.hub.on('ReceiveMessage', (msg: any) => this.messageReceived$.next(msg));
    this.hub.on('MessageSent',    (msg: any) => this.messageSent$.next(msg));
    this.hub.onclose(() => this.connected$.next(false));
    try { await this.hub.start(); this.connected$.next(true); } catch { console.warn('SignalR unavailable — polling active'); }
  }
  async sendMessage(receiverId: string, content: string): Promise<void> {
    if (this.hub?.state === 'Connected') { await this.hub.invoke('SendMessage', receiverId, content, null); }
  }
  async stopConnection(): Promise<void> { await this.hub?.stop(); }
  ngOnDestroy() { this.stopConnection(); }
}