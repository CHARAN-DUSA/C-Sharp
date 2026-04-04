import { Injectable } from '@angular/core';
@Injectable({ providedIn: 'root' })
export class StorageService {
  set<T>(key: string, value: T): void { localStorage.setItem(key, JSON.stringify(value)); }
  get<T>(key: string): T | null { try { const item = localStorage.getItem(key); return item ? JSON.parse(item) : null; } catch { return null; } }
  remove(key: string): void { localStorage.removeItem(key); }
  clear(): void { localStorage.clear(); }
}