import { Component } from '@angular/core';
@Component({ selector: 'app-loader', standalone: true,
  template: `<div class="d-flex justify-content-center py-5"><div class="spinner-border text-primary" style="width:3rem;height:3rem"></div></div>`,
  styleUrls: ['./loader.component.css'] })
export class LoaderComponent {}