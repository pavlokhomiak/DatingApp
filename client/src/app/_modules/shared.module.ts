import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    // dropdown from ngx-bootstrap
    BrowserAnimationsModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    NgxGalleryModule,
    NgxSpinnerModule.forRoot({ 
      type: 'like-scale-party' 
    })
  ],
  // export libs as well
  exports: [
    BrowserAnimationsModule,
    BsDropdownModule,
    ToastrModule,
    NgxGalleryModule,
    NgxSpinnerModule
  ]
})
export class SharedModule { }
