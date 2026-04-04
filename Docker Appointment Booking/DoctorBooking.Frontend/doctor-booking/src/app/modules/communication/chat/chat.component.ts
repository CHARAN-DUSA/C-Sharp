import {
  Component, OnInit, OnDestroy, signal,
  ViewChild, ElementRef, AfterViewChecked
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Subscription, interval, startWith, switchMap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ChatMessage, Conversation } from '../../../core/models/chat.model';
import { Doctor } from '../../../core/models/doctor.model';
import { AuthService } from '../../../core/services/auth.service';
import { SignalrService } from '../../../core/services/signalr.service';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar.component';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent, SidebarComponent, TimeAgoPipe],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesEnd') messagesEnd!: ElementRef;

  // Chat state
  conversations  = signal<Conversation[]>([]);
  messages       = signal<ChatMessage[]>([]);
  activeConv     = signal<Conversation | null>(null);
  messageInput   = '';
  sending        = signal(false);
  chatTab        = signal<'conversations' | 'doctors'>('conversations');

  // Doctors list for new chats
  allDoctors       = signal<Doctor[]>([]);
  filteredDoctors  = signal<Doctor[]>([]);
  doctorsLoading   = signal(false);
  doctorSearch     = '';

  readonly currentUserId = this.auth.currentUser()?.userId ?? '';

  // Unread total count
  unreadTotal = () => this.conversations().reduce((s, c) => s + c.unreadCount, 0);

  private pollSub?:   Subscription;
  private convSub?:   Subscription;
  private signalSub?: Subscription;

  constructor(
    private http: HttpClient,
    readonly auth: AuthService,
    private signalr: SignalrService
  ) {}

  ngOnInit() {
    this.signalr.startConnection();

    // Poll conversations every 5s
    this.convSub = interval(5000).pipe(
      startWith(0),
      switchMap(() =>
        this.http.get<Conversation[]>(`${environment.apiUrl}/chat/conversations`)
      )
    ).subscribe(c => this.conversations.set(c));

    // Real-time incoming messages via SignalR
    this.signalSub = this.signalr.messageReceived$.subscribe((msg: any) => {
      if (this.activeConv()?.participantId === msg.senderId) {
        this.messages.update(m => [...m, msg]);
        this.markRead(msg.senderId);
      }
      // Refresh conversation list to update unread count
      this.http.get<Conversation[]>(`${environment.apiUrl}/chat/conversations`)
        .subscribe(c => this.conversations.set(c));
    });

    // Load all doctors initially
    this.loadDoctors();
  }

  // ── Load all registered doctors ────────────────────────────────────────────
  loadDoctors() {
    this.doctorsLoading.set(true);
    this.http.get<Doctor[]>(`${environment.apiUrl}/doctors`).subscribe({
      next: d => {
        this.allDoctors.set(d);
        this.filteredDoctors.set(d);
        this.doctorsLoading.set(false);
      },
      error: () => this.doctorsLoading.set(false)
    });
  }

  filterDoctors() {
    const q = this.doctorSearch.toLowerCase();
    if (!q) {
      this.filteredDoctors.set(this.allDoctors());
      return;
    }
    this.filteredDoctors.set(
      this.allDoctors().filter(d =>
        d.fullName.toLowerCase().includes(q) ||
        d.specialty.toLowerCase().includes(q)
      )
    );
  }

  // ── Start a new chat with a doctor ─────────────────────────────────────────
  startChatWithDoctor(doc: Doctor) {
    // Create a virtual conversation and switch to it
    const conv: Conversation = {
      participantId:     doc.userId,
      participantName:   `Dr. ${doc.fullName}`,
      participantAvatar: doc.profilePicture,
      lastMessage:       '',
      lastMessageTime:   new Date().toISOString(),
      unreadCount:       0
    };
    this.selectConversation(conv);
    this.chatTab.set('conversations');
  }

  // ── Select a conversation and start polling messages ───────────────────────
  selectConversation(conv: Conversation) {
    this.activeConv.set(conv);
    this.pollSub?.unsubscribe();

    // Auto-poll messages every 3s
    this.pollSub = interval(3000).pipe(
      startWith(0),
      switchMap(() =>
        this.http.get<ChatMessage[]>(
          `${environment.apiUrl}/chat/messages/${conv.participantId}`
        )
      )
    ).subscribe(m => this.messages.set(m));

    this.markRead(conv.participantId);
  }

  // ── Send a message ──────────────────────────────────────────────────────────
  async send() {
    const content = this.messageInput.trim();
    if (!content || !this.activeConv()) return;
    this.sending.set(true);
    this.messageInput = '';

    // Optimistically add message to UI
    const optimistic: ChatMessage = {
      id:          Date.now(),
      senderId:    this.currentUserId,
      receiverId:  this.activeConv()!.participantId,
      senderName:  this.auth.currentUser()?.fullName ?? '',
      content,
      sentAt:      new Date().toISOString(),
      isRead:      false
    };
    this.messages.update(m => [...m, optimistic]);

    try {
      await this.signalr.sendMessage(this.activeConv()!.participantId, content);
    } catch {
      // Fallback to HTTP
      this.http.post(`${environment.apiUrl}/chat/send`, {
        receiverId: this.activeConv()!.participantId,
        content
      }).subscribe();
    }

    this.sending.set(false);
  }

  sendOnEnter(e: KeyboardEvent) {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      this.send();
    }
  }

  markRead(participantId: string) {
    this.http.patch(`${environment.apiUrl}/chat/read/${participantId}`, {})
      .subscribe();
  }

  isMine(msg: ChatMessage) {
    return msg.senderId === this.currentUserId;
  }

  ngAfterViewChecked() {
    this.messagesEnd?.nativeElement?.scrollIntoView({ behavior: 'smooth' });
  }

  ngOnDestroy() {
    this.pollSub?.unsubscribe();
    this.convSub?.unsubscribe();
    this.signalSub?.unsubscribe();
    this.signalr.stopConnection();
  }
}