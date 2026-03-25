export interface MessageModel {
  messageId: number;
  senderId: number;
  senderName: string;
  receiverId?: number | null;
  messageText?: string;
  fileUrl?: string;
  createdDate: string;
}