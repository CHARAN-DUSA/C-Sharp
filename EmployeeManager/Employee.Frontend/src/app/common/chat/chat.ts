import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { ChatService } from '../../shared/services/chat.service';
import { MessageModel } from '../../shared/models/message.model';
import { SessionService } from '../../shared/services/session.service';
import { EmployeeService } from '../../shared/services/employee-service';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrls: ['./chat.css']
})
export class ChatComponent implements OnInit, OnDestroy {

  @ViewChild('chatContainer') chatContainer!: ElementRef;

  messages: MessageModel[] = [];
  messageText = '';
  selectedUserId: number | null = null;
  selectedFile!: File;

  userId = 0;
  users: any[] = [];

  typingUser = '';
  connection!: signalR.HubConnection;

  constructor(
    private chatService: ChatService,
    private sessionService: SessionService,
    private employeeService: EmployeeService
  ) {}

  ngOnInit() {
    const user = this.sessionService.getUser();
    this.userId = user?.employeeId || 0;

    this.loadUsers();
    this.loadMessages();
    this.startSignalR();
  }
  isImage(fileUrl: string): boolean {
  if (!fileUrl) return false;

  return fileUrl.includes('jpg') ||
         fileUrl.includes('jpeg') ||
         fileUrl.includes('png') ||
         fileUrl.includes('gif');
}

  loadUsers() {
    this.employeeService.getAll().subscribe((res: any[]) => {
      this.users = res.filter(u => u.employeeId !== this.userId);
    });
  }

  loadMessages() {
    this.chatService.getMessages(this.userId)
      .subscribe(res => {
        this.messages = res;
        setTimeout(() => this.scrollToBottom(), 100);
      });
  }

  startSignalR() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://employee-api-fuis.onrender.com/chatHub')
      .withAutomaticReconnect()
      .build();

    this.connection.start();

    this.connection.on('ReceiveMessage', (msg: MessageModel) => {
      this.messages.push(msg);
      setTimeout(() => this.scrollToBottom(), 100);

      if (msg.senderId !== this.userId) {
        alert(`New message from ${msg.senderName}`);
      }
    });

    this.connection.on('UserTyping', (name: string) => {
      this.typingUser = `${name} is typing...`;
      setTimeout(() => this.typingUser = '', 2000);
    });
  }

  // 📎 FILE SELECT
  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  // 🚀 SEND MESSAGE (TEXT + FILE)
  sendMessage() {
    if (!this.messageText.trim() && !this.selectedFile) return;

    if (this.selectedFile) {
      this.chatService.uploadFile(this.selectedFile).subscribe(res => {

        const payload = {
          senderId: this.userId,
          receiverId: this.selectedUserId,
          messageText: this.messageText,
          fileUrl: res.fileUrl
        };

        this.chatService.sendMessage(payload).subscribe(() => {
          this.messageText = '';
          this.selectedFile = undefined!;
        });

      });
    } else {
      const payload = {
        senderId: this.userId,
        receiverId: this.selectedUserId,
        messageText: this.messageText
      };

      this.chatService.sendMessage(payload)
        .subscribe(() => this.messageText = '');
    }
  }

  // ✍️ TYPING
  sendTyping() {
    const user = this.sessionService.getUser();
    this.connection.invoke('Typing', user?.name);
  }

  // 📜 AUTO SCROLL
  scrollToBottom() {
    try {
      this.chatContainer.nativeElement.scrollTop =
        this.chatContainer.nativeElement.scrollHeight;
    } catch {}
  }

  ngOnDestroy() {
    if (this.connection) this.connection.stop();
  }
}